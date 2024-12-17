using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;
using UniqloMVC1.Enums;
using UniqloMVC1.Models;

namespace UniqloMVC1.FileExtensions
{
    public static class SeedExtension
    {
        public static void UseUserSeed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
              
                if (!roleManager.Roles.Any())
                {
                    foreach (Roles item in Enum.GetValues(typeof(Roles)))
                    {
                        roleManager.CreateAsync(new IdentityRole(item.ToString())).Wait();
                    }
                }

                if (!userManager.Users.Any(x => x.NormalizedUserName == "ADMIN"))
                {

                    User u = new User
                    {
                        Fullname = "admin",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        ProfileImageUrl = "photo.jpg"

                    };


                    userManager.CreateAsync(u, "Mini123").Wait();

                    
                    userManager.AddToRoleAsync(u, nameof(Roles.Admin)).Wait();
                }

            }



        }
    }
}