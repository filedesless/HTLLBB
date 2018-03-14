using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HTLLBB.Data;
using HTLLBB.Models;
using HTLLBB.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HTLLBB.Controllers
{
    public class ForumController : ApplicationController
    {
        public ForumController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }

        // GET: Forum/{name}
        [Route("Forum/{name}")]
        public async Task<IActionResult> Index(String name)
        {
            if (string.IsNullOrEmpty(name)) return NotFound();

            Forum forum = await _context.Forums
                                        .Include(f => f.Category)
                                        .Include(f => f.Threads)
                                            .ThenInclude(t => t.Posts)
                                            .ThenInclude(p => p.Author)
                                        .SingleOrDefaultAsync(f => f.Name == name);

            forum.Threads = forum.Threads.OrderBy(t => t.ID).ToList();

            if (forum == null) return NotFound();

            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            String userId = user.Id;

            return View(new IndexViewModel { Forum = forum, IsAdmin = isAdmin, UserId = userId });
        }

        // GET: Forum/Create
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create(int? id)
        {
            if (id == null) return NotFound();

            return View(new CreateViewModel { CatID = (int)id });
        }

        // POST: Forum/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Forums.CountAsync((Forum arg) => arg.Name == model.Name) > 0)
                {
                    ModelState.AddModelError("Name", "A Forum already exist with that name");
                    return View();
                }

                var cat = await _context.Categories.SingleAsync(c => c.ID == model.CatID);
                if (cat != null)
                {
                    cat.Forums.Add(new Forum { Name = model.Name });
                    await _context.SaveChangesAsync();
                }
            }
            else
                return View(model);
            
            return RedirectToAction("Index", "Category");	
        }

        // GET: Forum/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var forum = await _context.Forums
                                      .SingleOrDefaultAsync(m => m.ID == id);
            
            if (forum == null) return NotFound();

            return View(new EditViewModel { Name = forum.Name });
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, EditViewModel model)
        {
            Forum forumToUpdate = await _context.Forums.SingleOrDefaultAsync(f => f.ID == id);

            if (forumToUpdate.Name == model.Name)
                return RedirectToAction("Index", "Category");

            if (_context.Forums.Count((Forum arg) => arg.Name == model.Name) > 0)
                ModelState.AddModelError("Name", "A Forum already exist with that name");

            if (ModelState.IsValid)
            {
                try
                {
                    forumToUpdate.Name = model.Name;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Category");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumExists(id))
                        return NotFound();
                }
            }

            return View(model);

        }

        // GET: Forums/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var forum = await _context.Forums
                .SingleOrDefaultAsync(m => m.ID == id);
            if (forum == null)
            {
                return NotFound();
            }

            return View(forum);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound();

            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.ID == id);
            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Category");
        }

        private bool ForumExists(int id)
        {
            return _context.Forums.Any(e => e.ID == id);
        }
    }
}
