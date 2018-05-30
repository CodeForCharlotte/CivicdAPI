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
        var password = GeneratePasswordHash(user.Password);
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
          DisplayName = user.DisplayName,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          PasswordHash = password,
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

        var selectedUser = userManager.FindByEmail(user.Email);

        if (selectedUser == null)
        {
          throw new Exception("Unable to find matching user.");
        }
        if (!User.IsInRole("Admin") && selectedUser.Id != loggedInUser)
        {
          throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
        }

        List<Tag> dbTags = context.Tags.ToList();

        List<Tag> tags = dbTags.Where(t => user.Tags.Any(ot => ot.Name == t.Name)).ToList();

        selectedUser.Address.StreetAddressOne = user.StreetAddressOne;
        selectedUser.Address.StreetAddressTwo = user.StreetAddressTwo;
        selectedUser.Address.City = user.City;
        selectedUser.Address.State = user.State;
        selectedUser.Address.ZipCode = user.ZipCode;
        selectedUser.Category = (OrganizationCategory)user.Category;
        selectedUser.DisplayName = user.DisplayName;
        selectedUser.Email = user.Email;
        selectedUser.FirstName = user.FirstName;
        selectedUser.LastName = user.LastName;
        selectedUser.PhoneNumber = user.PhoneNumber;
        selectedUser.ProfileDescription = user.ProfileDescription;
        selectedUser.Tags = tags;

        userManager.Update(selectedUser);

        context.SaveChanges();

        return new UserDTO()
        {
          City = selectedUser.Address.City,
          DisplayName = selectedUser.DisplayName,
          Email = selectedUser.Email,
          FirstName = selectedUser.FirstName,
          LastName = selectedUser.LastName,
          Category = (int)selectedUser.Category,
          PhoneNumber = selectedUser.PhoneNumber,
          ProfileDescription = selectedUser.ProfileDescription,
          State = selectedUser.Address.State,
          StreetAddressOne = selectedUser.Address.StreetAddressOne,
          StreetAddressTwo = selectedUser.Address.StreetAddressTwo,
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