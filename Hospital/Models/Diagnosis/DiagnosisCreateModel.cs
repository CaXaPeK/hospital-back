using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Diagnosis
{
    public class DiagnosisCreateModel
    {
        [Required]
        public Guid IcdDiagnosisId { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }

        [Required]
        public DiagnosisType Type { get; set; }
    }
}
