using Hospital.Models.Diagnosis;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class InspectionDiagnosis
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public Guid IcdDiagnosisId { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }

        [Required]
        public DiagnosisType Type { get; set; }
    }
}
