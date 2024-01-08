using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class Speciality
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public Speciality(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CreateDate = DateTime.UtcNow;
        }
    }
}
