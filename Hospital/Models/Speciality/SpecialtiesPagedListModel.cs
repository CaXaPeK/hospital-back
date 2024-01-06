using Hospital.Models.General;

namespace Hospital.Models.Speciality
{
    public class SpecialtiesPagedListModel
    {
        public List<SpecialityModel>? Specialities { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
