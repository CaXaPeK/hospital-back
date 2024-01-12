using Hospital.Database.TableModels;
using Hospital.Models.Diagnosis;
using Hospital.Models.Inspection;

namespace Hospital.Services.Interfaces
{
    public interface IInspectionService
    {
        Task<InspectionModel> GetFullInspection(Guid inspectionId);

        Task EditInspection(Guid inspectionId, InspectionEditModel editedInspection, Guid doctorId);

        Task<List<InspectionPreviewModel>> GetInspectionChain(Guid rootId);

        void ValidateCreateInspection(InspectionCreateModel newInspection, Patient patient);

        IQueryable<Inspection> FilterInspections(IQueryable<Inspection> inspections, List<Guid> icdRoots, bool grouped);

        IQueryable<Inspection> PaginateInspections(IQueryable<Inspection> inspections, int page, int size);

        InspectionPagedListModel GetPagedFilteredInspectionList(IQueryable<Inspection> inspections, List<Guid> icdRoots, bool grouped, int page, int size);

        List<string> GetIcdRootCodes(List<Guid> icdRoots);
    }
}
