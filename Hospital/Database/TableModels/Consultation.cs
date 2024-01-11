using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class Consultation
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public Guid InspectionId { get; set; }

        public Inspection Inspection { get; set; }

        [Required]
        public Guid SpecialityId { get; set; }

        public Speciality Speciality { get; set; }

        [Required]
        public List<Comment> Comments { get; set; }
    }
}
