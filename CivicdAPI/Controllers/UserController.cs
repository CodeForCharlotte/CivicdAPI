using CivicdAPI.Models;
using CivicdAPI.Models.DTO;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CivicdAPI.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("users")]
        public UserDTO CreateUser(NewUserDTO user)
        {
            using (var context = new ApplicationDbContext())
            {
                if (user.Category != 0 && !User.IsInRole("Admin"))
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }

                AssertIsValid((OrganizationCategory)user.Category);

                var address = new Address()
                {
                    StreetAddressOne = user.StreetAddressOne,
                    StreetAddressTwo = user.StreetAddressTwo,
                    City = user.City,
                    State = user.State,
                    ZipCode = user.ZipCode
                };

                List<Tag> dbTags = context.Tags.ToList();

                List<Tag> tags = dbTags.Where(t => user.Tags.Any(ot => ot.Name == t.Name)).ToList();

                var userManager = new UserManager<ApplicationUser>(
                  new UserStore<ApplicationUser>(context));

                var userEntity = new ApplicationUser()
                {
                    Address = address,
                    Category = (OrganizationCategory)user.Category,
                    DisplayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.FirstName + user.LastName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ProfileDescription = user.ProfileDescription,
                    UserName = user.Email,
                    Tags = tags
                };

                if (!context.Users.Any(u => u.Email == user.Email))
                {
                    userManager.Create(userEntity);
                }
                else
                {
                    throw new Exception("A user by that e-mail already exists. Please specify a different e-mail.");
                }

                var userCreated = userManager.FindByEmail(user.Email);

                userManager.AddPassword(userCreated.Id, user.Password);

                if (user.Category == 0)
                {
                    userManager.AddToRole(userCreated.Id, "User");
                }
                else
                {
                    userManager.AddToRole(userCreated.Id, "Organization");
                }

                context.SaveChanges();

                return new UserDTO()
                {
                    City = userCreated.Address.City,
                    DisplayName = userCreated.DisplayName,
                    Email = userCreated.Email,
                    FirstName = userCreated.FirstName,
                    LastName = userCreated.LastName,
                    Category = (int)userCreated.Category,
                    PhoneNumber = userCreated.PhoneNumber,
                    ProfileDescription = userCreated.ProfileDescription,
                    State = userCreated.Address.State,
                    StreetAddressOne = userCreated.Address.StreetAddressOne,
                    StreetAddressTwo = userCreated.Address.StreetAddressTwo,
                    ZipCode = userCreated.Address.ZipCode,
                    Tags = from t in userCreated.Tags
                           select new TagDTO()
                           {
                               Id = t.ID,
                               Name = t.Name
                           }
                };
            }
        }

        [Authorize]
        [HttpPut]
        [Route("users/{userEmail}/")]
        public UserDTO UpdateUser(string userEmail, [FromBody] UserDTO user)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();

                var userManager = new UserManager<ApplicationUser>(
                  new UserStore<ApplicationUser>(context));

                var selectedUser = userManager.FindByEmail(userEmail);

                if (selectedUser == null)
                {
                    throw new Exception("Unable to find matching user.");
                }
                if (!User.IsInRole("Admin") && selectedUser.Id != loggedInUser)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }

                if (!string.IsNullOrEmpty(user.NewPassword))
                {
                    userManager.ChangePassword(selectedUser.Id, user.OldPassword, user.NewPassword);
                }


                if (selectedUser.Address != null)
                {
                    selectedUser.Address.StreetAddressOne =
                    SetNewValue(user.StreetAddressOne, selectedUser.Address.StreetAddressOne);
                    selectedUser.Address.StreetAddressTwo =
                      SetNewValue(user.StreetAddressTwo, selectedUser.Address.StreetAddressTwo);
                    selectedUser.Address.City = SetNewValue(user.City, selectedUser.Address.City);
                    selectedUser.Address.State = SetNewValue(user.State, selectedUser.Address.State);
                    selectedUser.Address.ZipCode = SetNewValue(user.ZipCode, selectedUser.Address.ZipCode);
                }
                selectedUser.DisplayName = SetNewValue(user.DisplayName, selectedUser.DisplayName);
                selectedUser.Email = SetNewValue(user.Email, selectedUser.Email);
                selectedUser.FirstName = SetNewValue(user.FirstName, selectedUser.FirstName);
                selectedUser.LastName = SetNewValue(user.LastName, selectedUser.LastName);
                selectedUser.PhoneNumber = SetNewValue(user.PhoneNumber, selectedUser.PhoneNumber);
                selectedUser.ProfileDescription = SetNewValue(user.ProfileDescription, selectedUser.ProfileDescription);

                if (user.Tags != null && user.Tags.Any())
                {
                    ICollection<Tag> dbTags = new List<Tag>();
                    foreach (var tag in user.Tags)
                    {
                        dbTags.Add(context.Tags.Find(tag.Id));
                    }

                    selectedUser.Tags = dbTags?.ToList();
                }

                if (User.IsInRole("Organization") || User.IsInRole("Admin") && user.Category < 7 && user.Category > 0)
                {
                    selectedUser.Category = (OrganizationCategory)user.Category;
                }

                userManager.Update(selectedUser);

                context.SaveChanges();

                return new UserDTO()
                {
                    City = selectedUser.Address.City,
                    DisplayName = selectedUser.DisplayName,
                    Email = selectedUser.Email,
                    FirstName = selectedUser.FirstName,
                    LastName = selectedUser.LastName,
                    OldPassword = "Successfully changed",
                    NewPassword = "Successfully changed",
                    Category = (int)selectedUser.Category,
                    PhoneNumber = selectedUser.PhoneNumber,
                    ProfileDescription = selectedUser.ProfileDescription,
                    State = selectedUser.Address?.State,
                    StreetAddressOne = selectedUser.Address?.StreetAddressOne,
                    StreetAddressTwo = selectedUser.Address?.StreetAddressTwo,
                    ZipCode = selectedUser.Address.ZipCode,
                    Tags = from t in selectedUser.Tags
                           select new TagDTO()
                           {
                               Id = t.ID,
                               Name = t.Name
                           }
                };
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("users/{userEmail}/")]
        public IHttpActionResult DeleteUser(string userEmail)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var selectedUser = context.Users.FirstOrDefault(o => o.Email == userEmail);

                if (!User.IsInRole("Admin") && selectedUser.Id != loggedInUser)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }

                context.Users.Remove(selectedUser);
                context.SaveChanges();

                return Ok();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("users/{userEmail}/")]
        public UserDTO GetUser(string userEmail)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(org => org.Email == userEmail);
                if (user == null)
                {
                    throw new Exception("Unable to find matching user.");
                }

                return new UserDTO()
                {
                    City = user.Address.City,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Category = (int)user.Category,
                    PhoneNumber = user.PhoneNumber,
                    ProfileDescription = user.ProfileDescription,
                    State = user.Address.State,
                    StreetAddressOne = user.Address.StreetAddressOne,
                    StreetAddressTwo = user.Address.StreetAddressTwo,
                    Tags = user.Tags.Select(t => new TagDTO
                    {
                        Id = t.ID,
                        Name = t.Name
                    }),
                    ZipCode = user.Address.ZipCode
                };
            }
        }

        private string SetNewValue(string newValue = null, string oldValue = null)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                return oldValue;
            }

            return newValue;
        }

        private bool AssertIsValid(Enum enumValue)
        {
            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentException("Organization Category is invalid.");
            }

            return true;
        }

        private void UpdateTags(ICollection<Tag> existingTags, ICollection<Tag> newTags)
        {
            var tagsToAdd = newTags.Except(existingTags).ToList();

            var tagsToRemove = existingTags.Except(newTags).ToList();

            foreach (var tag in tagsToAdd)
            {
                existingTags.Add(tag);
            }
            foreach (var tag in tagsToRemove)
            {
                existingTags.Remove(tag);
            }
        }
    }
}