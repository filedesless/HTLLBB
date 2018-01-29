using System;
using System.Threading.Tasks;
using HTLLBB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HTLLBB.Data
{
    public static class DbInitializer
    {

        public static void Seed(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (! roleManager.RoleExistsAsync("Admin").Result )
                roleManager.CreateAsync(new IdentityRole { Name = "Admin" }).Wait();

            if (userManager.FindByNameAsync("root").Result == null)
            {
                var user = new ApplicationUser { 
                    UserName = "root", 
                    EmailConfirmed = true, 
                    Email = "root@localhost" 
                };
                userManager.CreateAsync(user, "Password123!").Wait();
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }
    }
}
