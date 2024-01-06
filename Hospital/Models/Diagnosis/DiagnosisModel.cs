using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Diagnosis
{
    public class DiagnosisModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Code { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public DiagnosisType Type { get; set; }
    }
}
