using CivicdAPI.Models;
using CivicdAPI.Models.DTO;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CivicdAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class ActivityActionController : ApiController
    {
        /// <summary>
        /// Get Activity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [Route("activities/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var activities = from a in db.Activities
                                 where a.ID == id
                                 select new ActivityDTO()
                                 {
                                     Id = a.ID,
                                     DisplayTitle = a.DisplayTitle,
                                     Description = a.Description,
                                     CategoryName = a.Category.ToString(),
                                     PhotoURL = a.Photo,
                                     StartTime = a.StartTime.ToString(),
                                     EndTime = a.EndTime.ToString(),
                                     AddressDisplayName = a.Address.Name,
                                     StreetAddressOne = a.Address.StreetAddressOne,
                                     StreetAddressTwo = a.Address.StreetAddressTwo,
                                     City = a.Address.City,
                                     State = a.Address.State,
                                     ZipCode = a.Address.ZipCode,
                                     Tags = from t in a.Tags
                                            select new TagDTO()
                                            {
                                                Id = t.ID,
                                                Name = t.Name
                                            }
                                 };
                return Ok(activities?.ToList());
            }
        }

        /// <summary>
        /// Get activities by tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [Route("activities/tags/{tagName}/")]
        [HttpGet]
        public IHttpActionResult GetByTag(string tagName)
        {
            using (var db = new ApplicationDbContext())
            {
                var tag = db.Tags.Include("Activities").FirstOrDefault(t => t.Name == tagName);
                if (tag == null)
                {
                    throw new Exception("No matching tags found");
                }

                var activities = tag.Activities.Select(act => new ActivityDTO
                {
                    AddressDisplayName = act.Address.Name,
                    CategoryName = act.Category.ToString(),
                    City = act.Address.City,
                    Description = act.Description,
                    DisplayTitle = act.DisplayTitle,
                    EndTime = act.EndTime.ToString(),
                    Id = act.ID,
                    PhotoURL = act.Photo,
                    StartTime = act.StartTime.ToString(),
                    State = act.Address.State,
                    StreetAddressOne = act.Address.StreetAddressOne,
                    StreetAddressTwo = act.Address.StreetAddressTwo,
                    ZipCode = act.Address.ZipCode,
                    Tags = act.Tags.Select(t => new TagDTO
                    {
                        Id = t.ID,
                        Name = t.Name
                    })
                });

                return Ok(activities?.ToList());
            }
        }

        /// <summary>
        /// Get activity. This will either get all activities or get a range of activities by date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [HttpGet]
        [Route("activities")]
        public IHttpActionResult GetByDateRange(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var db = new ApplicationDbContext())
            {
                var activities = db.Activities
                        .Include("Tags")
                        .Where(act => (startDate == null && endDate == null) || (act.StartTime <= startDate && act.EndTime >= endDate))
                        .Select(act => new ActivityDTO
                        {
                            AddressDisplayName = act.Address.Name,
                            CategoryName = act.Category.ToString(),
                            City = act.Address.City,
                            Description = act.Description,
                            DisplayTitle = act.DisplayTitle,
                            EndTime = act.EndTime.ToString(),
                            Id = act.ID,
                            PhotoURL = act.Photo,
                            StartTime = act.StartTime.ToString(),
                            State = act.Address.State,
                            StreetAddressOne = act.Address.StreetAddressOne,
                            StreetAddressTwo = act.Address.StreetAddressTwo,
                            ZipCode = act.Address.ZipCode,
                            Tags = act.Tags.Select(t => new TagDTO
                            {
                                Id = t.ID,
                                Name = t.Name
                            })
                        });
                return Ok(activities?.ToList());
            }
        }

        /// <summary>
        /// Get activities by category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [HttpGet]
        [Route("activities/categories/{categoryName}/")]
        public IHttpActionResult GetCategory(string categoryName)
        {
            var categoryInt = (ActivityCategory)Enum.Parse(typeof(ActivityCategory), categoryName, true);
            var categoryEnum = (ActivityCategory)Enum.ToObject(typeof(ActivityCategory), categoryInt);
            using (var db = new ApplicationDbContext())
            {

                var activities = from a in db.Activities
                                 where a.Category == categoryEnum
                                 select new ActivityDTO()
                                 {
                                     Id = a.ID,
                                     DisplayTitle = a.DisplayTitle,
                                     Description = a.Description,
                                     CategoryName = a.Category.ToString(),
                                     PhotoURL = a.Photo,
                                     StartTime = a.StartTime.ToString(),
                                     EndTime = a.EndTime.ToString(),
                                     StreetAddressOne = a.Address.StreetAddressOne,
                                     StreetAddressTwo = a.Address.StreetAddressTwo,
                                     City = a.Address.City,
                                     State = a.Address.State,
                                     Tags = from t in a.Tags
                                            select new TagDTO()
                                            {
                                                Id = t.ID,
                                                Name = t.Name
                                            }
                                 };
                return Ok(activities?.ToList());
            }
        }

        /// <summary>
        /// Get activities by organization
        /// </summary>
        /// <param name="organizationUserName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [HttpGet]
        [Route("activities/organizations/{organizationUserName}/")]
        public IHttpActionResult GetByOrganization(string organizationUserName)
        {
            using (var db = new ApplicationDbContext())
            {
                var activities = db.UserActivities
                        .Include("Activity")
                        .Include("User")
                        .Where(ua => ua.User.UserName == organizationUserName)
                        .Select(ua => ua.Activity);

                var activityModels = activities.Select(act => new ActivityDTO
                {
                    AddressDisplayName = act.Address.Name,
                    CategoryName = act.Category.ToString(),
                    City = act.Address.City,
                    Description = act.Description,
                    DisplayTitle = act.DisplayTitle,
                    EndTime = act.EndTime.ToString(),
                    Id = act.ID,
                    PhotoURL = act.Photo,
                    StartTime = act.StartTime.ToString(),
                    State = act.Address.State,
                    StreetAddressOne = act.Address.StreetAddressOne,
                    StreetAddressTwo = act.Address.StreetAddressTwo,
                    ZipCode = act.Address.ZipCode,
                    Tags = act.Tags.Select(t => new TagDTO
                    {
                        Id = t.ID,
                        Name = t.Name
                    })
                });
                return Ok(activityModels?.ToList());
            }
        }

        /// <summary>
        /// Get activities by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [HttpGet]
        [Route("activities/users/{userName}/")]
        public IHttpActionResult GetByUser(string userName)
        {
            var loggedInUser = User.Identity.GetUserId();
            using (var db = new ApplicationDbContext())
            {

                var selectedUser = db.Users.FirstOrDefault(user => user.UserName == userName);

                if (selectedUser.Id != loggedInUser || !User.IsInRole("Admin"))
                {
                    throw new Exception("You can not access another user's activities.");
                }

                var activities = db.UserActivities
                    .Include("Activity")
                    .Include("User")
                    .Where(ua => ua.User.UserName == userName)
                    .Select(ua => ua.Activity);

                var models = activities.Select(act => new ActivityDTO
                {
                    AddressDisplayName = act.Address.Name,
                    CategoryName = act.Category.ToString(),
                    City = act.Address.City,
                    Description = act.Description,
                    DisplayTitle = act.DisplayTitle,
                    EndTime = act.EndTime.ToString(),
                    Id = act.ID,
                    PhotoURL = act.Photo,
                    StartTime = act.StartTime.ToString(),
                    State = act.Address.State,
                    StreetAddressOne = act.Address.StreetAddressOne,
                    StreetAddressTwo = act.Address.StreetAddressTwo,
                    ZipCode = act.Address.ZipCode,
                    Tags = act.Tags.Select(t => new TagDTO
                    {
                        Id = t.ID,
                        Name = t.Name
                    })
                });

                return Ok(models?.ToList());
            }
        }

        /// <summary>
        /// RSVP to an activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("activities/{activityId:int}/rsvps/{userName}/")]
        public IHttpActionResult RSVP(int activityId, string userName)
        {

            using (var context = new ApplicationDbContext())
            {
                var rsvp = new UserActivity();
                var activity = context.Activities.FirstOrDefault(a => a.ID == activityId);
                var user = context.Users.FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                {
                    throw new Exception("Invalid Username");
                }
                if (activity == null)
                {
                    throw new Exception("Invalid Activity Id");
                }
                if (!User.IsInRole("Admin") && User.Identity.GetUserId() != user.Id)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                rsvp.Activity = activity;
                rsvp.User = user;

                context.UserActivities.Add(rsvp);
                context.SaveChanges();

                return Ok();
            }
        }

        /// <summary>
        /// Check in for an Activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("activities/{activityId:int}/checkins/{userName}/")]
        public IHttpActionResult CheckInActivity(int activityId, string userName)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                {
                    throw new Exception("Invalid Username");
                }

                var activity = context.UserActivities.FirstOrDefault(ua => ua.ActivityID == activityId && ua.User.Id == user.Id);
                if (activity == null)
                {
                    throw new Exception("Invalid Activity or User is Not RSVP'd for this Activity");
                }
                if (!User.IsInRole("Admin") && User.Identity.GetUserId() != user.Id)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                activity.CheckedIn = true;
                context.SaveChanges();

                return Ok();
            }
        }

        /// <summary>
        /// Add an Activity
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [Route("activities")]
        public ActivityDTO CreateActivity(ActivityDTO activity)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var user = context.Users.Single(u => u.Id == loggedInUser);
                var organization = context.Users.Single(u => u.Email == activity.Organization.Email);

                var userIsAdmin = User.IsInRole("Admin");
                var userIsOrg = User.IsInRole("Organization");
                if (User.IsInRole("User") || (userIsOrg && user.Id != organization.Id))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                DateTimeOffset endDate;

                DateTimeOffset.TryParse(activity.EndTime, out endDate);
                
                var activityEntity = new Activity()
                {
                    DisplayTitle = activity.DisplayTitle,
                    Category = (ActivityCategory)Enum.Parse(typeof(ActivityCategory), activity.CategoryName, true),
                    StartTime = DateTimeOffset.Parse(activity.StartTime),
                    EndTime = endDate == default(DateTimeOffset) ? default(DateTimeOffset) : endDate
                };

                if (!String.IsNullOrEmpty(activity.StreetAddressOne))
                {
                    activityEntity.Address = new Address()
                    {
                        StreetAddressOne = activity.StreetAddressOne,
                        StreetAddressTwo = activity.StreetAddressTwo,
                        City = activity.City,
                        State = activity.State,
                        ZipCode = activity.ZipCode,
                        Name = activity.AddressDisplayName
                    };
                }

                var userActivity = new UserActivity()
                {
                    Activity = activityEntity,
                    User = user,
                    Host = !userIsAdmin,
                    CheckedIn = organization.Id == user.Id
                };

                context.Activities.Add(activityEntity);
                context.UserActivities.Add(userActivity);

                context.SaveChanges();
                activity.Id = activityEntity.ID;

                return activity;
            }
        }

        /// <summary>
        /// Update an Activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityDTO))]
        [Route("activities/{activityId:int}")]
        public ActivityDTO UpdateActivity(int activityId, ActivityDTO activity)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var user = context.Users.Single(u => u.Id == loggedInUser);
                var organization = context.Users.Single(u => u.Email == activity.Organization.Email);

                var userIsAdmin = User.IsInRole("Admin");
                var userIsOrg = User.IsInRole("Organization");
                if (User.IsInRole("User") || (userIsOrg && user.Id != organization.Id))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                var activityEntity = context.UserActivities.FirstOrDefault(a => a.ActivityID == activityId && user.Id == a.UserID).Activity;
                if (activityEntity == null)
                {
                    throw new Exception("Unable to Find Matching Activity");
                }

                activityEntity.DisplayTitle = activity.DisplayTitle;
                activityEntity.Category = (ActivityCategory)Enum.Parse(typeof(ActivityCategory), activity.CategoryName);
                activityEntity.StartTime = DateTimeOffset.Parse(activity.StartTime);

                var endDate = new DateTimeOffset();
                DateTimeOffset.TryParse(activity.EndTime, out endDate);
                activityEntity.EndTime = endDate == default(DateTimeOffset) ? default(DateTimeOffset?) : endDate;
                if (!String.IsNullOrEmpty(activity.StreetAddressOne))
                {
                    if (activityEntity.Address == null)
                    {
                        activityEntity.Address = new Address()
                        {
                            StreetAddressOne = activity.StreetAddressOne,
                            StreetAddressTwo = activity.StreetAddressTwo,
                            City = activity.City,
                            State = activity.State,
                            ZipCode = activity.ZipCode,
                            Name = activity.AddressDisplayName
                        };
                    }
                    else
                    {
                        activityEntity.Address.StreetAddressOne = activity.StreetAddressOne;
                        activityEntity.Address.StreetAddressTwo = activity.StreetAddressTwo;
                        activityEntity.Address.City = activity.City;
                        activityEntity.Address.State = activity.State;
                        activityEntity.Address.ZipCode = activity.ZipCode;
                        activityEntity.Address.Name = activity.AddressDisplayName;
                    }
                }

                context.SaveChanges();
                return activity;
            }
        }

        /// <summary>
        /// Delete an Activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [Route("activities/{activityId:int}")]
        public bool DeleteActivity(int activityId)
        {
            using (var context = new ApplicationDbContext())
            {
                var loggedInUser = User.Identity.GetUserId();
                var user = context.Users.Single(u => u.Id == loggedInUser);
                var activityEntity = context.UserActivities.Include("Activities").FirstOrDefault(a => a.ActivityID == activityId);

                var organization = context.Users.Single(u => u.Email == activityEntity.User.Email);

                var userIsAdmin = User.IsInRole("Admin");
                var userIsOrg = User.IsInRole("Organization");
                if (User.IsInRole("User") || (userIsOrg && user.Id != organization.Id))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }
                if (activityEntity == null)
                {
                    throw new Exception("Unable to Find Matching Activity");
                }

                context.Activities.Remove(activityEntity.Activity);
                context.UserActivities.Remove(activityEntity);

                context.SaveChanges();
                return true;
            }
        }
    }
}
