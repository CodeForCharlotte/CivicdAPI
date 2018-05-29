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
  public class OrganizationController : ApiController
  {
    [HttpPost]
    [Route("organizations")]
    public OrganizationDTO CreateOrganization(NewOrganizationDTO organization)
    {
      using (var context = new ApplicationDbContext())
      {
        if (organization.OrganizationCategory != 0 && !User.IsInRole("Admin"))
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
          Category = (OrganizationCategory)organization.OrganizationCategory,
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

        var userManager = new UserManager<ApplicationUser>(
          new UserStore<ApplicationUser>(context));
        var userId = userManager.FindByEmail(organization.Email).Id;
        if (organization.OrganizationCategory == 0)
        {
          userManager.AddToRole(userId, "User");
        }
        else
        {
          userManager.AddToRole(userId, "Organization");
        }

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

    [Authorize]
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
        selectedOrganization.Category = (OrganizationCategory)organization.OrganizationCategory;
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

    [Authorize]
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

    [Authorize]
    [HttpGet]
    [Route("organizations/{organizationEmail}/")]
    public OrganizationDTO GetOrganization(string organizationEmail)
    {
      using (var context = new ApplicationDbContext())
      {
        var organization = context.Users.FirstOrDefault(org => org.Email == organizationEmail);
        if (organization == null)
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
          OrganizationCategory = (int)organization.Category,
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