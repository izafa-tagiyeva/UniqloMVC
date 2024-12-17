using Microsoft.AspNetCore.Mvc;
using UniqloMVC1.Services.Abstractions;
using UniqloMVC1.ViewModels.Auths;
using UniqloMVC1.ViewModels.ResetPassword;
public class ResetPasswordController(ResetPasswordService _resetPasswordService, IEmailService _emailService) : Controller
{
    [HttpGet("SendCode")]
    public IActionResult SendCode()
    {
        return View(); 
    }
    [HttpPost("SendCode")]
    public async Task<IActionResult> SendCode(string email)
    {
        var user = await _resetPasswordService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        string code = _resetPasswordService.GenerateAndSaveCode(email);
        _emailService.SendEmailConfirmation(email, user.Fullname, code);
        return Ok("Verification code sent.");
    }

      [HttpGet("ResetPassword")]
    public IActionResult ResetPassword()
    {
        return View(); 
    }
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM vm, string code)
    {

        UserRegisterVM new1 = new UserRegisterVM
        { 
            Email = vm.Email,
            Password = vm.NewPassword,

        };
        var isValid = _resetPasswordService.ValidateCode(new1.Email, code);

        if (!isValid)
        {
            return BadRequest("The code is invalid or expired.");
        }
        var user = await _resetPasswordService.GetUserByEmailAsync(new1.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        await _resetPasswordService.UpdatePasswordAsync(user,new1.Password);
        return Ok("Password changed successfully.");
        return RedirectToAction("Account", "Login");
        // Account-Login place
    }
}
