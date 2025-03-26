using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SkiSurf.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"(?=^.{8,20}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{:;'?/>,<])(?!.*\s).*$",
         ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 non-alphanumeric, and be 8-20 characters long.")]

        public string Password { get; set; }
    }
}
