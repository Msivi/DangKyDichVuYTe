using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Required(ErrorMessage = "Confirm Password is required")]
        public string? ConfirmNewPassword { get; set; }
    }
}
