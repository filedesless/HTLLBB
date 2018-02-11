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
using HTLLBB.Repository;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        readonly IForumRepository _repo;
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public ForumController(IForumRepository repo, 
                                UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager) 
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Forum/{name}
        [Route("Forum/{name}")]
        public async Task<IActionResult> Index(String name)
        {
            if (string.IsNullOrEmpty(name)) return NotFound();

            var forum = await _repo.GetForumByName(name);
            if (forum == null) return NotFound();

            bool isAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            }

            return View(new IndexViewModel { Forum = forum, IsAdmin = isAdmin });
        }

        // GET: Forum/Create
        public IActionResult Create(int? id)
        {
            if (!id.HasValue) return NotFound();

            return View(new CreateViewModel { CatID = id.Value });
        }

        // POST: Forum/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _repo.ForumExists(model.Name))
                {
                    ModelState.AddModelError("Name", "A Forum already exist with that name");
                    return View();
                }

                var forum = new Forum { Name = model.Name };
                await _repo.AddForum(forum, model.CatID);
            }
            else
                return View(model);
            
            return RedirectToAction("Index", "Category");	
        }

        // GET: Forum/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var forum = await _repo.GetForumById(id.Value);
            if (forum == null) return NotFound();

            return View(new EditViewModel { Name = forum.Name });
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, EditViewModel model)
        {
            var forumToUpdate = await _repo.GetForumById(id);

            if (forumToUpdate.Name == model.Name)
                return RedirectToAction("Index", "Category");

            if (await _repo.ForumExists(model.Name))
                ModelState.AddModelError("Name", "A Forum already exist with that name");

            if (ModelState.IsValid)
            {
                forumToUpdate.Name = model.Name;
                await _repo.UpdForum(forumToUpdate);
                return RedirectToAction("Index", "Category");
            }

            return View(model);
        }

        // GET: Forums/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var forum = await _repo.GetForumById(id.Value);
            if (forum == null) return NotFound();

            return View(forum);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (!id.HasValue) return NotFound();

            await _repo.DelForum(id.Value);

            return RedirectToAction("Index", "Category");
        }
    }
}
