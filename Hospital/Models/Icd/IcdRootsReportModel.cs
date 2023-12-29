namespace Hospital.Models.Icd
{
    public class IcdRootsReportModel
    {
        public IcdRootsReportFiltersModel filters { get; set; }

        public List<IcdRootsReportRecordModel>? records { get; set; }

        public int? summaryByRoot { get; set; }
    }
}
