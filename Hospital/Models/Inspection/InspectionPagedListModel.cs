using Hospital.Models.General;

namespace Hospital.Models.Inspection
{
    public class InspectionPagedListModel
    {
        public List<InspectionPreviewModel>? inspections { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
