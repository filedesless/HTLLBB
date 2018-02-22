using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HTLLBB.Models.MemberViewModels;
using System.Linq;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class MemberController : ApplicationController
    {
        public MemberController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }

        // GET /Members
        [Route("Members")]
        public async Task<IActionResult> Index() 
        {
            ApplicationUser current_user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(current_user, Roles.Admin);

            List<Member> members = new List<Member>();

            foreach (var user in _context.Users)
            {
                String roles = String.Join(", ", await _userManager.GetRolesAsync(user));
                members.Add(new Member
                {
                    Name = user.UserName,
                    Role = String.IsNullOrEmpty(roles) ? " - " : roles
                });
            }
                

            return View(new IndexViewModel
            {
                IsAdmin = isAdmin,
                Members = members
            });
        }
    }
}
