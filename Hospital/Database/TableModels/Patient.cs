using Hospital.Models.General;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class Patient
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [BirthDate]
        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public List<Inspection> Inspections { get; set; }
    }
}
