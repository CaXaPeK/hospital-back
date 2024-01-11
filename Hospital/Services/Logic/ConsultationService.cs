using Hospital.Database;
using Hospital.Exceptions;
using Hospital.Models.Comment;
using Hospital.Models.Consultation;
using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Models.Speciality;
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

        public async Task<ConsultationModel> GetConsultation(Guid id)
        {
            var consultation = _dbContext.Consultations
                .Include(c => c.Speciality)
                .Include(c => c.Comments).ThenInclude(c => c.Author)
                .FirstOrDefault(c => c.Id == id);

            if (consultation == null)
            {
                throw new NotFoundException($"Consultation with ID {id} not found in the database");
            }

            var consultationModel = new ConsultationModel
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
                Comments = consultation.Comments
                    .Select(comment => new CommentModel
                    {
                        Id = comment.Id,
                        CreateTime = comment.CreateTime,
                        ModifiedDate = comment.ModifiedDate,
                        Content = comment.Content,
                        AuthorId = comment.AuthorId,
                        Author = comment.Author.Name,
                        ParentId = comment.ParentId
                    }).ToList()
            };

            return consultationModel;
        }
    }
}
