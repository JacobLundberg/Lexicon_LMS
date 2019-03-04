using System;
using Lexicon_LMS.Models;
using System.Threading.Tasks;
using Lexicon_LMS.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

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
                    newUser.Name = "T Teacher";
                    newUser.Email = "t@t.com";
                    newUser.Password = "Teacher-1";
                    newUser.Role = "Teacher";

                    await CreateUserAsync(userManager, roleManager, newUser);

                    // Student
                    newUser = new newUser();
                    newUser.Name = "S Student";
                    newUser.Email = "s@s.com";
                    newUser.Password = "Student-1";
                    newUser.Role = "Student";

                    await CreateUserAsync(userManager, roleManager, newUser);


                    // Seed ActivityType
                    var activityTypes = new List<ActivityType>();
                    activityTypes.Add(new ActivityType { Name = "E-Learning", Description="Studenten lär sig på egen hand genom att se på video på datorn." });
                    activityTypes.Add(new ActivityType { Name = "Föreläsning", Description = "Läraren går igenom ett ämne." });
                    activityTypes.Add(new ActivityType { Name = "Övningstillfälle", Description = "Studenten får göra övningar för att se om hen behärskar ett ämne." });
                    activityTypes.Add(new ActivityType { Name = "Annat", Description = "Diverse andra aktiviteter." });
                    context.ActivityType.RemoveRange(context.ActivityType);
                    context.ActivityType.AddRange(activityTypes);
                    context.SaveChanges();

                    var indexPicker = new Random();


                    // Seed Course
                    var courses = new List<Course>();
                    courses.Add(new Course { Name = "C#", Description = "Programmeringskurs i Microsoft C#.", StartDate = DateTime.Parse("2019-03-28") });
                    courses.Add(new Course { Name = "C# Avancerad", Description = "Fortsättningskurs i Microsoft C#.", StartDate = DateTime.Parse("2019-04-18") });
                    courses.Add(new Course { Name = "Java", Description = "Programmeringskurs i Java.", StartDate = DateTime.Parse("2019-05-05") });
                    courses.Add(new Course { Name = "Python", Description = "Programmeringskurs i Python.", StartDate = DateTime.Parse("2019-05-13") });
                    courses.Add(new Course { Name = "Visual Basic", Description = "Programmeringskurs i Visual Basic.", StartDate = DateTime.Parse("2019-05-22") });
                    context.Course.RemoveRange(context.Course);
                    context.Course.AddRange(courses);
                    context.SaveChanges();


                    // Seed Module
                    var courseIds = context.Course.Select(a => a.Id).ToArray();
                    var modules = new List<Module>();
                    modules.Add(new Module { Name = "C# Grunderna", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Grunderna i Microsoft C#.", StartTime = DateTime.Parse("2019-03-28"), EndTime = DateTime.Parse("2019-03-29") });
                    modules.Add(new Module { Name = "C# Konsol", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Skapa ett konsolprogram.", StartTime = DateTime.Parse("2019-03-30"), EndTime = DateTime.Parse("2019-04-02") });
                    modules.Add(new Module { Name = "C# Generics", CourseId = courseIds[indexPicker.Next(0, courseIds.Length - 1)], Description = "Generics i Microsoft C#.", StartTime = DateTime.Parse("2019-04-03"), EndTime = DateTime.Parse("2019-04-05") });
                    context.Module.RemoveRange(context.Module);
                    context.Module.AddRange(modules);
                    context.SaveChanges();

                    // Seed ActivityModel
                    var activityTypeIds = context.ActivityType.Select(a => a.Id).ToArray();
                    var moduleIds = context.Module.Select(a => a.Id).ToArray();
                    var activityModels = new List<ActivityModel>();
                    activityModels.Add(new ActivityModel { Name = "C# Beginner", ModuleId = moduleIds[indexPicker.Next(0, moduleIds.Length - 1)], ActivityTypeId = activityTypeIds[indexPicker.Next(0, activityTypeIds.Length - 1)], Description = "E-learning med Scott Allen", StartDate = DateTime.Parse("2019-03-28 09:00"), StopDate = DateTime.Parse("2019-03-28 12:00") });
                    activityModels.Add(new ActivityModel { Name = "JavaScript Beginner", ModuleId = moduleIds[indexPicker.Next(0, moduleIds.Length - 1)], ActivityTypeId = activityTypeIds[indexPicker.Next(0, activityTypeIds.Length - 1)], Description = "Grunder i JavaScript", StartDate = DateTime.Parse("2019-03-28 09:00"), StopDate = DateTime.Parse("2019-03-28 12:00") });
                    activityModels.Add(new ActivityModel { Name = "CSS Beginner", ModuleId = moduleIds[indexPicker.Next(0, moduleIds.Length - 1)], ActivityTypeId = activityTypeIds[indexPicker.Next(0, activityTypeIds.Length - 1)], Description = "Grunderna i Cascading Style Sheets.", StartDate = DateTime.Parse("2019-03-28 09:00"), StopDate = DateTime.Parse("2019-03-28 12:00") });
                    context.ActivityModel.RemoveRange(context.ActivityModel);
                    context.ActivityModel.AddRange(activityModels);
                    context.SaveChanges();

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
