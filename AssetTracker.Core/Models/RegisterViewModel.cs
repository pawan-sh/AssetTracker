using System.ComponentModel.DataAnnotations;
namespace AssetTracker.Web.Models
{
    public class RegisterViewModel
    {

        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty; 
    }
}
