using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal.DAL.Extend;

namespace Portal.PL.Controllers
{


    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {


        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }


        public IActionResult Index()
        {
            var data = userManager.Users;
            return View(data);
        }


        public async Task<IActionResult> Update(string id)
        {
            var data = await userManager.FindByIdAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUser model)
        {

            var result = await userManager.UpdateAsync(model);

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
            var data = await userManager.FindByIdAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser model)
        {

            var data = await userManager.FindByIdAsync(model.Id);

            if (data != null)
            {
                var result = await userManager.DeleteAsync(data);


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
    }
}
