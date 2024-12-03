using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Auths;

namespace UniqloMVC1.Controllers
{
    public class AccountController(UserManager<User> _userManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM vm)
        {
            if (!ModelState.IsValid) return View();
            User user = new User
            {
                Email = vm.Email,
                Fullname = vm.Fullname,
                UserName = vm.Username,
                ProfileImageUrl="photo.jpg"
            };

             var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded) {

                foreach (var error in result.Errors) {

                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return View();
        }


    }
}
