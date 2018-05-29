using CivicdAPI.Models;
using Microsoft.AspNet.Identity;

namespace CivicdAPI.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CivicdAPI.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CivicdAPI.Models.ApplicationDbContext context)
        {
            // Roles
            var roleManager = new RoleManager<IdentityRole>(
              new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "User"))
            {
                roleManager.Create(new IdentityRole { Name = "User" });
            }
            if (!context.Roles.Any(r => r.Name == "Organization"))
            {
                roleManager.Create(new IdentityRole { Name = "Organization" });
            }

            // Users
            var userManager = new UserManager<ApplicationUser>(
              new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "getcivicd@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "getcivicd@gmail.com",
                    Email = "getcivicd@gmail.com",
                    FirstName = "Civicd",
                    LastName = "API",
                }, "Password1!");
            }

            var userId = userManager.FindByEmail("getcivicd@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(u => u.Email == "civicdgroup@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "civicdgroup@mailinator.com",
                    Email = "civicdgroup@mailinator.com",
                    FirstName = "Test",
                    LastName = "Organization",
                    DisplayName = "Civicd Group"
                }, "Password1!");
            }

            var userId2 = userManager.FindByEmail("civicdgroup@mailinator.com").Id;
            userManager.AddToRole(userId2, "Organization");

            if (!context.Users.Any(u => u.Email == "civicduser@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "civicduser@mailinator.com",
                    Email = "civicduser@mailinator.com",
                    FirstName = "Test",
                    LastName = "User",
                    DisplayName = "Civicd User"
                }, "Password1!");
            }

            var userId3 = userManager.FindByEmail("civicduser@mailinator.com").Id;
            userManager.AddToRole(userId3, "User");

            context.Tags.AddOrUpdate(
                p => p.Name,
                new Tag
                {
                    Name = "Liberal",
                },
                new Tag
                {
                    Name = "Conservative",
                },
                new Tag
                {
                    Name = "Moderate",
                },
                new Tag
                {
                    Name = "Activism",
                },
                new Tag
                {
                    Name = "Transit",
                },
                new Tag
                {
                    Name = "Feminism",
                },
                new Tag
                {
                    Name = "LGBTQ",
                },
                new Tag
                {
                    Name = "Immigration",
                },
                new Tag
                {
                    Name = "Racial Issues",
                },
                new Tag
                {
                    Name = "Disability",
                },
                new Tag
                {
                    Name = "Civil Rights",
                },
                new Tag
                {
                    Name = "Town Hall",
                },
                new Tag
                {
                    Name = "Net Neutrality",
                },
                new Tag
                {
                    Name = "Taxes",
                },
                new Tag
                {
                    Name = "Voting rights",
                },
                new Tag
                {
                    Name = "Inequality",
                },
                new Tag
                {
                    Name = "Income Gap",
                },
                new Tag
                {
                    Name = "Socialism",
                },
                new Tag
                {
                    Name = "Libertarism",
                },
                new Tag
                {
                    Name = "Affordable Housing",
                },
                new Tag
                {
                    Name = "Healthcare",
                },
                new Tag
                {
                    Name = "Obesity",
                },
                new Tag
                {
                    Name = "Mental Health",
                },
                new Tag
                {
                    Name = "Entitlements",
                },
                new Tag
                {
                    Name = "Environmental Issues",
                },
                new Tag
                {
                    Name = "Food Security",
                },
                new Tag
                {
                    Name = "International",
                },
                new Tag
                {
                    Name = "Arts",
                },
                new Tag
                {
                    Name = "Police",
                },
                new Tag
                {
                    Name = "Privacy",
                },
                new Tag
                {
                    Name = "Internet Connectivity",
                },
                new Tag
                {
                    Name = "Nutrition",
                },
                new Tag
                {
                    Name = "Social Media",
                },
                new Tag
                {
                    Name = "Grassroots",
                },
                new Tag
                {
                    Name = "Small business",
                }
            );

            // Addresses
            context.Addresses.AddOrUpdate(
                p => p.ID,
                new Address()
                {
                    StreetAddressOne = "101 Main Street",
                    City = "Charlotte",
                    State = "NC",
                    ZipCode = "28215"
                },
                new Address()
                {
                    StreetAddressOne = "2222 Ordinary Way",
                    City = "Charlotte",
                    State = "NC",
                    ZipCode = "28277"
                }
            );

            // Activities
            context.Activities.AddOrUpdate(
                p => p.DisplayTitle,
                new Activity
                {
                    DisplayTitle = "City Council Meeting",
                    Description =
                    "Lorem ipsum dolor sit amet, eripuit lobortis sapientem pri no, ut sed delenit honestatis. An diceret copiosae pri, ius quas possit ea. Id pri partiendo salutatus disputando. Id nam minim minimum repudiare, ex harum commune interesset usu. Semper dissentiunt eum in. Simul graeco tacimates ius in.",
                    Category = ActivityCategory.Government,
                    StartTime = new DateTimeOffset(2018, 05, 03, 13, 30, 00, new TimeSpan(0, 0, 0)),
                    EndTime = new DateTimeOffset(2018, 05, 03, 14, 30, 00, new TimeSpan(0, 0, 0))
                },
                new Activity
                {
                    DisplayTitle = "Phone Bank",
                    Description =
                    "Lorem ipsum dolor sit amet, eripuit lobortis sapientem pri no, ut sed delenit honestatis. An diceret copiosae pri, ius quas possit ea. Id pri partiendo salutatus disputando. Id nam minim minimum repudiare, ex harum commune interesset usu. Semper dissentiunt eum in. Simul graeco tacimates ius in.",
                    Category = ActivityCategory.IndependentActivity,
                    StartTime = new DateTimeOffset(2018, 05, 06, 16, 00, 00, new TimeSpan(0, 0, 0)),
                    EndTime = new DateTimeOffset(2018, 05, 06, 17, 00, 00, new TimeSpan(0, 0, 0))
                },
                new Activity
                {
                    DisplayTitle = "School Meeting",
                    Description = "Lorem ipsum dolor sit amet, eripuit lobortis sapientem pri no, ut sed delenit honestatis. An diceret copiosae pri, ius quas possit ea. Id pri partiendo salutatus disputando. Id nam minim minimum repudiare, ex harum commune interesset usu. Semper dissentiunt eum in. Simul graeco tacimates ius in.",
                    Category = ActivityCategory.School,
                    StartTime = new DateTimeOffset(2018, 05, 08, 10, 30, 00, new TimeSpan(0, 0, 0)),
                    EndTime = new DateTimeOffset(2018, 05, 08, 11, 30, 00, new TimeSpan(0, 0, 0))
                }
            );

            context.SaveChanges();

            // Add Tags to Activities
            var activity1 = context.Activities.Find(1);
            var activity2 = context.Activities.Find(2);
            var activity3 = context.Activities.Find(3);
            var tag1 = context.Tags.Find(1);
            var tag2 = context.Tags.Find(2);
            var tag3 = context.Tags.Find(3);
            var tag4 = context.Tags.Find(4);

            activity1.Tags.Add(tag1);
            activity1.Tags.Add(tag2);
            activity2.Tags.Add(tag3);
            activity3.Tags.Add(tag3);
            activity3.Tags.Add(tag1);
            activity3.Tags.Add(tag2);
            activity3.Tags.Add(tag4);

            // Add Address to Activity
            var address1 = context.Addresses.Find(1);
            var address2 = context.Addresses.Find(2);
            activity1.Address = address1;
            activity2.Address = address2;

            context.SaveChanges();

          
        }
    }
}
