using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Consultation
{
    public class ConsultationModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid inspectionId { get; set; }

        public SpecialityModel speciality { get; set; }

        public List<CommentModel>? comments { get; set; }
    }
}
