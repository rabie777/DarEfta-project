using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal.BL.Models;
using Portal.DAL.Extend;

namespace Portal.PL.Controllers
{


    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {



        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        public IActionResult Index()
        {
            var data = roleManager.Roles;
            return View(data);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {

            var result = await roleManager.CreateAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View();
        }




        public async Task<IActionResult> Update(string id)
        {
            var data = await roleManager.FindByIdAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(IdentityRole model)
        {

            var result = await roleManager.UpdateAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }



        public async Task<IActionResult> Delete(string id)
        {
            var data = await roleManager.FindByIdAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IdentityRole model)
        {

            var data = await roleManager.FindByIdAsync(model.Id);

            if (data != null)
            {
                var result = await roleManager.DeleteAsync(data);


                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }

            return View(model);
        }



        public async Task<IActionResult> AddOrRemoveUsers(string RoleId)
        {

            ViewBag.roleId = RoleId;

            var role = await roleManager.FindByIdAsync(RoleId);

            var model = new List<UserInRoleVM>();

            foreach (var user in userManager.Users)
            {
                var userInRole = new UserInRoleVM()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }

                model.Add(userInRole);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleVM> model, string RoleId)
        {

            var role = await roleManager.FindByIdAsync(RoleId);

            for (int i = 0; i < model.Count; i++)
            {

                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {

                    result = await userManager.AddToRoleAsync(user, role.Name);

                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

            }

            return RedirectToAction("Update", new { id = RoleId });
        }

    }
}
