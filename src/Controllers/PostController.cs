using System;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using HTLLBB.Models.PostViewModels;
using HTLLBB.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        readonly IPostRepository _repo;
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public PostController(IPostRepository repo,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager) 
            
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // POST: Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            Thread thread = await _repo.GetThreadById(model.ThreadID);

            if (ModelState.IsValid)
            {
                Post post = new Post
                {
                    Author = await _userManager.GetUserAsync(User),
                    Content = model.Content,
                    CreationTime = DateTime.UtcNow,
                };

                await _repo.AddPost(post, model.ThreadID);
            }

            return RedirectToAction("Index", "Thread", new { Title = thread.Title });
        }

        // GET: Post/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            Post post = await _repo.GetPostById(id.Value);
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

            Post postToUpdate = await _repo.GetPostById(id);

            postToUpdate.Content = post.Content;
            await _repo.UpdPost(postToUpdate);

            return RedirectToAction("Index", "Thread", new { Title = postToUpdate.Thread.Title });
        }

        // GET: Post/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            Post post = await _repo.GetPostById(id.Value);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Post post = await _repo.GetPostById(id);
            Thread thread = await _repo.GetThreadById(post.ThreadId);

            Post firstPost = thread.Posts
                                .OrderBy(p => p.CreationTime)
                                .First();

            // deleting first post deletes thread
            if (post.ID == firstPost.ID)
            {
                await _repo.DelThread(post.Thread.ID);
                return RedirectToAction("Index", "Forum", new { Name = thread.Forum.Name });
            }

            await _repo.DelPost(post.ID);
            return RedirectToAction("Index", "Thread", new { Title = post.Thread.Title });
        }
    }
}