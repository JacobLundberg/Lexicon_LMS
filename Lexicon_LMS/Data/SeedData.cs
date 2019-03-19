using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS
{
    public partial class Program
    {
        public class newUser
        {
            public string Name;
            public string Email;
            public string Password;
            public string Role;
        }

        public static class SeedData
        {
            public static async Task Initialize(IServiceProvider serviceProvider)
            {
                using (var context = new ApplicationDbContext(
                 serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    if (roleManager == null || userManager == null)
                    {
                        throw new Exception("roleManager or userManager is null");
                    }

                    // Roles
                    var roleNames = new[] { "Teacher", "Student" };
                    foreach (var name in roleNames)
                    {
                        // Lägg till rollen om den inte finns
                        bool adminRoleExists = await roleManager.RoleExistsAsync(name);
                        if (!adminRoleExists)
                        {
                            var role = new IdentityRole { Name = name };
                            var result = await roleManager.CreateAsync(role);
                            if (!result.Succeeded)
                            {
                                throw new Exception(string.Join("\n", result.Errors));
                            }
                        }
                    }

                    // Teacher
                    var newUser = new newUser();
                    newUser.Name = "Dimitris";
                    newUser.Email = "t@t.com";
                    newUser.Password = "Teacher-1";
                    newUser.Role = "Teacher";
                    await CreateUserAsync(userManager, roleManager, newUser);

                    // Student
                    newUser = new newUser();
                    newUser.Name = "Fredrik";
                    newUser.Email = "s@s.com";
                    newUser.Password = "Student-1";
                    newUser.Role = "Student";
                    await CreateUserAsync(userManager, roleManager, newUser);

                    newUser = new newUser();
                    newUser.Name = "Göran";
                    newUser.Email = "s1@s.com";
                    newUser.Password = "Student-1";
                    newUser.Role = "Student";
                    await CreateUserAsync(userManager, roleManager, newUser);

                    newUser = new newUser();
                    newUser.Name = "Jacob";
                    newUser.Email = "s2@s.com";
                    newUser.Password = "Student-1";
                    newUser.Role = "Student";
                    await CreateUserAsync(userManager, roleManager, newUser);

                    newUser = new newUser();
                    newUser.Name = "Saber";
                    newUser.Email = "s3@s.com";
                    newUser.Password = "Student-1";
                    newUser.Role = "Student";
                    await CreateUserAsync(userManager, roleManager, newUser);


                    var indexPicker = new Random();

                    // Seed ActivityType
                    if (context.ActivityType.Count() == 0)
                    {
                        var activityTypes = new List<ActivityType>();
                        activityTypes.Add(new ActivityType { Name = "E-Learning", Description = "Studenten lär sig på egen hand genom att se på video på datorn." });
                        activityTypes.Add(new ActivityType { Name = "Föreläsning", Description = "Läraren går igenom ett ämne." });
                        activityTypes.Add(new ActivityType { Name = "Övningstillfälle", Description = "Studenten får göra övningar för att se om hen behärskar ett ämne." });
                        activityTypes.Add(new ActivityType { Name = "Annat", Description = "Diverse andra aktiviteter." });
                        //context.ActivityType.RemoveRange(context.ActivityType);
                        context.ActivityType.AddRange(activityTypes);
                        context.SaveChanges();
                    }


                    // Seed Course
                    if (context.Course.Count() == 0)
                    {
                        var courses = new List<Course>();
                        courses.Add(new Course { Name = ".NET", Description = "Programmeringskurs i Microsoft .NET.", StartDate = DateTime.Parse("2018-11-26") });
                        courses.Add(new Course { Name = "Java", Description = "Programmeringskurs i Java.", StartDate = DateTime.Parse("2019-01-05") });
                        courses.Add(new Course { Name = "Tekniker", Description = "Lär dig bli datatekniker.", StartDate = DateTime.Parse("2019-02-25") });
                        //context.Course.RemoveRange(context.Course);
                        context.Course.AddRange(courses);
                        context.SaveChanges();
                    }

                    // Seed ApplicationUserCourse
                    if (context.UserCourse.Count() == 0)
                    {
                        var courseIds = context.Course.Where(c => c.Name == ".NET").Select(a => a.Id).ToArray();
                        var userIds = context.Users.Select(a => a.Id).ToArray();

                        var userCourses = new List<ApplicationUserCourse>();
                        for (int i = 0; i < userIds.Length; i++)
                            userCourses.Add(new ApplicationUserCourse
                            {
                                ApplicationUserId = userIds[i]
                                ,
                                CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)]
                            });

                        //context.Course.RemoveRange(context.Course);
                        context.UserCourse.AddRange(userCourses);
                        context.SaveChanges();
                    }

                    // Seed Module .NET Påbyggnadsutbildning
                    if (context.Module.Count() == 0)
                    {
                        var courseIds = context.Course.Where(c => c.Name == ".NET").Select(a => a.Id).ToArray();
                        var modules = new List<Module>();
                        modules.Add(new Module { Name = "C#", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "C# 7 (Objektorienterad programmering, Generics, Linq)", StartTime = DateTime.Parse("2018-11-26"), EndTime = DateTime.Parse("2018-12-14") });
                        modules.Add(new Module { Name = "Testning", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Unittestning mm.", StartTime = DateTime.Parse("2018-12-17"), EndTime = DateTime.Parse("2019-01-02") });
                        modules.Add(new Module { Name = "Webb", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "HTML5, CSS3, JavaScript.", StartTime = DateTime.Parse("2019-01-03"), EndTime = DateTime.Parse("2019-01-14") });
                        modules.Add(new Module { Name = "MVC", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "ASP.NET MVC Core.", StartTime = DateTime.Parse("2019-01-15"), EndTime = DateTime.Parse("2019-01-30") });
                        modules.Add(new Module { Name = "Databaser", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Entity framework & SQL.", StartTime = DateTime.Parse("2019-01-31"), EndTime = DateTime.Parse("2019-02-08") });
                        modules.Add(new Module { Name = "Webbapplikationer", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Förhöj användarupplevelsen.", StartTime = DateTime.Parse("2019-02-11"), EndTime = DateTime.Parse("2019-02-21") });
                        modules.Add(new Module { Name = "MVC Fördjupning", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Projektarbete Full-stack applikation.", StartTime = DateTime.Parse("2019-02-22"), EndTime = DateTime.Parse("2019-03-22") });
                        //context.Module.RemoveRange(context.Module);
                        context.Module.AddRange(modules);
                        context.SaveChanges();
                    }

                    // Seed ActivityModel
                    if (context.ActivityModel.Count() == 0)
                    {
                        string[] randomNames = { "Scott Allen", "Susan Johnson", "Bill Gates", "Jeff Bezos", "Steve Balmer" };
                        string[] randomWords = { "Fördjupning", "Grunderna", "Info om", "Mera om" };
                        var activityTypes = context.ActivityType.ToArray();
                        var modules = context.Module.ToArray();
                        var activityModels = new List<ActivityModel>();
                        foreach (var module in modules)
                        {
                            for (int i = 0; i < indexPicker.Next(1, 10); i++)
                            {
                                var activityTypeIndex = indexPicker.Next(0, activityTypes.Length - 1);
                                var dateIndex = (module.EndTime - module.StartTime).Days;
                                var startDate = module.StartTime.AddDays(indexPicker.Next(0, dateIndex - 3));
                                var stopDate = startDate.AddDays(indexPicker.Next(0, 3));
                                activityModels.Add(new ActivityModel
                                {
                                    Name = randomWords[indexPicker.Next(0, randomWords.Length - 1)]+" "+module.Name,
                                    ModuleId = module.Id,
                                    ActivityTypeId = activityTypes[activityTypeIndex].Id,
                                    Description = activityTypes[activityTypeIndex].Name + " med " + randomNames[indexPicker.Next(0, randomNames.Length - 1)] + ".",
                                    StartDate = startDate,
                                    StopDate = stopDate
                                });
                            }
                        }
                        context.ActivityModel.AddRange(activityModels);
                        context.SaveChanges();
                    }
                }
            }

            private static async Task<bool> CreateUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, newUser newUser)
            {
                Boolean success = false;

                if (userManager != null && roleManager != null)
                {
                    var foundUser = await userManager.FindByEmailAsync(newUser.Email);
                    if (foundUser == null)
                    {
                        var user = new ApplicationUser { Name = newUser.Name, UserName = newUser.Email, Email = newUser.Email };
                        var addUserResult = await userManager.CreateAsync(user, newUser.Password);

                        if (addUserResult.Succeeded)
                        {
                            var addToRoleResultAdmin = await userManager.AddToRoleAsync(user, newUser.Role);
                            if (addToRoleResultAdmin.Succeeded)
                                success = true;
                        }
                    }
                }

                return success;
            }
        }




    }
}
