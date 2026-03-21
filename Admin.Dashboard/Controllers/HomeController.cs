using Admin.Dashboard.Models;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Domain.Entities.ProductModules;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Admin.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork ,UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
           _unitOfWork = unitOfWork;
           _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = (await _unitOfWork.GetRepository<Product, int>().GetAllAsync()).Count();
            ViewBag.UserCount = _userManager.Users.Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
