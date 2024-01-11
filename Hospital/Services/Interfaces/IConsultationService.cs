using Hospital.Models.Inspection;

namespace Hospital.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetYourSpecialityInspections(Guid doctorId, List<Guid> icdRoots, bool grouped, int page, int size);
    }
}
