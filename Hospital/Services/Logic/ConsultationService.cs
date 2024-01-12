using Hospital.Database;
using Hospital.Database.TableModels;
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
                throw new KeyNotFoundException($"Consultation with ID {id} not found in the database");
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

        public async Task<Guid> AddComment(Guid consultationId, CommentCreateModel newComment, Guid doctorId)
        {
            var consultation = _dbContext.Consultations
                .Include(c => c.Inspection)
                .FirstOrDefault(c => c.Id == consultationId);

            if (consultation == null)
            {
                throw new KeyNotFoundException($"Parent comment with ID {consultationId} not found in the database");
            }

            var parentComment = _dbContext.Comments
                .FirstOrDefault(c => c.Id == newComment.ParentId);

            if (parentComment == null)
            {
                throw new KeyNotFoundException($"Parent comment with ID {newComment.ParentId} not found in the database");
            }

            var doctor = _dbContext.Doctors
                .First(d => d.Id == doctorId);

            if (doctor.Speciality != consultation.Speciality && doctor != consultation.Inspection.Doctor)
            {
                throw new MethodAccessException($"Can't leave a comment: You should be the inspection's author or have a speciality of ID {consultation.SpecialityId}");
            }

            var commentEntity = new Comment
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                Content = newComment.Content,
                Author = doctor,
                Parent = parentComment,
                Consultation = consultation
            };

            _dbContext.Comments.Add(commentEntity);
            parentComment.ChildComments.Add(commentEntity);
            consultation.Comments.Add(commentEntity);
            doctor.Comments.Add(commentEntity);

            await _dbContext.SaveChangesAsync();

            return commentEntity.Id;
        }

        public async Task EditComment(Guid commentId, InspectionCommentCreateModel editedComment, Guid doctorId)
        {
            var comment = _dbContext.Comments
                .FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found in the database");
            }

            if (comment.AuthorId != doctorId)
            {
                throw new MethodAccessException("Can't edit a comment of a different doctor");
            }

            comment.Content = editedComment.Content;
            comment.ModifiedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}
