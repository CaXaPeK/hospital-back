using Hospital.Models.Patient;

namespace Hospital.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel newPatient);

        Task<PatientModel> GetPatient(Guid id);
    }
}
