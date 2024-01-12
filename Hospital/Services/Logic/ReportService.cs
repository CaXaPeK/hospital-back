using Hospital.Database;
using Hospital.Models.Diagnosis;
using Hospital.Models.Icd;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace Hospital.Services.Logic
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;
        private readonly IInspectionService _inspectionService;

        public ReportService(AppDbContext dbContext, IInspectionService inspectionService)
        {
            _dbContext = dbContext;
            _inspectionService = inspectionService;
        }

        public async Task<IcdRootsReportModel> GetIcdRootsReport(DateTime start, DateTime end, List<Guid> icdRoots)
        {
            if (start > end)
            {
                throw new InvalidCredentialException("Invalid time interval");
            }

            List<string> icdRootCodes;

            if (icdRoots.Count != 0)
            {
                icdRootCodes = _inspectionService.GetIcdRootCodes(icdRoots.Distinct().ToList());
            }
            else
            {
                icdRootCodes = _dbContext.Diagnoses
                    .Where(d => d.ParentId == null)
                    .OrderBy(d => d.MkbCode)
                    .Select(d => d.MkbCode)
                    .ToList();
            }

            var inspections = _dbContext.Inspections
                .Include(i => i.Diagnoses).ThenInclude(d => d.IcdDiagnosis)
                .Where(i => i.Diagnoses.Any(d => icdRootCodes.Contains(d.IcdDiagnosis.RootCode))
                    && i.Date >= start && i.Date <= end)
                .ToList();

            var patientsAndVisits = new Dictionary<Guid, Dictionary<string, int>>();

            foreach (var inspection in inspections) //считаю все посещения
            {
                if (!patientsAndVisits.ContainsKey(inspection.PatientId)) //добавляю пациента, если его нет в словаре
                {
                    patientsAndVisits.Add(inspection.PatientId, new Dictionary<string, int>());

                    foreach (var rootCode in icdRootCodes)
                    {
                        patientsAndVisits[inspection.PatientId].Add(rootCode, 0); //инициирую счётчики
                    }
                }

                var inspectionDiagnosisCode = inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main).IcdDiagnosis.RootCode;

                if (patientsAndVisits[inspection.PatientId].ContainsKey(inspectionDiagnosisCode)) //засчитываю код диагноза посещения
                {
                    patientsAndVisits[inspection.PatientId][inspectionDiagnosisCode]++;
                }
            }

            var records = new List<IcdRootsReportRecordModel>();

            var summaryByRoot = new Dictionary<string, int>();

            foreach (var rootCode in icdRootCodes) //инициирую счётчики
            {
                summaryByRoot.Add(rootCode, 0);
            }

            foreach (var patient in patientsAndVisits)
            {
                foreach (var visitTracker in patient.Value) 
                {
                    if (visitTracker.Value == 0) //удаляю нулевые счётчики
                    {
                        patient.Value.Remove(visitTracker.Key); 
                    }
                    else
                    {
                        summaryByRoot[visitTracker.Key] += visitTracker.Value;
                    }
                }

                var patientEntity = _dbContext.Patients
                    .First(p => p.Id == patient.Key);

                records.Add(new IcdRootsReportRecordModel
                {
                    PatientName = patientEntity.Name,
                    PatientBirthdate = patientEntity.BirthDate,
                    Gender = patientEntity.Gender,
                    VisitsByRoot = patient.Value
                });
            }

            return new IcdRootsReportModel
            {
                Filters = new IcdRootsReportFiltersModel
                {
                    Start = start,
                    End = end,
                    IcdRoots = icdRootCodes
                },
                Records = records,
                SummaryByRoot = summaryByRoot
            };
        }
    }
}
