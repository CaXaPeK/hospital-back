using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Doctor
{
    public class LoginCredentialsModel
    {
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}
