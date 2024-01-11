using Hospital.Models.General;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Doctor
{
    public class DoctorRegisterModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^(?=.*\\d).{6,}$",
            ErrorMessage = "Password must be at least 6 letters and have at least 1 digit")]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [BirthDate]
        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
            ErrorMessage = "Phone number must be in a format of \"+7 (xxx) xxx-xx-xx\"")]
        public string? Phone { get; set; }

        [Required]
        public Guid Speciality { get; set; }
    }
}
