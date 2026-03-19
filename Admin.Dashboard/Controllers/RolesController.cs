using Admin.Dashboard.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }
      
        //check if the role name is exist or not
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await _roleManager.RoleExistsAsync(model.Name);

                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Name));
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Name", "Role already exists.");


            }


            return View(nameof(Index), await _roleManager.Roles.ToListAsync());

        }
     
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is not null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is  null)
            {
                ModelState.AddModelError("Id", "No Role Exist With This Id.");
                return RedirectToAction(nameof(Index));
            }
           UpdateRoleViewModel updateModel = new UpdateRoleViewModel
            {
                Id = id,
                Name = role.Name!
            };

            return View(updateModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateRoleViewModel updatemodel)
        {
            if (!ModelState.IsValid)
                return View(updatemodel);

            // check if the role exist or not by id
            var role = await _roleManager.FindByIdAsync(updatemodel.Id);
            if (role is null)
            {
                ModelState.AddModelError("Id", "No role exists with this Id.");
                return View(updatemodel);
            }

            // check if the new role name already exists (excluding the current role)
            var roleNameExists = await _roleManager.RoleExistsAsync(updatemodel.Name);
            if (roleNameExists)
            {
                ModelState.AddModelError("Name", "Role name already exists.");
                return View(updatemodel);
            }

            // update the role name
            role.Name = updatemodel.Name;
            await _roleManager.UpdateAsync(role);

            return RedirectToAction(nameof(Index));
        }
    }
}
