using Hospital.Models.General;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Doctor
{
    public class DoctorModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [BirthDate]
        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }
    }
}
