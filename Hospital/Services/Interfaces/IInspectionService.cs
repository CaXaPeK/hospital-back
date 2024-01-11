using Hospital.Database.TableModels;
using Hospital.Models.Diagnosis;
using Hospital.Models.Inspection;

namespace Hospital.Services.Interfaces
{
    public interface IInspectionService
    {
        public Task<InspectionModel> GetFullInspection(Guid inspectionId);
    }
}
