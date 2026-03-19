using Admin.Dashboard.Models.Roles;
using Admin.Dashboard.Models.Users;
using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Admin.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager ,RoleManager<IdentityRole> roleManager)
        {
           _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            // get all users
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                
                var roles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();
           
            var userRoles = await _userManager.GetRolesAsync(user);

            var usermodel = new UserRoleViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = roles.Select(role => new UpdateRoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    
                    IsSelected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(usermodel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            // get roles to add
            var rolesToAdd = model.Roles
                .Where(r => r.IsSelected && !currentRoles.Contains(r.Name))
                .Select(r => r.Name);

            // get roles to remove
            var rolesToRemove = model.Roles
                .Where(r => !r.IsSelected && currentRoles.Contains(r.Name))
                .Select(r => r.Name);

            // add new roles
            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            // delete removed roles
            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove); 

            return RedirectToAction(nameof(Index));
        }
    }
}
