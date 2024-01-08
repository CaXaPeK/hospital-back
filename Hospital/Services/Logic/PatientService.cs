using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Exceptions;
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

        public async Task<PatientModel> GetPatient(Guid id)
        {
            var patient = FindPatient(id);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found in database");
            }

            var patientCard = new PatientModel
            {
                Id = patient.Id,
                Name = patient.Name,
                CreateTime = patient.CreateTime,
                Birthday = patient.BirthDate,
                Gender = patient.Gender
            };

            return patientCard;
        }

        private Patient? FindPatient(Guid id)
        {
            return _dbContext.Patients
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
