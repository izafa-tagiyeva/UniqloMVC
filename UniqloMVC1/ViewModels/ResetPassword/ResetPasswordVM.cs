using System.ComponentModel.DataAnnotations;

namespace UniqloMVC1.ViewModels.ResetPassword
{
    public class ResetPasswordVM
    {
        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
