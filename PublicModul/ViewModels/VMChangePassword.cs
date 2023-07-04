using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PublicModul.ViewModels
{
    public class VMChangePassword
    {
        [DisplayName("User name")]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Passsword is required.")]
        [DisplayName("Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Passsword is required.")]
        [DisplayName("New Password")]
        [Compare("Password")]
        public string NewPassword { get; set; }
    }
}

