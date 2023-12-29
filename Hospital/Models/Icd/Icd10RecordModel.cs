using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.Icd
{
    public class Icd10RecordModel
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public DateTime createTime { get; set; }

        public string? code { get; set; }

        public string? name { get; set; }
    }
}
