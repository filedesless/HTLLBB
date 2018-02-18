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
using HTLLBB.Repository;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class ThreadController : Controller
    {
        readonly IThreadRepository _repo;
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public ThreadController(IThreadRepository repo,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Thread
        [Route("Thread/{title}")]
        public async Task<IActionResult> Index(String title)
        {
            if (string.IsNullOrWhiteSpace(title)) return NotFound();

            Thread thread = await _repo.GetThreadByTitle(title);

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
                if (await _repo.ThreadExists(model.Title))
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

                await _repo.AddThread(thread);
                return RedirectToAction(nameof(Index), new { Title = thread.Title });
            }

            return View(model);
            
        }

        // GET: Thread/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            Thread thread = await _repo.GetThreadById(id.Value);
            
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
            Thread threadToUpdate = await _repo.GetThreadById(id);

            if (threadToUpdate.Title == model.Title)
                return RedirectToAction("Index", "Forum", new { Name = threadToUpdate.Forum.Name });

            if (await _repo.ThreadExists(model.Title))
                ModelState.AddModelError("Title", "A Thread already exist with that title");

            if (ModelState.IsValid)
            {
                threadToUpdate.Title = model.Title;
                await _repo.UpdThread(threadToUpdate);
                return RedirectToAction("Index", "Forum", new { Name = threadToUpdate.Forum.Name });
            }

            return View(model);

        }

        // GET: Thread/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var thread = await _repo.GetThreadById(id.Value);

            if (thread == null) return NotFound();

            return View(thread);
        }

        // POST: Thread/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thread = await _repo.GetThreadById(id);
            String forumName = thread.Forum.Name;

            await _repo.DelThread(id);

            return RedirectToAction("Index", "Forum", new { Name = forumName });
        }
    }
}
