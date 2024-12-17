using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Mono.TextTemplating;
using System.Net;
using System.Net.Mail;
using UniqloMVC1.Enums;
using UniqloMVC1.Helpers;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Auths;

namespace UniqloMVC1.Controllers
{
    public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager, IOptions<SmtpOptions> opts) : Controller
    {

        readonly  SmtpOptions _smtpOpt = opts.Value;
        bool isAuthenticated => User.Identity?.IsAuthenticated ?? false;
        public IActionResult Register()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM vm)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User user = new User
            {
                Email = vm.Email,
                Fullname = vm.Fullname,
                UserName = vm.Username,
                ProfileImageUrl = "photo.jpg"
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded) {

                foreach (var error in result.Errors) {

                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!roleResult.Succeeded)
            {

                foreach (var error in roleResult.Errors)
                {

                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

           string token = await  _userManager.GenerateEmailConfirmationTokenAsync(user);



            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }


        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            //if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);



            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is wrong");
                return View();
            }



            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "Username or password is wrong");
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                return View();
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }


            //return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");

        }



        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));

        }

        public async Task<IActionResult> Test()
        {
            SmtpClient smtp = new();

            smtp.Host = _smtpOpt.Host;
            smtp.Port = _smtpOpt.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_smtpOpt.Username , _smtpOpt.Password);
            MailAddress from = new MailAddress(_smtpOpt.Username, "Uniqlo");
            MailAddress to = new("tagiyevaa013@gmail.com");
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = "Password reset link";
            msg.Body= "<p>Someone has requested a link to change your password. You can do this through the link below.<p><a>Change my password</a><p>If you didn't request this, please ignore this email.<p>";
            msg.IsBodyHtml = true;
            smtp.Send(msg);
            return Ok("Succes");

        } 
    }
}
