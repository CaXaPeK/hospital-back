using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Inspection
{
    public class InspectionCommentCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
