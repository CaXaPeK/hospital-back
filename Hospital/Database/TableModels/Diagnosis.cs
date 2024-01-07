using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Hospital.Database.TableModels
{
    public class Diagnosis
    {
        [Required]
        public bool Actual { get; set; }

        public int? AddlCode { get; set; }

        public DateOnly? Date { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public Guid? ParentId { get; set; }

        [Required]
        [MinLength(1)]
        public string MkbCode { get; set; }

        [Required]
        [MinLength(1)]
        public string MkbName { get; set; }

        [Required]
        [MinLength(1)]
        public string RecCode { get; set; }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string RootCode { get; set; }

        public Diagnosis(bool actual, int? addlCode, DateOnly? date, DateTime createDate, Guid? parentId, string mkbCode, string mkbName, string recCode, Guid id, string rootCode)
        {
            Actual = actual;
            AddlCode = addlCode;
            Date = date;
            CreateDate = createDate;
            ParentId = parentId;
            MkbCode = mkbCode;
            MkbName = mkbName;
            RecCode = recCode;
            Id = id;
            RootCode = rootCode;
        }
    }
}
