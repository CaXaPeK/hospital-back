using Hospital.Models.Inspection;
using Hospital.Models.Consultation;

namespace Hospital.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetYourSpecialityInspections(Guid doctorId, List<Guid> icdRoots, bool grouped, int page, int size);

        Task<ConsultationModel> GetConsultation(Guid id);
    }
}
