using Hospital.Models.Diagnosis;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionEditModel
    {
        [MaxLength(5000)]
        public string? anamnesis { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string complaints { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string treatment { get; set; }

        [Required]
        public Conclusion conclusion { get; set; }

        public DateTime? nextVisitDate { get; set; }

        public DateTime? deathTime { get; set; }

        [Required]
        [MinLength(1)]
        public List<DiagnosisCreateModel> diagnoses { get; set; }
    }
}
