using Hospital.Models.General;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Patient
{
    public class PatientCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
