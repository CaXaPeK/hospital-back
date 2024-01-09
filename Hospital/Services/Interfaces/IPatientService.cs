using Hospital.Models.Inspection;
using Hospital.Models.Patient;

namespace Hospital.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel newPatient);

        Task<Guid> CreateInspection(InspectionCreateModel newInspection, Guid patientId, Guid authorId);

        Task<PatientModel> GetPatient(Guid id);
    }
}
