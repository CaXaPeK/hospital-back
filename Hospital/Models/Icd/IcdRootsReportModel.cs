namespace Hospital.Models.Icd
{
    public class IcdRootsReportModel
    {
        public IcdRootsReportFiltersModel Filters { get; set; }

        public List<IcdRootsReportRecordModel>? Records { get; set; }

        public Dictionary<string, int>? SummaryByRoot { get; set; }
    }
}
