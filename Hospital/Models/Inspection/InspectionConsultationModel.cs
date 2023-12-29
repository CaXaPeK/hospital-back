using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionConsultationModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid inspectionId { get; set; }

        public SpecialityModel speciality { get; set; }

        public InspectionCommentModel rootComment { get; set; }

        public int commentsNumber { get; set; }
    }
}
