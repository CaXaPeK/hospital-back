using Hospital.Database.TableModels;
using Hospital.Models.Diagnosis;
using Hospital.Models.Inspection;

namespace Hospital.Services.Interfaces
{
    public interface IInspectionService
    {
        public Task<InspectionModel> GetFullInspection(Guid inspectionId);

        public Task EditInspection(Guid inspectionId, InspectionEditModel editedInspection, Guid doctorId);

        public Task<List<InspectionPreviewModel>> GetInspectionChain(Guid rootId);

        public void ValidateCreateInspection(InspectionCreateModel newInspection, Patient patient);
    }
}
