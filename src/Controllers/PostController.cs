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
using HTLLBB.Models.PostViewModels;

namespace HTLLBB.Controllers
{
    public class PostController : ApplicationController
    {
        public PostController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager) 
            : base(context, userManager, signInManager)
        {
        }


        // POST: Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            Thread thread = await _context.Thread
                              .SingleOrDefaultAsync(t => t.ID == model.ThreadID);

            if (ModelState.IsValid)
            {
                Post post = new Post
                {
                    Author = await _userManager.GetUserAsync(User),
                    Content = model.Content,
                    CreationTime = DateTime.UtcNow
                };

                thread.Posts.Add(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Thread", new { Title = thread.Title });
        }

        // GET: Post/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                                     .Include(p => p.Thread)
                                                .ThenInclude(t => t.Forum)
                                     .SingleOrDefaultAsync(m => m.ID == id);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Content")] Post post)
        {
            if (id != post.ID) return NotFound();

            Post postToUpdate = await _context.Posts
                                              .Include(p => p.Thread)
                                              .SingleOrDefaultAsync(p => p.ID == post.ID);

            try
            {
                postToUpdate.Content = post.Content;
                _context.Update(postToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index", "Thread", new { Title = postToUpdate.Thread.Title });
        }

        // GET: Post/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Thread)
                    .ThenInclude(t => t.Forum)
                .SingleOrDefaultAsync(m => m.ID == id);
            
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts
                                     .Include(p => p.Thread)
                                     .SingleOrDefaultAsync(m => m.ID == id);

            var thread = await _context.Thread
                                       .Include(t => t.Posts)
                                       .Include(t => t.Forum)
                                       .SingleOrDefaultAsync(t => t.ID == post.ThreadId);

            var firstPost = thread.Posts
                                .OrderBy(p => p.CreationTime)
                                .First();

            // deleting first post deletes thread
            if (post.ID == firstPost.ID)
            {
                _context.Thread.Remove(post.Thread);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Forum", new { Name = thread.Forum.Name });

            } else
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Thread", new { Title = post.Thread.Title });
            }

        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }
    }
}
