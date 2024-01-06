using Hospital.Models.General;

namespace Hospital.Models.Icd
{
    public class IcdRootsReportRecordModel
    {
        public string? PatientName { get; set; }

        public DateTime PatientBirthdate { get; set; }

        public Gender Gender { get; set; }

        public int? VisitsByRoot { get; set; }
    }
}
