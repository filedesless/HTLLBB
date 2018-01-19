using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HTLLBB.Controllers
{
    public class ApplicationController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
    }
}
