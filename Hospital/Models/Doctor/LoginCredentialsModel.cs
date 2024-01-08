using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Doctor
{
    public class LoginCredentialsModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
