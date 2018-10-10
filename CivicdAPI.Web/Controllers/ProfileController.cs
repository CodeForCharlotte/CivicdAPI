using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using global::CivicdAPI.Web.Entities;
using global::CivicdAPI.Web.Models;

namespace CivicdAPI.Controllers
{
    [Authorize]
    [Route("api")]
    public class ProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get User by Email
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserViewModel))]
        [HttpGet]
        [Route("User/{userEmail}")]
        public async Task<IActionResult> GetUserByEmail(string userEmail)
        {

            var matchedUser = await _userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);
            if (matchedUser == null)
            {
                //TODO: Specific exception message maybe?
                throw new Exception("Unable to Find Matching User");
            }

            return Ok(new UserViewModel
            {
                Email = matchedUser.Email,
                DisplayName = matchedUser.DisplayName,
                FirstName = matchedUser.FirstName,
                LastName = matchedUser.LastName,
                ProfileDescription = matchedUser.ProfileDescription,
                StreetAddressOne = matchedUser.Address?.StreetAddressOne,
                StreetAddressTwo = matchedUser.Address?.StreetAddressTwo,
                City = matchedUser.Address?.City,
                State = matchedUser.Address?.State,
                ZipCode = matchedUser.Address?.ZipCode,
                Tags = matchedUser.Tags?.Select(t => new TagDTO
                {
                    Id = t.ID,
                    Name = t.Name
                })
            });

        }
        /// <summary>
        /// Get Organization by Organization Id
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
      //  [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserViewModel))]
        [HttpGet]
        [Route("Organizations/{organizationId}")]
        public async Task<IActionResult> GetOrganizationById(string organizationId)
        {
            var matchedOrganization = await _userManager.FindByIdAsync(organizationId);
            if (matchedOrganization == null)
            {
                //TODO: specific exception message
                throw new Exception("Unable to Find Matching Organization.");
            }

            return Ok(new UserDTO
            {
                Email = matchedOrganization.Email,
                DisplayName = matchedOrganization.DisplayName,
                FirstName = matchedOrganization.FirstName,
                LastName = matchedOrganization.LastName,
                Category = (int)matchedOrganization.Category,
                ProfileDescription = matchedOrganization.ProfileDescription,
                StreetAddressOne = matchedOrganization.Address?.StreetAddressOne,
                StreetAddressTwo = matchedOrganization.Address?.StreetAddressTwo,
                City = matchedOrganization.Address?.City,
                State = matchedOrganization.Address?.State,
                ZipCode = matchedOrganization.Address?.ZipCode,
                Tags = matchedOrganization.Tags?.Select(t => new TagDTO
                {
                    Id = t.ID,
                    Name = t.Name
                })
            });
        }
    }

}

