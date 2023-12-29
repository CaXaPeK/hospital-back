using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Consultation
{
    public class ConsultationCreateModel
    {
        [Required]
        public Guid specialityId { get; set; }

        [Required]
        public InspectionCommentCreateModel comment { get; set; }
    }
}
