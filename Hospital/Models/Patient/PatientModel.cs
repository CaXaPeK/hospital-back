using Hospital.Models.General;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Patient
{
    public class PatientModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
