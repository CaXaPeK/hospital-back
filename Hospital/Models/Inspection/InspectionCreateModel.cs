﻿using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Hospital.Models.Consultation;
using Hospital.Models.Diagnosis;
using Hospital.Validation;

namespace Hospital.Models.Inspection
{
    public class InspectionCreateModel
    {
        [Required]
        [InspectionDate]
        public DateTime Date { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Anamnesis { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Complaints { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Treatment { get; set; }

        [Required]
        public Conclusion Conclusion { get; set; }

        [NextVisitDate]
        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid? PreviousInspectionId { get; set; }

        [Required]
        [MinLength(1)]
        public List<DiagnosisCreateModel> Diagnoses { get; set; }

        public List<ConsultationCreateModel>? Consultations { get; set; }
    }
}
