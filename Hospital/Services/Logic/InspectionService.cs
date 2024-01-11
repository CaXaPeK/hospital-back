using Hospital.Database;
using Hospital.Models.Inspection;
using Hospital.Services.Interfaces;
using Hospital.Exceptions;
using Hospital.Models.Doctor;
using Hospital.Models.Patient;
using Microsoft.EntityFrameworkCore;
using Hospital.Models.Diagnosis;
using Hospital.Models.Speciality;
using Hospital.Database.TableModels;
using System.Security.Authentication;

namespace Hospital.Services.Logic
{
    public class InspectionService : IInspectionService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDictionaryService _dictionaryService;

        public InspectionService(AppDbContext dbContext, IDictionaryService dictionaryService)
        {
            _dbContext = dbContext;
            _dictionaryService = dictionaryService;
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
                .FirstOrDefault(i => i.Id == inspectionId);

            if (inspection == null)
            {
                throw new NotFoundException($"Inspection with ID {inspectionId} not found in the database");
            }

            var patient = inspection.Patient;

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

            var doctor = inspection.Doctor;

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
                    Code = diagnosis.IcdDiagnosis.MkbCode,
                    Name = diagnosis.IcdDiagnosis.MkbName,
                    Description = diagnosis.Description,
                    Type = diagnosis.Type
                })
                .ToList();

            var consultationModels = inspection.Consultations
                .Select(consultation => new InspectionConsultationModel
                {
                    Id = consultation.Id,
                    CreateTime = consultation.CreateTime,
                    InspectionId = consultation.InspectionId,
                    Speciality = new SpecialityModel
                    {
                        Id = consultation.Speciality.Id,
                        CreateTime = consultation.Speciality.CreateDate,
                        Name = consultation.Speciality.Name
                    },
                    RootComment = new InspectionCommentModel
                    {
                        Id = consultation.Comments.First(c => c.ParentId == null).Id,
                        CreateTime = consultation.Comments.First(c => c.ParentId == null).CreateTime,
                        ParentId = null,
                        Content = consultation.Comments.First(c => c.ParentId == null).Content,
                        Author = doctorModel,
                        ModifyTime = consultation.Comments.First(c => c.ParentId == null).ModifiedDate
                    },
                    CommentsNumber = consultation.Comments.Count()
                }).ToList();

            var inspectionModel = new InspectionModel
            {
                Id = inspection.Id,
                CreateTime = inspection.CreateTime,
                Date = inspection.Date,
                Anamnesis = inspection.Anamnesis,
                Complaints = inspection.Complaints,
                Treatment = inspection.Treatment,
                Conclusion = inspection.Conclusion,
                NextVisitDate = inspection.NextVisitDate,
                DeathDate = inspection.DeathDate,
                BaseInspectionId = inspection.BaseInspectionId,
                PreviousInspectionId = inspection.PreviousInspectionId,
                Patient = patientModel,
                Doctor = doctorModel,
                Diagnoses = diagnosisModels,
                Consultations = consultationModels
            };

            return inspectionModel;
        }

        public async Task EditInspection(Guid inspectionId, InspectionEditModel editedInspection, Guid doctorId)
        {
            var inspection = _dbContext.Inspections
                .Include(i => i.Diagnoses)
                .FirstOrDefault(i => i.Id == inspectionId);

            if (inspection == null)
            {
                throw new NotFoundException($"Inspection with ID {inspectionId} not found in the database");
            }

            if (inspection.DoctorId != doctorId)
            {
                throw new MethodAccessException($"Can't edit an inspection created by a different doctor");
            }

            ValidateEditInspection(editedInspection, inspection.Date);

            var newDiagnoses = new List<InspectionDiagnosis>();

            foreach (var diagnosis in editedInspection.Diagnoses)
            {
                var diagnosisEntity = new InspectionDiagnosis
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.UtcNow,
                    IcdDiagnosisId = diagnosis.IcdDiagnosisId,
                    Description = diagnosis.Description,
                    Type = diagnosis.Type
                };

                newDiagnoses.Add(diagnosisEntity);
                _dbContext.InspectionDiagnoses.Add(diagnosisEntity);
            }

            _dbContext.InspectionDiagnoses.RemoveRange(inspection.Diagnoses);

            inspection.Anamnesis = editedInspection.Anamnesis;
            inspection.Complaints = editedInspection.Complaints;
            inspection.Treatment = editedInspection.Treatment;
            inspection.Conclusion = editedInspection.Conclusion;
            inspection.NextVisitDate = editedInspection.NextVisitDate;
            inspection.DeathDate = editedInspection.DeathDate;
            inspection.Diagnoses = newDiagnoses;

            await _dbContext.SaveChangesAsync();
        }

        public void ValidateCreateInspection(InspectionCreateModel newInspection, Patient patient)
        {
            ValidateDiagnoses(newInspection.Diagnoses);

            ValidateDates(newInspection.Conclusion, newInspection.NextVisitDate, newInspection.DeathDate, newInspection.Date);

            if (HasDeathInspection(patient))
            {
                throw new InvalidCredentialException($"Patient with ID {patient.Id} already has an inspection with a Death conclusion. Can't create new inspections");
            }

            if (newInspection.PreviousInspectionId != null)
            {
                var previousInspection = _dbContext.Inspections.FirstOrDefault(i => i.Id == newInspection.PreviousInspectionId.Value);

                if (previousInspection == null)
                {
                    throw new NotFoundException($"Previous inspection with ID {newInspection.PreviousInspectionId.Value} not found in the database");
                }

                if (previousInspection.Patient != patient)
                {
                    throw new InvalidCredentialException($"Previous inspection is not this patient's inspection");
                }

                if (_dbContext.Inspections.First(i => i == previousInspection).NextInspectionId != null)
                {
                    throw new InvalidCredentialException($"Child inspection already exists for inspection {previousInspection.Id}");
                }

                if (newInspection.Date < previousInspection.Date)
                {
                    throw new InvalidCredentialException("Inspection can't be done earlier than its previous inspection");
                }
            }
        }

        private void ValidateEditInspection(InspectionEditModel editedInspection, DateTime visitDate)
        {
            ValidateDiagnoses(editedInspection.Diagnoses);

            ValidateDates(editedInspection.Conclusion, editedInspection.NextVisitDate, editedInspection.DeathDate, visitDate);
        }

        private void ValidateDiagnoses(List<DiagnosisCreateModel> diagnoses)
        {
            if (MainDiagnosesCount(diagnoses) != 1)
            {
                throw new InvalidCredentialException("Inspection must have one main-type diagnosis");
            }

            foreach (var diagnosis in diagnoses)
            {
                if (!_dictionaryService.DiagnosisExists(diagnosis.IcdDiagnosisId))
                {
                    throw new NotFoundException($"Diagnosis with ID {diagnosis.IcdDiagnosisId} not found in the database");
                }
            }
        }

        private void ValidateDates(Conclusion conclusion, DateTime? nextVisitDate, DateTime? deathDate, DateTime visitDate)
        {
            switch (conclusion)
            {
                case Conclusion.Disease:
                    if (nextVisitDate == null)
                    {
                        throw new InvalidCredentialException("Inspection with a Disease conclusion must contain the date of the next visit");
                    }
                    if (deathDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Disease conclusion mustn't contain the date of the patient's death");
                    }
                    break;
                case Conclusion.Death:
                    if (nextVisitDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Death conclusion mustn't contain the date of the next visit");
                    }
                    if (deathDate == null)
                    {
                        throw new InvalidCredentialException("Inspection with a Death conclusion must contain the date of the patient's death");
                    }
                    break;
                case Conclusion.Recovery:
                    if (nextVisitDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Recovery conclusion mustn't contain the date of the next visit");
                    }
                    if (deathDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Recovery conclusion mustn't contain the date of the patient's death");
                    }
                    break;
                default:
                    break;
            }

            if (nextVisitDate != null)
            {
                if (nextVisitDate < visitDate)
                {
                    throw new InvalidCredentialException("Next visit's date can't be earlier than this inspection's date");
                }
            }

            if (deathDate != null)
            {
                if (deathDate > visitDate)
                {
                    throw new InvalidCredentialException("Death date can't be later than this inspection's date");
                }
            }
        }

        public int MainDiagnosesCount(List<DiagnosisCreateModel> diagnoses)
        {
            int count = 0;

            foreach (var diagnosis in diagnoses)
            {
                if (diagnosis.Type == DiagnosisType.Main)
                {
                    count++;
                }
            }

            return count;
        }

        private bool HasDeathInspection(Patient patient)
        {
            return patient.Inspections
                .Any(i => i.Conclusion == Conclusion.Death);
        }
    }
}