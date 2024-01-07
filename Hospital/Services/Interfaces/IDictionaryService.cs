using Hospital.Models.Speciality;

namespace Hospital.Services.Interfaces
{
    public interface IDictionaryService
    {
        Task<SpecialtiesPagedListModel> GetSpecialitiesList(string? name, int page, int size);
    }
}
