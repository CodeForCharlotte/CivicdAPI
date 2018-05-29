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

        List<Tag> tags = context.Tags.SelectMany(t => user.Tags, (t, ot) => new { t, ot })
          .Where(@t1 => @t1.t.Name == @t1.ot.Name)
          .Select(@t1 => new Tag()
          {
            ID = @t1.t.ID,
            Name = @t1.t.Name
          }).ToList();


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

        context.Users.Add(userEntity);

        var userManager = new UserManager<ApplicationUser>(
          new UserStore<ApplicationUser>(context));
        var userId = userManager.FindByEmail(user.Email).Id;
        if (user.Category == 0)
        {
          userManager.AddToRole(userId, "User");
        }
        else
        {
          userManager.AddToRole(userId, "user");
        }

        context.SaveChanges();

        return new UserDTO()
        {
          City = user.City,
          DisplayName = user.DisplayName,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Category = user.Category,
          PhoneNumber = user.PhoneNumber,
          ProfileDescription = user.ProfileDescription,
          State = user.State,
          StreetAddressOne = user.StreetAddressOne,
          StreetAddressTwo = user.StreetAddressTwo,
          ZipCode = user.ZipCode,
          Tags = user.Tags
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
        var selectedUser = context.Users.FirstOrDefault(o => o.Email == userEmail);

        if (selectedUser == null)
        {
          throw new Exception("Unable to find matching user.");
        }
        if (!User.IsInRole("Admin") && selectedUser.Id != loggedInUser)
        {
          throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
        }

        var tags = context.Tags.Where(t => user.Tags.Any(ot => ot.Name == t.Name)).ToList();

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

        context.SaveChanges();

        return user;
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