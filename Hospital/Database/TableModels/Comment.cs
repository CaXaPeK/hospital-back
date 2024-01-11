using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class Comment
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [ModifiedDate]
        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        public Doctor Author { get; set; }

        public Guid? ParentId { get; set; }

        public Comment? Parent { get; set; }

        public List<Comment> ChildComments { get; set; }

        [Required]
        public Guid ConsultationId { get; set; }

        public Consultation Consultation { get; set; }
    }
}
