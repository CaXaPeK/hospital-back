﻿using Hospital.Models.Icd;
using Hospital.Models.Speciality;

namespace Hospital.Services.Interfaces
{
    public interface IDictionaryService
    {
        Task<SpecialtiesPagedListModel> GetSpecialitiesList(string? name, int page, int size);

        Task<List<Icd10RecordModel>> GetRootDiagnoses();

        Task<Icd10SearchModel> GetDiagnosesList(string? request, int page, int size);

        bool SpecialityExists(Guid id);

        bool DiagnosisExists(Guid id);
    }
}
