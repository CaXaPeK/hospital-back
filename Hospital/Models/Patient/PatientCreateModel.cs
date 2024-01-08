using Hospital.Models.General;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Patient
{
    public class PatientCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; set; }

        [BirthDate]
        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
