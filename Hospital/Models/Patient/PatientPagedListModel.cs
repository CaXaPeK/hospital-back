using Hospital.Models.General;

namespace Hospital.Models.Patient
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patients { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
