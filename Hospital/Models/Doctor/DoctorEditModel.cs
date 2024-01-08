using System.ComponentModel.DataAnnotations;
using Hospital.Models.General;
using Hospital.Validation;

namespace Hospital.Models.Doctor
{
    public class DoctorEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; set; }

        [BirthDate]
        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
            ErrorMessage = "PhoneNumber is not valid")]
        public string? Phone { get; set; }
    }
}
