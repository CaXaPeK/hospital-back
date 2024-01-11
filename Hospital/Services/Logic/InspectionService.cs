using Hospital.Database;
using Hospital.Models.Inspection;
using Hospital.Services.Interfaces;
using Hospital.Exceptions;
using Hospital.Models.Doctor;
using Hospital.Models.Patient;
using Microsoft.EntityFrameworkCore;
using Hospital.Models.Diagnosis;
using Hospital.Database.TableModels;

namespace Hospital.Services.Logic
{
    public class InspectionService : IInspectionService
    {
        private readonly AppDbContext _dbContext;

        public InspectionService(AppDbContext dbContext, IDictionaryService dictionaryService)
        {
            _dbContext = dbContext;
        }

        public async Task<InspectionModel> GetFullInspection(Guid inspectionId)
        {
            var inspection = _dbContext.Inspections
                .Include(i => i.Patient)
                .Include(i => i.Doctor)
                .Include(i => i.Diagnoses)
                    .ThenInclude(d => d.IcdDiagnosis)
                .Include(i => i.Consultations)
                    .ThenInclude(c => c.Speciality)
                .Include(i => i.Consultations)
                    .ThenInclude(c => c.Comments)
                        .ThenInclude(c => c.Author)
                .FirstOrDefault(i => i.Id == inspectionId);

            if (inspection == null)
            {
                throw new NotFoundException($"Inspection with ID {inspectionId} not found in the database");
            }

            var patient = _dbContext.Patients
                .FirstOrDefault(p => p.Id == inspection.PatientId);

            if (patient == null)
            {
                throw new NotFoundException($"Inspection's patient with ID {inspection.PatientId} not found in the database");
            }

            var patientModel = new PatientModel
            {
                Id = patient.Id,
                Name = patient.Name,
                CreateTime = patient.CreateTime,
                Birthday = patient.BirthDate,
                Gender = patient.Gender
            };

            var doctor = _dbContext.Doctors
                .FirstOrDefault(d => d.Id == inspection.DoctorId);

            if (doctor == null)
            {
                throw new NotFoundException($"Inspection's doctor with ID {inspection.DoctorId} not found in the database");
            }

            var doctorModel = new DoctorModel
            {
                Id = doctor.Id,
                CreateTime = doctor.CreateTime,
                Name = doctor.Name,
                Birthday = doctor.BirthDate,
                Gender = doctor.Gender,
                Email = doctor.Email,
                Phone = doctor.Phone
            };

            var diagnosisModels = inspection.Diagnoses
                .Select(diagnosis => new DiagnosisModel
                {
                    Id = diagnosis.Id,
                    CreateTime = diagnosis.CreateTime,
                    Code = _dbContext.Diagnoses.First(d => d.Id == diagnosis.IcdDiagnosisId).MkbCode,
                    Name = _dbContext.Diagnoses.First(d => d.Id == diagnosis.IcdDiagnosisId).MkbName,
                    Description = diagnosis.Description,
                    Type = DiagnosisType.Main
                })
                .ToList();

            /*var consultationModels = inspection.Consultations
                .Select(consultation => new InspectionConsultationModel
                {
                    Id = consultation.Id,
                    CreateTime = consultation.CreateTime,
                    InspectionId = consultation.InspectionId,
                    Speciality = 
                }).ToList();*/

            var inspectionModel = new InspectionModel
            {

            };

            return inspectionModel;
        }
        public static DiagnosisModel CreateDiagnosisModel(InspectionDiagnosis diagnosis, Diagnosis icdDiagnosis)
        {
            return new DiagnosisModel
            {
                Id = diagnosis.Id,
                CreateTime = diagnosis.CreateTime,
                Code = icdDiagnosis.MkbCode,
                Name = icdDiagnosis.MkbName,
                Description = diagnosis.Description,
                Type = DiagnosisType.Main
            };
        }
    }
}
