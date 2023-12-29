using Hospital.Models.General;

namespace Hospital.Models.Speciality
{
    public class SpecialtiesPagedListModel
    {
        public List<SpecialityModel>? specialities { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
