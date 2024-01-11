using Hospital.Database;
using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace Hospital.Services.Logic
{
    public class ConsultationService : IConsultationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDictionaryService _dictionaryService;
        private readonly IInspectionService _inspectionService;

        public ConsultationService(AppDbContext dbContext, IDictionaryService dictionaryService, IInspectionService inspectionService)
        {
            _dbContext = dbContext;
            _dictionaryService = dictionaryService;
            _inspectionService = inspectionService;
        }

        public async Task<InspectionPagedListModel> GetYourSpecialityInspections(Guid doctorId, List<Guid> icdRoots, bool grouped, int page, int size)
        {
            var specialityId = _dbContext.Doctors
                .First(d => d.Id == doctorId).SpecialityId;

            var inspections = _dbContext.Inspections
                .Include(i => i.Diagnoses).ThenInclude(d => d.IcdDiagnosis)
                .Include(i => i.Consultations)
                .Where(i => i.Consultations.Any(c => c.SpecialityId == specialityId))
                .OrderByDescending(i => i.Date)
                .AsQueryable();

            var list = _inspectionService.GetPagedFilteredInspectionList(inspections, icdRoots, grouped, page, size);

            return list;
        }
    }
}
