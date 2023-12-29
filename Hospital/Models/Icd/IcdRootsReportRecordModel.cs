using Hospital.Models.General;

namespace Hospital.Models.Icd
{
    public class IcdRootsReportRecordModel
    {
        public string? patientName { get; set; }

        public DateTime patientBirthdate { get; set; }

        public Gender gender { get; set; }

        public int? visitsByRoot { get; set; }
    }
}
