using Hospital.Models.Inspection;
using Hospital.Models.Consultation;
using Hospital.Models.Comment;

namespace Hospital.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetYourSpecialityInspections(Guid doctorId, List<Guid> icdRoots, bool grouped, int page, int size);

        Task<ConsultationModel> GetConsultation(Guid id);

        Task<Guid> AddComment(Guid consultationId, CommentCreateModel newComment, Guid doctorId);

        Task EditComment(Guid commentId, InspectionCommentCreateModel editedComment, Guid doctorId);
    }
}
