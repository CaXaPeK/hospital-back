using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Comment
{
    public class CommentCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string content { get; set; }

        public Guid parentId { get; set; }
    }
}
