using Hospital.Models.Doctor;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionCommentModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public Guid? ParentId { get; set; }

        public string? Content { get; set; }

        public DoctorModel Author { get; set; }

        public DateTime ModifyTime { get; set; }
    }
}
