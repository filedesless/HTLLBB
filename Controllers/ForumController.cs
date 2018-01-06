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
    [Authorize(Roles = "Admin")]
    public class ForumController : ApplicationController
    {
        public ForumController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }

        // GET: Forums
        [Authorize]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return NotFound();

            bool isAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            }

            Forum forum = await _context.Forums
                                        .Include(f => f.Category)
                                        .Include(f => f.Threads)
                                            .ThenInclude(t => t.Posts)
                                            .ThenInclude(p => p.Author)
                                        .SingleOrDefaultAsync(f => f.ID == id);

            return View(new IndexViewModel { Forum = forum, IsAdmin = isAdmin });
        }

        // GET: Forums/Create
        public IActionResult Create(int? id)
        {
            if (id == null) return NotFound();

            return View(new CreateViewModel { CatID = (int)id });
        }

        // POST: Forums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cat = await _context.Categories.SingleAsync(c => c.ID == model.CatID);
                if (cat != null) 
                {
                    cat.Forums.Add(new Forum { Name = model.Name });
                    await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index", "Category");	
        }

        // GET: Forums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.ID == id);
            if (forum == null) return NotFound();

            return View(forum);
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Forum forum)
        {
            if (id != forum.ID)
            {
                return NotFound();
            }

            Forum forumToUpdate = await _context.Forums.SingleOrDefaultAsync(f => f.ID == forum.ID);

            if (await TryUpdateModelAsync(forumToUpdate, "", f => f.Name))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumExists(forum.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Category");
            }
            return View(forum);
        }

        // GET: Forums/Delete/5
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
