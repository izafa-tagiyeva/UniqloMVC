using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniqloMVC1.Models;

public class ResetPasswordService(UserManager<User> _userManager)
{
   
    private readonly Dictionary<string, (string Code, DateTime Expiration)> _resetCodes = new(); // Create volatile code!!!

    
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

 
    public string GenerateAndSaveCode(string email)
    {
        string code = new Random().Next(100000, 999999).ToString(); 
        _resetCodes[email] = (code, DateTime.UtcNow.AddMinutes(25)); 
        return code;
    }

    
    public bool ValidateCode(string email, string code)
    {
        if (_resetCodes.TryGetValue(email, out var entry))
        {
            if (entry.Code == code && DateTime.UtcNow <= entry.Expiration) 
            {
                _resetCodes.Remove(email); 
                return true;
            }
        }
        return false;
    }

   
    public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
    {
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user); 
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword); 
        return result.Succeeded;
    }
}


