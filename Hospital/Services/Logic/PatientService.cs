using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Exceptions;
using Hospital.Models.Diagnosis;
using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace Hospital.Services.Logic
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDictionaryService _dictionaryService;
        private readonly ITokenService _tokenService;

        public PatientService(AppDbContext dbContext, IDictionaryService dictionaryService, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _dictionaryService = dictionaryService;
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
                Gender = newPatient.Gender,
                Inspections = new List<Inspection>()
            };

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();

            return patient.Id;
        }

        public async Task<Guid> CreateInspection(InspectionCreateModel newInspection, Guid patientId, Guid authorId)
        {
            var patient = FindPatient(patientId);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found in the database");
            }

            ValidateInspection(newInspection, patient);

            var newInspectionId = Guid.NewGuid();

            var diagnoses = new List<InspectionDiagnosis>();

            foreach (var diagnosis in newInspection.Diagnoses)
            {
                var diagnosisEntity = new InspectionDiagnosis
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.UtcNow,
                    IcdDiagnosisId = diagnosis.IcdDiagnosisId,
                    Description = diagnosis.Description,
                    Type = diagnosis.Type
                };

                diagnoses.Add(diagnosisEntity);
                _dbContext.InspectionDiagnoses.Add(diagnosisEntity);
            }

            var consultations = new List<Consultation>();

            foreach (var consultation in newInspection.Consultations)
            {
                var newConsultationId = Guid.NewGuid();

                var newComment = new Comment
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.UtcNow,
                    Content = consultation.Comment.Content,
                    AuthorId = authorId,
                    ConsultationId = newConsultationId
                };

                var consultationEntity = new Consultation
                {
                    Id = newConsultationId,
                    CreateTime = DateTime.UtcNow,
                    InspectionId = newInspectionId,
                    SpecialityId = consultation.SpecialityId,
                    Comments = new List<Comment> { newComment }
                };

                consultations.Add(consultationEntity);
                _dbContext.Consultations.Add(consultationEntity);
                _dbContext.Comments.Add(newComment);
            }

            var baseInspectionId = new Guid();

            if (newInspection.PreviousInspectionId == null)
            {
                baseInspectionId = newInspectionId;
            }
            else
            {
                var previousInspection = FindInspection(newInspection.PreviousInspectionId.Value);

                baseInspectionId = previousInspection.BaseInspectionId;
            }

            var inspection = new Inspection
            {
                Id = newInspectionId,
                CreateTime = DateTime.UtcNow,
                Date = newInspection.Date,
                Anamnesis = newInspection.Anamnesis,
                Complaints = newInspection.Complaints,
                Treatment = newInspection.Treatment,
                Conclusion = newInspection.Conclusion,
                NextVisitDate = newInspection.NextVisitDate,
                DeathDate = newInspection.DeathDate,
                BaseInspectionId = baseInspectionId,
                PreviousInspectionId = newInspection.PreviousInspectionId,
                Diagnoses = diagnoses,
                Consultations = consultations,
                PatientId = patientId,
                AuthorId = authorId
            };

            await _dbContext.Inspections.AddAsync(inspection);
            patient.Inspections.Add(inspection);
            await _dbContext.SaveChangesAsync();

            return inspection.Id;
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

        private void ValidateInspection(InspectionCreateModel newInspection, Patient patient)
        { 
            if (HasDeathInspection(patient))
            {
                throw new MethodAccessException($"Patient with ID {patient.Id} already has an inspection with a Death conclusion. Can't create new inspections");
            }

            if (newInspection.PreviousInspectionId != null)
            {
                var previousInspection = FindInspection(newInspection.PreviousInspectionId.Value);

                if (previousInspection == null)
                {
                    throw new NotFoundException($"Previous inspection with ID {newInspection.PreviousInspectionId.Value} not found in the database");
                }

                if (PreviousInspectionAlreadyHasChild(newInspection.PreviousInspectionId.Value, patient))
                {
                    throw new InvalidCredentialException($"Child inspection already exists for inspection {previousInspection.Id}");
                }

                if (newInspection.Date < previousInspection.Date)
                {
                    throw new InvalidCredentialException("Inspection can't be done earlier than its previous inspection");
                }
            }

            if (MainDiagnosesCount(newInspection.Diagnoses) != 1)
            {
                throw new InvalidCredentialException("Inspection must have one main-type diagnosis");
            }

            foreach (var diagnosis in newInspection.Diagnoses)
            {
                if (!_dictionaryService.DiagnosisExists(diagnosis.IcdDiagnosisId))
                {
                    throw new NotFoundException($"Diagnosis with ID {diagnosis.IcdDiagnosisId} not found in the database");
                }
            }

            foreach (var consultation in newInspection.Consultations)
            {
                if (!_dictionaryService.SpecialityExists(consultation.SpecialityId))
                {
                    throw new NotFoundException($"Speciality with ID {consultation.SpecialityId} not found in the database");
                }

                int sameSpecialityCount = 0;

                foreach (var consultationForCheck in newInspection.Consultations)
                {
                    if (consultation.SpecialityId == consultationForCheck.SpecialityId)
                    {
                        sameSpecialityCount++;
                    }
                }

                if (sameSpecialityCount > 1)
                {
                    throw new InvalidCredentialException("All doctor specialities of an inspection must be unique");
                }
            }

            switch (newInspection.Conclusion)
            {
                case Conclusion.Disease:
                    if (newInspection.NextVisitDate == null)
                    {
                        throw new InvalidCredentialException("Inspection with a Disease conclusion must contain the date of the next visit");
                    }
                    break;
                case Conclusion.Death:
                    if (newInspection.NextVisitDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Death conclusion mustn't contain the date of the next visit");
                    }
                    if (newInspection.DeathDate == null)
                    {
                        throw new InvalidCredentialException("Inspection with a Death conclusion must contain the date of the patient's death");
                    }
                    break;
                case Conclusion.Recovery:
                    if (newInspection.NextVisitDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Recovery conclusion mustn't contain the date of the next visit");
                    }
                    if (newInspection.DeathDate != null)
                    {
                        throw new InvalidCredentialException("Inspection with a Recovery conclusion mustn't contain the date of the patient's death");
                    }
                    break;
                default:
                    break;
            }
        }

        private bool PreviousInspectionAlreadyHasChild(Guid previousInspectionId, Patient patient)
        {
            return patient.Inspections
                .Any(x => x.PreviousInspectionId == previousInspectionId);
        }

        private bool HasDeathInspection(Patient patient)
        {
            return patient.Inspections
                .Any(x => x.Conclusion == Conclusion.Death);
        }

        private int MainDiagnosesCount(List<DiagnosisCreateModel> diagnoses)
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

        private Inspection? FindInspection(Guid id)
        {
            return _dbContext.Inspections
                .FirstOrDefault(x => x.Id == id);
        }

        private Patient? FindPatient(Guid id)
        {
            return _dbContext.Patients
                .Include(x => x.Inspections)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
