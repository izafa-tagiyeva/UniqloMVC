using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using UniqloMVC1.Enums;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Auths;

namespace UniqloMVC1.Controllers
{
    public class AccountController(UserManager<User> _userManager,SignInManager<User> _signInManager) : Controller
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
            var roleResult= await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!roleResult.Succeeded)
            {

                foreach (var error in roleResult.Errors)
                {

                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View();
        }




        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm,string returnUrl)
        {
            if(!ModelState.IsValid) return View();
            User user = null;
            if (vm.UsernameOrEmail.Contains('@') )
             {
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
             }else 
            { 
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail); 
            }


            if (user is null) 
            { 
                ModelState.AddModelError("", "Username or password is wrong");
                return View();  
            }



           var result= await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
            if (!result.Succeeded) 
            {
                if (result.IsNotAllowed) 
                {
                    ModelState.AddModelError("", "Username or password is wrong");

                }
                if (result.IsLockedOut) 
                 ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return View();
            }

            if (string.IsNullOrEmpty(returnUrl)) { 
             return RedirectToAction("Index","Home");
            }

            return LocalRedirect(returnUrl);
            
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));

        }



    }
}
