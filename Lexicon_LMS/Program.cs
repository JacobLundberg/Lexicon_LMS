using System;
using Lexicon_LMS.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lexicon_LMS.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Lexicon_LMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateWebHostBuilder(args).Build().Run();
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();

                // dotnet user-secrets set  "GYM:AdminPW" "FooBar77!" + admin@gym.se

                // var AdminPw = config["GYM:AdminPW"];
                try
                {
                    // SeedData.Initialize(services, AdminPw).Wait();
                    SeedData.Initialize(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "An error occurred seeding the DB.");
                }
            }

            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

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
