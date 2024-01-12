using Hospital.Models.Icd;

namespace Hospital.Services.Interfaces
{
    public interface IReportService
    {
        Task<IcdRootsReportModel> GetIcdRootsReport(DateTime start, DateTime end, List<Guid> icdRoots);
    }
}
