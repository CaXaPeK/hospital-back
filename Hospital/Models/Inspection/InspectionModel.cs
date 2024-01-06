using Hospital.Models.Diagnosis;
using Hospital.Models.Doctor;
using Hospital.Models.Patient;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime Date { get; set; }

        public string? Anamnesis { get; set; }

        public string? Complaints { get; set; }

        public string? Treatment { get; set; }

        public Conclusion Consclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid? BaseInspectionId { get; set; }

        public Guid? PreviousInspectionId { get; set; }

        public PatientModel Patient { get; set; }

        public DoctorModel Doctor { get; set; }

        public List<DiagnosisModel>? Diagnoses { get; set; }

        public List<InspectionConsultationModel>? Consultations { get; set; }
    }
}
