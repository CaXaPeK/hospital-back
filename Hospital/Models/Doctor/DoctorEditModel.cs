using System.ComponentModel.DataAnnotations;
using Hospital.Models.General;

namespace Hospital.Models.Doctor
{
    public class DoctorEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }

        [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
            ErrorMessage = "PhoneNumber is not valid")]
        public string? phone { get; set; }
    }
}
