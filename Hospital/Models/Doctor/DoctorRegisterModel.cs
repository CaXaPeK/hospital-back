using Hospital.Models.General;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Doctor
{
    public class DoctorRegisterModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

        [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
            ErrorMessage = "PhoneNumber is not valid")]
        public string? phone { get; set; }

        [Required]
        public Guid specialty { get; set; }
    }
}
