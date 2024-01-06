using Hospital.Models.General;

namespace Hospital.Models.Icd
{
    public class Icd10SearchModel
    {
        public List<Icd10RecordModel>? Records { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
