using CivicdAPI.Models;
using CivicdAPI.Models.DTO;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace CivicdAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/activities")]
    public class TagsController : ApiController
    {
        /// <summary>
        /// Create Tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TagDTO))]
        [HttpPost]
        [Route("tags/{tagName}")]
        public TagDTO CreateTag(string tagName)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsInRole("Admin"))
                {
                    throw new HttpException((int)HttpStatusCode.Forbidden, "This functionality is only available to Administrators.");
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
                return new TagDTO()
                {
                    Name = newTag.Name,
                    Id = newTag.ID
                };
            }
        }

        /// <summary>
        /// Delete a Tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [HttpDelete]
        [Route("tags/{tagName}")]
        public bool DeleteTag(string tagName)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsInRole("Admin"))
                {
                    throw new HttpException((int)HttpStatusCode.Forbidden, "This functionality is only available to Administrators.");
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TagDTO))]
        [HttpGet]
        [Route("tags")]
        public IEnumerable<TagDTO> GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tags.Select(dbTag => new TagDTO
                {
                    Id = dbTag.ID,
                    Name = dbTag.Name
                }).ToList();
            }
        }
    }
}
