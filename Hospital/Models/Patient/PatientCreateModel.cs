using Hospital.Models.General;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Patient
{
    public class PatientCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }

        public DateTime? birthday { get; set; }

        [Required]
        public Gender gender { get; set; }
    }
}
