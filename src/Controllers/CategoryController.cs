using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HTLLBB.Models.CategoryViewModels;
using HTLLBB.Repository;

namespace HTLLBB.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        readonly ICategoryRepository _repo;
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public CategoryController(ICategoryRepository repo,
                                  UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager)
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Forums
        [AllowAnonymous]
        [Route("Forums")]
        public async Task<IActionResult> Index()
        {
            bool isAdmin = false;
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            }

            IEnumerable<Category> categories = await _repo.GetCategories();

            categories.OrderBy( (Category arg) => arg.ID );

            foreach (var cat in categories)
                cat.Forums = cat.Forums.OrderBy(forum => forum.ID).ToList();

            return View(new IndexViewModel { 
                Categories = categories, 
                IsAdmin = isAdmin 
            });
        }

        // GET: Forums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Forums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (ModelState.IsValid)
                await _repo.AddCategory(category);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Forums/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var category = await _repo.GetCategoryById(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (!id.HasValue) return NotFound();

            await _repo.DelCategory(id.Value);

            return RedirectToAction(nameof(Index));
        }

        // GET: Forums/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var category = await _repo.GetCategoryById(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Category category)
        {
            if (id != category.ID) return NotFound();

            if (ModelState.IsValid)
            {
                await _repo.UpdCategory(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}
