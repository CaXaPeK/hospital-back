using Hospital.Models.Doctor;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionCommentModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public Guid? parentId { get; set; }

        public string? content { get; set; }

        public DoctorModel author { get; set; }

        public DateTime modifyTime { get; set; }
    }
}
