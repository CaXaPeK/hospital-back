namespace Hospital.Models.Icd
{
    public class IcdRootsReportFiltersModel
    {
        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public List<string>? roots { get; set; }
    }
}
