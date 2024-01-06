using Hospital.Models.Inspection;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Consultation
{
    public class ConsultationCreateModel
    {
        [Required]
        public Guid SpecialityId { get; set; }

        [Required]
        public InspectionCommentCreateModel Comment { get; set; }
    }
}
