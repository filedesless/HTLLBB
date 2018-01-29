using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HTLLBB.Models.ThreadViewModels;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class ThreadController : ApplicationController
    {
        public ThreadController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }

        // GET: Thread
        [Route("Thread/{title}")]
        public async Task<IActionResult> Index(String title)
        {
            if (string.IsNullOrWhiteSpace(title)) return NotFound();

            Thread thread = await _context.Thread
                                          .Include((Thread t) => t.Forum)
                                            .ThenInclude((Forum f) => f.Category)
                                          .Include((Thread t) => t.Posts)
                                            .ThenInclude((Post p) => p.Author) 
                                          .SingleOrDefaultAsync(t => t.Title == title);

            if (thread == null) return NotFound();

            return View(thread);
        }

        // GET: Thread/Create
        public IActionResult Create(int id)
        {
            var model = new CreateViewModel
            {
                ForumID = id
            };

            return View(model);
        }

        // POST: Thread/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Thread.CountAsync((Thread arg) => arg.Title == model.Title) > 0)
                {
                    ModelState.AddModelError("Title", "A Thread already exist with that title");
                    return View(model);
                }

                Thread thread = new Thread
                {
                    ForumId = model.ForumID,
                    Title = model.Title,
                    Posts = new List<Post> 
                    { 
                        new Post
                        {
                            Author = await _userManager.GetUserAsync(User),
                            Content = model.Content,
                            CreationTime = DateTime.UtcNow
                        }
                    }
                };

                _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { Title = thread.Title });
            }
            return View(model);
        }

        // TODO: 
        // GET: Thread/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var thread = await _context.Thread
                                       .Include(t => t.Forum)
                                       .SingleOrDefaultAsync(m => m.ID == id);
            if (thread == null) return NotFound();

            return View(thread);
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title")] Thread model)
        {
            if (id != model.ID) return NotFound();

            Thread threadToUpdate = await _context.Thread
                                                  .Include(t => t.Forum)
                                                  .SingleOrDefaultAsync(t => t.ID == model.ID);

            if (await _context.Thread.CountAsync((Thread arg) => arg.Title == model.Title) > 0)
            {
                ModelState.AddModelError("Title", "A Thread already exist with that title");
                return View();
            }

            try
            {
                threadToUpdate.Title = model.Title;
                _context.Update(threadToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Forum", new { Name = threadToUpdate.Forum.Name });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThreadExists(model.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Thread/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Thread
                .Include(t => t.Forum)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }

        // POST: Thread/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thread = await _context.Thread
                                       .Include(t => t.Forum)
                                       .SingleOrDefaultAsync(m => m.ID == id);
            String forumName = thread.Forum.Name;
            _context.Thread.Remove(thread);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Forum", new { Name = forumName });
        }

        private bool ThreadExists(int id)
        {
            return _context.Thread.Any(e => e.ID == id);
        }
    }
}
