using Hospital.Models.Patient;

namespace Hospital.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel newPatient);
    }
}
