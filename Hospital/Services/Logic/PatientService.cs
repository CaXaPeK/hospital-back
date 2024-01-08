using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using System.ComponentModel.Design;

namespace Hospital.Services.Logic
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public PatientService(AppDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public async Task<Guid> CreatePatient(PatientCreateModel newPatient)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                Name = newPatient.Name,
                BirthDate = newPatient.Birthday,
                Gender = newPatient.Gender
            };

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();

            return patient.Id;
        }
    }
}
