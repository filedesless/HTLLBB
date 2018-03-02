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
                    Id = user.Id,
                    Name = user.UserName,
                    Role = String.IsNullOrEmpty(roles) ? " - " : roles,
                    Avatar = user.AvatarPath
                });
            }
                

            return View(new IndexViewModel
            {
                IsAdmin = isAdmin,
                Members = members
            });
        }

        // GET /Member/Edit/{id}
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return NotFound();

            ApplicationUser user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(new EditViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = ""
            });
        }

        // POST /Member/Edit/id
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(String id, EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View();
            
            if (_context.Users.Any(u => u.UserName == model.UserName && u.Id != id))
            {
                ModelState.AddModelError(nameof(model.UserName), "A user already exists with that username");
                return View();
            }

            if (_context.Users.Any(u => u.Email == model.Email && u.Id != id))
            {
                ModelState.AddModelError(nameof(model.Email), "A user already exists with that email");
                return View();
            }

            ApplicationUser user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;

            if (model.Email != user.Email)
            {
                String token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                await _userManager.ChangeEmailAsync(user, model.Email, token);
            }

            if (!String.IsNullOrWhiteSpace(model.Password)) 
            {
                String token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, model.Password);
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET /Member/Delete/{id}
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return NotFound();

            ApplicationUser user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(new DeleteViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
            });
        }

        // POST /Member/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return NotFound();

            ApplicationUser user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
