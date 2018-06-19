using CivicdAPI.Models;
using CivicdAPI.Models.DTO;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;

namespace CivicdAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class OrganizationController : ApiController
    {
        /// <summary>
        /// Create An Organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TagDTO))]
        [HttpPost]
        [Route("organizations")]
        public OrganizationDTO CreateOrganization(NewOrganizationDTO organization)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsInRole("Admin"))
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }
                var password = GeneratePasswordHash(organization.Password);
                var address = new Address()
                {
                    StreetAddressOne = organization.StreetAddressOne,
                    StreetAddressTwo = organization.StreetAddressTwo,
                    City = organization.City,
                    State = organization.State,
                    ZipCode = organization.ZipCode
                };

                var tags = context.Tags.Where(t => organization.Tags.Any(ot => ot.Id == t.ID)).ToList();

                var orgEntity = new ApplicationUser()
                {
                    Address = address,
                    Category = organization.OrganizationCategory,
                    DisplayName = organization.DisplayName,
                    Email = organization.Email,
                    FirstName = organization.FirstName,
                    LastName = organization.LastName,
                    PasswordHash = password,
                    PhoneNumber = organization.PhoneNumber,
                    ProfileDescription = organization.ProfileDescription,
                    UserName = organization.Email,
                    Tags = tags
                };

                context.Users.Add(orgEntity);
                context.SaveChanges();

                return new OrganizationDTO()
                {
                    City = organization.City,
                    DisplayName = organization.DisplayName,
                    Email = organization.Email,
                    FirstName = organization.FirstName,
                    LastName = organization.LastName,
                    OrganizationCategory = organization.OrganizationCategory,
                    PhoneNumber = organization.PhoneNumber,
                    ProfileDescription = organization.ProfileDescription,
                    State = organization.State,
                    StreetAddressOne = organization.StreetAddressOne,
                    StreetAddressTwo = organization.StreetAddressTwo,
                    ZipCode = organization.ZipCode,
                    Tags = organization.Tags
                };
            }
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="organizationEmail"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("organizations/{organizationEmail}/")]
        public OrganizationDTO UpdateOrganization(string organizationEmail, [FromBody] OrganizationDTO organization)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var selectedOrganization = context.Users.FirstOrDefault(o => o.Email == organizationEmail);

                if (selectedOrganization == null)
                {
                    throw new Exception("Unable to find matching Organization.");
                }
                if (!User.IsInRole("Admin") && selectedOrganization.Id != loggedInUser)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }

                var tags = context.Tags.Where(t => organization.Tags.Any(ot => ot.Id == t.ID)).ToList();

                selectedOrganization.Address.StreetAddressOne = organization.StreetAddressOne;
                selectedOrganization.Address.StreetAddressTwo = organization.StreetAddressTwo;
                selectedOrganization.Address.City = organization.City;
                selectedOrganization.Address.State = organization.State;
                selectedOrganization.Address.ZipCode = organization.ZipCode;
                selectedOrganization.Category = organization.OrganizationCategory;
                selectedOrganization.DisplayName = organization.DisplayName;
                selectedOrganization.Email = organization.Email;
                selectedOrganization.FirstName = organization.FirstName;
                selectedOrganization.LastName = organization.LastName;
                selectedOrganization.PhoneNumber = organization.PhoneNumber;
                selectedOrganization.ProfileDescription = organization.ProfileDescription;
                selectedOrganization.Tags = tags;

                context.SaveChanges();

                return organization;
            }
        }

        /// <summary>
        /// Delete an Organization
        /// </summary>
        /// <param name="organizationEmail"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("organizations/{organizationEmail}/")]
        public IHttpActionResult DeleteOrganization(string organizationEmail)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var selectedOrganization = context.Users.FirstOrDefault(o => o.Email == organizationEmail);

                if (!User.IsInRole("Admin") && selectedOrganization.Id != loggedInUser)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }

                context.Users.Remove(selectedOrganization);
                context.SaveChanges();

                return Ok();
            }
        }

        /// <summary>
        /// Get Organization by Email
        /// </summary>
        /// <param name="organizationEmail"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("organizations/{organizationEmail}/")]
        public OrganizationDTO GetOrganization(string organizationEmail)
        {
            using (var context = new ApplicationDbContext())
            {
                var organization = context.Users.FirstOrDefault(org => org.Email == organizationEmail);
                //TODO: More elegant solution for ensuring db user is org or user.
                if (organization == null || !organization.Roles.Any(role => role.RoleId == "27ca283a-bf04-4bec-8318-9baa09a50f77"))  
                {
                    throw new Exception("Unable to find matching Organization.");
                }

                return new OrganizationDTO()
                {
                    City = organization.Address.City,
                    DisplayName = organization.DisplayName,
                    Email = organization.Email,
                    FirstName = organization.FirstName,
                    LastName = organization.LastName,
                    OrganizationCategory = organization.Category,
                    PhoneNumber = organization.PhoneNumber,
                    ProfileDescription = organization.ProfileDescription,
                    State = organization.Address.State,
                    StreetAddressOne = organization.Address.StreetAddressOne,
                    StreetAddressTwo = organization.Address.StreetAddressTwo,
                    Tags = organization.Tags.Select(t => new TagDTO
                    {
                        Id = t.ID,
                        Name = t.Name
                    }),
                    ZipCode = organization.Address.ZipCode
                };
            }
        }

        private string GeneratePasswordHash(string passwordPlaintext)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(passwordPlaintext, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}