using Hospital.Models.Diagnosis;
using Hospital.Models.Doctor;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public DateTime date { get; set; }

        public string? anamnesis { get; set; }

        public string? complaints { get; set; }

        public string? treatment { get; set; }

        public Conclusion consclusion { get; set; }

        public DateTime? nextVisitDate { get; set; }

        public DateTime? deathDate { get; set; }

        public Guid? baseInspectionId { get; set; }

        public Guid? previousInspectionId { get; set; }

        public PatientModel patient { get; set; }

        public DoctorModel doctor { get; set; }

        public List<DiagnosisModel>? diagnoses { get; set; }

        public List<InspectionConsultationModel>? consultations { get; set; }
    }
}
