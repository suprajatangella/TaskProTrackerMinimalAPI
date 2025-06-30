using System.ComponentModel.DataAnnotations;

namespace TaskProTracker.MinimalAPI.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Minimum length should be 6 characters")]
        public string Password { get; set; }

    }
}
