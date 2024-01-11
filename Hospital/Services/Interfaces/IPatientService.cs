using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Database.TableModels;

namespace Hospital.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel newPatient);

        Task<PatientPagedListModel> GetPatientList(
            string? name,
            List<Conclusion>? conclusions,
            PatientSorting? sorting,
            bool scheduledVisits,
            bool onlyMine,
            int page,
            int size,
            Guid doctorId
            );

        Task<Guid> CreateInspection(InspectionCreateModel newInspection, Guid patientId, Guid authorId);

        Task<InspectionPagedListModel> GetInspectionList(Guid patientId, List<Guid> icdRoots, bool grouped, int page, int size);

        Task<PatientModel> GetPatient(Guid id);

        Task<List<InspectionShortModel>> GetInspectionsWithoutChildren(Guid patientId, string? request);
    }
}
