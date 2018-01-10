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
    [Authorize(Roles="Admin")]
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var thread = await _context.Thread.SingleOrDefaultAsync(m => m.ID == id);
            if (thread == null) return NotFound();

            ViewData["ForumId"] = new SelectList(_context.Forums, "ID", "ID", thread.ForumId);
            return View(thread);
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,ForumId")] Thread thread)
        {
            if (id != thread.ID) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Thread.CountAsync((Thread arg) => arg.Title == thread.Title) > 0)
                {
                    ModelState.AddModelError("Name", "A Thread already exist with that title");
                    return View();
                }

                try
                {
                    _context.Update(thread);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThreadExists(thread.ID))
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
            ViewData["ForumId"] = new SelectList(_context.Forums, "ID", "ID", thread.ForumId);
            return View(thread);
        }

        // GET: Thread/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thread = await _context.Thread.SingleOrDefaultAsync(m => m.ID == id);
            _context.Thread.Remove(thread);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThreadExists(int id)
        {
            return _context.Thread.Any(e => e.ID == id);
        }
    }
}
