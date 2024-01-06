using Hospital.Models.General;

namespace Hospital.Models.Inspection
{
    public class InspectionPagedListModel
    {
        public List<InspectionPreviewModel>? Inspections { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
