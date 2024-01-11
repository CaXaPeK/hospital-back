using Hospital.Models.Inspection;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Database.TableModels
{
    public class Inspection
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [InspectionDate]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Anamnesis { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Complaints { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Treatment { get; set; }

        [Required]
        public Conclusion Conclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid? BaseInspectionId { get; set; }

        public Inspection? BaseInspection { get; set; }

        public Guid? PreviousInspectionId { get; set; }

        public Inspection? PreviousInspection { get; set; }

        public Guid? NextInspectionId { get; set; }

        public Inspection? NextInspection { get; set; }

        [Required]
        public List<InspectionDiagnosis> Diagnoses { get; set; }

        public List<Consultation> Consultations { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; }
    }
}
