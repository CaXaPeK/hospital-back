using Hospital.Database;
using Hospital.Database.TableModels;
using Hospital.Exceptions;
using Hospital.Models.Diagnosis;
using Hospital.Models.General;
using Hospital.Models.Icd;
using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Security.Authentication;

namespace Hospital.Services.Logic
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _dbContext;
        private readonly IInspectionService _inspectionService;

        public PatientService(AppDbContext dbContext, IInspectionService inspectionService)
        {
            _dbContext = dbContext;
            _inspectionService = inspectionService;
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

            var filteredPatients = FilterPatients(patient, name, conclusions, sorting, scheduledVisits, onlyMine, doctorId);

            var pagedPatients = PaginatePatients(filteredPatients, page, size);

            var pageCount = (int)Math.Ceiling((double)filteredPatients.Count() / size);

            if (page > pageCount && pageCount != 0)
            {
                throw new InvalidCredentialException("Invalid page");
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
            var patient = _dbContext.Patients
                .Include(p => p.Inspections)
                .FirstOrDefault(p => p.Id == patientId);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found in the database");
            }

            _inspectionService.ValidateCreateInspection(newInspection, patient);

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

                var doctor = _dbContext.Doctors
                    .Include(d => d.Consultations)
                    .First(d => d.Id == authorId);

                doctor.Comments.Add(newComment);
                doctor.Consultations.Add(consultationEntity);
            }

            var baseInspectionId = new Guid?();

            if (newInspection.PreviousInspectionId == null)
            {
                baseInspectionId = null;
            }
            else
            {
                var previousInspection = patient.Inspections.First(i => i.Id == newInspection.PreviousInspectionId.Value);

                if (previousInspection.BaseInspectionId == null)
                {
                    baseInspectionId = previousInspection.Id;
                }
                else
                {
                    baseInspectionId = previousInspection.BaseInspectionId;
                }

                previousInspection.NextInspectionId = newInspectionId;
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
                DoctorId = authorId
            };

            await _dbContext.Inspections.AddAsync(inspection);
            patient.Inspections.Add(inspection);
            await _dbContext.SaveChangesAsync();

            return inspection.Id;
        }

        public async Task<InspectionPagedListModel> GetInspectionList(Guid patientId, List<Guid> icdRoots, bool grouped, int page, int size)
        {
            var patient = _dbContext.Patients
                .Include(p => p.Inspections)
                .FirstOrDefault(p => p.Id == patientId);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found in the database");
            }

            var inspections = _dbContext.Inspections
                .Include(i => i.Diagnoses).ThenInclude(d => d.IcdDiagnosis)
                .Where(i => i.Patient == patient)
                .AsQueryable();

            var list = _inspectionService.GetPagedFilteredInspectionList(inspections, icdRoots, grouped, page, size);

            return list;
        }

        public async Task<PatientModel> GetPatient(Guid id)
        {
            var patient = _dbContext.Patients
                .FirstOrDefault(p => p.Id == id);

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

        public async Task<List<InspectionShortModel>> GetInspectionsWithoutChildren(Guid patientId, string? request)
        {
            var patient = _dbContext.Patients
                .FirstOrDefault(p => p.Id == patientId);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found in the database");
            }

            var inspections = _dbContext.Inspections
                .Include(i => i.Diagnoses).ThenInclude(d => d.IcdDiagnosis)
                .Where(i => i.PatientId == patientId)
                .AsQueryable();

            inspections = inspections
                .Where(i => i.NextInspectionId == null);

            if (request != "" && request != null)
            {
                inspections = inspections
                    .Where(i => MainDiagnosisMatchesNameOrCode(i, request));
            }

            var list = inspections
                .Select(inspection => new InspectionShortModel
                {
                    Id = inspection.Id,
                    CreateTime = inspection.CreateTime,
                    Date = inspection.Date,
                    Diagnosis = CreateMainDiagnosisModel(inspection)
                })
                .ToList();

            return list;
        }

        private static DiagnosisModel CreateMainDiagnosisModel(Inspection inspection)
        {
            var diagnosis = inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main);

            return new DiagnosisModel
            {
                Id = diagnosis.Id,
                CreateTime = diagnosis.CreateTime,
                Code = diagnosis.IcdDiagnosis.MkbCode,
                Name = diagnosis.IcdDiagnosis.MkbName,
                Description = diagnosis.Description,
                Type = DiagnosisType.Main
            };
        }

        private bool MainDiagnosisMatchesNameOrCode(Inspection inspection, string request)
        {
            var diagnosis = inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main).IcdDiagnosis;

            return diagnosis.MkbName.ToLower().Contains(request.ToLower())
                || diagnosis.MkbCode.ToLower().Contains(request.ToLower());
        }

        private IQueryable<Patient> FilterPatients(
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
                patients = patients.Where(p => p.Name.ToLower().Contains(name.ToLower()));
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
                        .Any(i => i.NextVisitDate != null && i.NextInspectionId == null));
            }

            if (onlyMine)
            {
                patients = patients
                    .Where(p => p.Inspections.Any(i => i.DoctorId == doctorId));
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

        private IQueryable<Patient> PaginatePatients(IQueryable<Patient> patients, int page, int size)
        {
            return patients.Skip((page - 1) * size).Take(size);
        }
    }
}
