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
            Thread thread = await _context.Threads
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var post = await _context.Posts
                                     .Include(p => p.Thread)
                                                .ThenInclude(t => t.Forum)
                                     .SingleOrDefaultAsync(m => m.ID == id);
            
            if (post == null) return NotFound();

            if (!await HasEditRight(id.Value)) return Forbid();

            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Content")] Post post)
        {
            if (id != post.ID) return NotFound();

            if (!await HasEditRight(id)) return Forbid();

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            if (!await HasEditRight(id.Value)) return Forbid();

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await HasEditRight(id)) return Forbid();

            var post = await _context.Posts
                                     .Include(p => p.Thread)
                                     .SingleOrDefaultAsync(m => m.ID == id);

            var thread = await _context.Threads
                                       .Include(t => t.Posts)
                                       .Include(t => t.Forum)
                                       .SingleOrDefaultAsync(t => t.ID == post.ThreadId);
           
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Thread", new { Title = post.Thread.Title });
        }

        private async Task<bool> HasEditRight(int id) 
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(user, Roles.Admin))
                return true;

            var post = await _context.Posts
                                     .Include(p => p.Author)
                                     .SingleOrDefaultAsync(p => p.ID == id);

            if (user.Id == post.Author.Id)
                return true;

            return false;
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }
    }
}
