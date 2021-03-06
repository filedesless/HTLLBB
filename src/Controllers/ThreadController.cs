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

            Thread thread = await _context.Threads
                                          .Include((Thread t) => t.Author)
                                          .Include((Thread t) => t.Forum)
                                            .ThenInclude((Forum f) => f.Category)
                                          .Include((Thread t) => t.Posts)
                                            .ThenInclude((Post p) => p.Author) 
                                          .SingleOrDefaultAsync(t => t.Title == title);

            thread.Posts = thread.Posts.OrderBy(p => p.CreationTime).ToList();

            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            String userId = user.Id;

            if (thread == null) return NotFound();

            return View(new IndexViewModel { Thread = thread, IsAdmin = isAdmin, UserId = userId });
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
                if (await _context.Threads.CountAsync((Thread arg) => arg.Title == model.Title) > 0)
                {
                    ModelState.AddModelError("Title", "A Thread already exist with that title");
                    return View(model);
                }

                Thread thread = new Thread
                {
                    ForumId = model.ForumID,
                    Title = model.Title,
                    Author = await _userManager.GetUserAsync(User),
                    Content = model.Content,
                    CreationTime = DateTime.UtcNow,
                    Posts = new List<Post>(),
                };

                _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { Title = thread.Title });
            }

            return View(model);
        }

        // GET: Thread/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var thread = await _context.Threads.FindAsync(id.Value);

            if (thread == null) return NotFound();

            if (!await HasEditRight(id.Value)) return Forbid();


            return View(new EditViewModel { Title = thread.Title, Content = thread.Content });
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditViewModel model)
        {
            if (!await HasEditRight(id)) return Forbid();

            Thread threadToUpdate = await _context.Threads
                                                  .Include(t => t.Forum)
                                                  .SingleOrDefaultAsync(t => t.ID == id);

            if (threadToUpdate.Title != model.Title &&
                await _context.Threads.CountAsync((Thread arg) => arg.Title == model.Title) > 0)
                ModelState.AddModelError("Title", "A Thread already exist with that title");

            if (ModelState.IsValid)
            {
                try
                {
                    threadToUpdate.Title = model.Title;
                    threadToUpdate.Content = model.Content;
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var thread = await _context.Threads
                .Include(t => t.Forum)
                .SingleOrDefaultAsync(m => m.ID == id);
            
            if (thread == null) return NotFound();

            if (!await HasEditRight(id.Value)) return Forbid();

            return View(thread);
        }

        // POST: Thread/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await HasEditRight(id)) return Forbid();

            var thread = await _context.Threads
                                       .Include(t => t.Forum)
                                       .SingleOrDefaultAsync(m => m.ID == id);
            
            String forumName = thread.Forum.Name;
            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Forum", new { Name = forumName });
        }

        [Route("Thread/Search")]
        public IActionResult Search(String query)
        {
            if (String.IsNullOrWhiteSpace(query)) return NotFound();

            List<Thread> threads =
                _context.Threads
                        .Include(t => t.Author)
                        .Include(t => t.Posts)
                        .AsParallel() // vroom vroom!
                        .Where(t => t.Title.Contains(query)
                               || t.Posts.Any(p => p.Content.Contains(query))
                               || t.Content.Contains(query))
                        .OrderByDescending(t => t.CreationTime)
                        .ToList();

            return View(new SearchViewModel {
                Threads = threads
            });
        }

        private async Task<bool> HasEditRight(int id) 
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(user, Roles.Admin))
                return true;

            var thread = await _context.Threads
                                       .Include(t => t.Author)
                                       .SingleOrDefaultAsync(t => t.ID == id);

            if (user.Id == thread.Author.Id)
                return true;

            return false;
        }

        private bool ThreadExists(int id)
        {
            return _context.Threads.Any(e => e.ID == id);
        }
    }
}
