using System;
using System.Collections.Generic;
using System.Linq;
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

            var ctx = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (! roleManager.RoleExistsAsync(Roles.Admin).Result )
                roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin }).Wait();

            if (userManager.FindByNameAsync("root").Result == null)
            {
                var user = new ApplicationUser { 
                    UserName = "root", 
                    EmailConfirmed = true, 
                    Email = "root@localhost" 
                };
                userManager.CreateAsync(user, "Password123!").Wait();
                userManager.AddToRoleAsync(user, Roles.Admin).Wait();
            }

            if (! ctx.ChatboxChannels.Any(c => c.ID == 1))
            {
                ctx.ChatboxChannels.Add(new ChatboxChannel
                {
                    ID = 1,
                    Topic = "General",
                    Blocks = new List<ChatboxMessageBlock>(),
                });

                ctx.SaveChanges();
            }
        }
    }
}
