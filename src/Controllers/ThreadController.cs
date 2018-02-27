using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using HTLLBB.Models.ThreadViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Controllers
{
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

            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);

            if (thread == null) return NotFound();

            return View(new IndexViewModel { Thread = thread, IsAdmin = isAdmin });
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

        // GET: Thread/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var thread = await _context.Thread
                                       .SingleOrDefaultAsync(m => m.ID == id);
            
            if (thread == null) return NotFound();

            return View(new EditViewModel { Title = thread.Title });
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, EditViewModel model)
        {
            Thread threadToUpdate = await _context.Thread
                                                  .Include(t => t.Forum)
                                                  .SingleOrDefaultAsync(t => t.ID == id);

            if (threadToUpdate.Title == model.Title)
                return RedirectToAction("Index", "Forum", new { Name = threadToUpdate.Forum.Name });

            if (await _context.Thread.CountAsync((Thread arg) => arg.Title == model.Title) > 0)
                ModelState.AddModelError("Title", "A Thread already exist with that title");

            if (ModelState.IsValid)
            {
                try
                {
                    threadToUpdate.Title = model.Title;
                    _context.Update(threadToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Forum", new { Name = threadToUpdate.Forum.Name });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThreadExists(id))
                        return NotFound();

                }
            }

            return View(model);

        }

        // GET: Thread/Delete/5
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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

        [Route("Thread/Search")]
        public IActionResult Search(String query)
        {
            if (String.IsNullOrWhiteSpace(query)) return NotFound();

            List<Thread> threads =
                _context.Thread
                        .Include(t => t.Posts)
                        .AsParallel() // vroom vroom!
                        .Where(t => t.Title.Contains(query)
                               || t.Posts.Any(p => p.Content.Contains(query)))
                        .OrderByDescending(t => t.Posts.First().CreationTime)
                        .ToList();

            return View(new SearchViewModel {
                Threads = threads
            });
        }

        private bool ThreadExists(int id)
        {
            return _context.Thread.Any(e => e.ID == id);
        }
    }
}
