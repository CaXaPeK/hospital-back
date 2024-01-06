namespace Hospital.Models.Icd
{
    public class IcdRootsReportFiltersModel
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public List<string>? Roots { get; set; }
    }
}
