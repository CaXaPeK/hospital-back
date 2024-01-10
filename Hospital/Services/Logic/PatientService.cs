using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Exceptions;
using Hospital.Models.Diagnosis;
using Hospital.Models.General;
using Hospital.Models.Icd;
using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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

        public async Task<PatientPagedListModel> GetPatientList(
            string? name,
            List<Conclusion>? conclusions,
            PatientSorting? sorting,
            bool scheduledVisits,
            bool onlyMine,
            int page,
            int size,
            Guid doctorId
            )
        {
            var patient = _dbContext.Patients
                .AsQueryable();

            var filteredPatients = ApplyPatientFilters(patient, name, conclusions, sorting, scheduledVisits, onlyMine, doctorId);

            var pagedPatients = PaginatePatients(filteredPatients, page, size);

            var pageCount = (int)Math.Ceiling((double)filteredPatients.Count() / size);

            pageCount = pageCount == 0 ? 1 : pageCount;

            if (page > pageCount)
            {
                throw new InvalidOperationException("Invalid page");
            }

            var list = new PatientPagedListModel
            {
                Patients = await pagedPatients
                    .Select(patient => new PatientModel
                    {
                        Id = patient.Id,
                        CreateTime = patient.CreateTime,
                        Name = patient.Name,
                        Birthday = patient.BirthDate,
                        Gender = patient.Gender
                    })
                    .ToListAsync(),
                Pagination = new PageInfoModel
                {
                    Size = size,
                    Count = pageCount,
                    Current = page
                }
            };

            return list;
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

        public async Task<InspectionPagedListModel> GetInspectionList(Guid patientId, List<Guid> icdRoots, bool grouped, int page, int size)
        {
            var patient = FindPatient(patientId);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found in the database");
            }

            //throw new NotFoundException((patient.Inspections == null).ToString());

            var inspections = _dbContext.Inspections
                .Where(i => i.PatientId == patientId)
                .AsQueryable();

            //throw new NotFoundException((inspections.Include(i => i.Diagnoses).First().Diagnoses == null).ToString());

            var filteredInspections = ApplyInspectionFilters(inspections, icdRoots, grouped);

            var pagedInspections = PaginateInspections(filteredInspections, page, size);

            var pageCount = (int)Math.Ceiling((double)filteredInspections.Count() / size);

            pageCount = pageCount == 0 ? 1 : pageCount;

            if (page > pageCount)
            {
                throw new InvalidOperationException("Invalid page");
            }

            var list = new InspectionPagedListModel
            {
                Inspections = pagedInspections
                    .Select(inspection => new InspectionPreviewModel
                    {
                        Id = inspection.Id,
                        CreateTime = inspection.CreateTime,
                        PreviousId = inspection.PreviousInspectionId,
                        Date = inspection.Date,
                        Conclusion = inspection.Conclusion,
                        DoctorId = inspection.AuthorId,
                        Doctor = _dbContext.Doctors.First(d => d.Id == inspection.AuthorId).Name,
                        PatientId = inspection.PatientId,
                        Patient = _dbContext.Patients.First(p => p.Id == inspection.PatientId).Name,
                        Diagnosis = CreateDiagnosisModel
                        (
                            inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main),
                            _dbContext.Diagnoses.First(icdD => icdD.Id == inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main).IcdDiagnosisId)
                        ),
                        HasChain = inspection.BaseInspectionId == inspection.Id,
                        HasNested = _dbContext.Inspections.Any(i => i.PreviousInspectionId == inspection.Id)
                    })
                    .ToList(),
                Pagination = new PageInfoModel
                {
                    Size = size,
                    Count = pageCount,
                    Current = page
                }
            };

            return list;
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

        private static DiagnosisModel CreateDiagnosisModel(InspectionDiagnosis diagnosis, Diagnosis icdDiagnosis)
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

        private IQueryable<Patient> ApplyPatientFilters(
            IQueryable<Patient> patients,
            string? name,
            List<Conclusion>? conclusions,
            PatientSorting? sorting,
            bool scheduledVisits,
            bool onlyMine,
            Guid doctorId
            )
        {
            if (name != "" && name != null)
            {
                patients = patients.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            }

            if (conclusions.Count != 0)
            {
                patients = patients
                    .Where(p => p.Inspections.Any(i => conclusions.Contains(i.Conclusion)));
            }

            if (scheduledVisits)
            {
                patients = patients
                    .Where(p => p.Inspections
                        .Any(i => i.NextVisitDate != null && !p.Inspections.Any(ii => ii.PreviousInspectionId == i.Id)));
            }

            if (onlyMine)
            {
                patients = patients
                    .Where(p => p.Inspections.Any(i => i.AuthorId == doctorId));
            }

            switch (sorting)
            {
                case PatientSorting.NameAsc:
                    patients = patients.OrderBy(p => p.Name);
                    break;
                case PatientSorting.NameDesc:
                    patients = patients.OrderByDescending(p => p.Name);
                    break;
                case PatientSorting.CreateAsc:
                    patients = patients.OrderBy(p => p.CreateTime);
                    break;
                case PatientSorting.CreateDesc:
                    patients = patients.OrderByDescending(p => p.CreateTime);
                    break;
                case PatientSorting.InspectionAsc:
                    patients = patients.OrderBy(p => p.Inspections.Min(i => i.Date));
                    break;
                case PatientSorting.InspectionDesc:
                    patients = patients.OrderByDescending(p => p.Inspections.Max(i => i.Date));
                    break;
                default:
                    patients = patients.OrderBy(p => p.Name);
                    break;
            }

            return patients;
        }
        private IQueryable<Inspection> ApplyInspectionFilters(IQueryable<Inspection> inspections, List<Guid> icdRoots, bool grouped)
        {
            if (icdRoots.Count != 0)
            {
                var icdRootCodes = new List<string>();

                foreach (var diagnosisId in icdRoots)
                {
                    var diagnosis = _dbContext.Diagnoses.FirstOrDefault(d => d.Id == diagnosisId);

                    if (diagnosis == null)
                    {
                        throw new NotFoundException($"Diagnosis with ID {diagnosisId} not found in the database");
                    }

                    if (diagnosis.ParentId != null)
                    {
                        throw new InvalidOperationException($"Diagnosis with ID {diagnosisId} is not a root diagnosis");
                    }

                    icdRootCodes.Add(diagnosis.MkbCode);
                }

                inspections = inspections
                    .Where(i => i.Diagnoses.Any(d => d.Type == DiagnosisType.Main
                            && icdRootCodes.Contains(_dbContext.Diagnoses.FirstOrDefault(icdD => icdD.Id == d.IcdDiagnosisId).MkbCode)));
            }

            if (grouped)
            {
                inspections = inspections
                    .Where(i => i.PreviousInspectionId == null);
            }

            return inspections;
        }

        private void ValidateInspection(InspectionCreateModel newInspection, Patient patient)
        { 
            if (HasDeathInspection(patient.Id))
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

                var prevInspectionId = newInspection.PreviousInspectionId.Value;

                if (_dbContext.Inspections.Any(i => i.PreviousInspectionId == prevInspectionId))
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

        private bool HasDeathInspection(Guid patientId)
        {
            return _dbContext.Inspections
                .Any(i => i.Conclusion == Conclusion.Death && i.PatientId == patientId);
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
                .FirstOrDefault(i => i.Id == id);
        }

        private Patient? FindPatient(Guid id)
        {
            return _dbContext.Patients
                .Include(p => p.Inspections)
                .FirstOrDefault(p => p.Id == id);
        }

        private IQueryable<Patient> PaginatePatients(IQueryable<Patient> patients, int page, int size)
        {
            return patients.Skip((page - 1) * size).Take(size);
        }

        private IQueryable<Inspection> PaginateInspections(IQueryable<Inspection> inspections, int page, int size)
        {
            return inspections.Skip((page - 1) * size).Take(size);
        }
    }
}
