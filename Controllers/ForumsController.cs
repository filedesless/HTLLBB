using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HTLLBB.Data;
using HTLLBB.Models;
using HTLLBB.Models.ForumsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HTLLBB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ForumsController : ApplicationController
    {
        public ForumsController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }

        // GET: Forums
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            bool isAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            }
            IEnumerable<Category> categories = await _context.Categories.Include(c => c.Forums).ToListAsync();

            return View(new IndexViewModel { Categories = categories, IsAdmin = isAdmin });
        }

        // GET: Forums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .SingleOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Forums/Create
        public IActionResult Create(int id)
        {
            return View(new CreateViewModel { CatID = id });
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
            return RedirectToAction(nameof(Index));	
        }

        // GET: Forums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.ID == id);
            if (forum == null)
            {
                return NotFound();
            }
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
                return RedirectToAction(nameof(Index));
            }
            return View(forum);
        }

        // GET: Forums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.ID == id);
            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumExists(int id)
        {
            return _context.Forums.Any(e => e.ID == id);
        }
    }
}
