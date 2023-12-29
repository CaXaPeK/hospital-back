using Hospital.Models.General;

namespace Hospital.Models.Icd
{
    public class Icd10SearchModel
    {
        public List<Icd10RecordModel>? records { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
