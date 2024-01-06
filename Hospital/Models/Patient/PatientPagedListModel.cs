using Hospital.Models.General;

namespace Hospital.Models.Patient
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? Patients { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
