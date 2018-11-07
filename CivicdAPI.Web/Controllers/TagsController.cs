using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CivicdAPI.Web.Entities;
using CivicdAPI.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicdAPI.Web.Controllers
{
    [Authorize]
    [Route("api/activities")]
    public class TagsController : ControllerBase
    {
        /// <summary>
        /// Create Tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
       // [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TagDTO))]
        [HttpPost]
        [Route("tags/{tagName}")]
        public IActionResult CreateTag(string tagName)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsInRole("Admin"))
                {
                    throw new Exception(/*(int)HttpStatusCode.Forbidden,*/ "This functionality is only available to Administrators.");
                }
                if (context.Tags.Any(dbTag => dbTag.Name == tagName))
                {
                    throw new Exception(string.Format("Tag with tagname: {0} already exists!", tagName));
                }

                var newTag = new Tag()
                {
                    Name = tagName
                };

                context.Tags.Add(newTag);
                context.SaveChanges();
                var result = new TagDTO()
                {
                    Name = newTag.Name,
                    Id = newTag.ID
                };
                return new OkObjectResult(result);
            }
        }

        /// <summary>
        /// Delete a Tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
      
        [HttpDelete]
        [Route("tags/{tagName}")]
        public bool DeleteTag(string tagName)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsInRole("Admin"))
                {
                    throw new Exception( "This functionality is only available to Administrators.");
                }
                if (!context.Tags.Any(dbTag => dbTag.Name == tagName))
                {
                    throw new Exception("No Matching tag found.");
                }

                var tag = context.Tags.Single(dbTag => dbTag.Name == tagName);
                context.Tags.Remove(tag);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Get All Tags
        /// </summary>
        /// <returns></returns>
      
        [HttpGet]
        [Route("tags")]
        public IActionResult GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                var tags = context.Tags.Select(dbTag => new TagDTO
                {
                    Id = dbTag.ID,
                    Name = dbTag.Name
                }).ToList();

                return new OkObjectResult(tags);
            }
        }
    }
}
