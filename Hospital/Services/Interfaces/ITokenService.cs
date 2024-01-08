using Hospital.Database.TableModels;

namespace Hospital.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Doctor doctor);
    }
}
