using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admin.Dashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1️ check if the user exists 
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginDto);
            }

            // 2️ check role first before sign in
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            if (!isAdmin && !isSuperAdmin)
            {
                ModelState.AddModelError(string.Empty, "You Are Not Authorized.");
                return View(loginDto);
            }

            // 3️ check password
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginDto);
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
