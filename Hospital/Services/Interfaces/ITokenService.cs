using Hospital.Database.TableModels;

namespace Hospital.Services.Interfaces
{
    public interface ITokenService
    {
        void BanToken(string token);

        string GenerateToken(Doctor doctor);

        void ValidateToken(string token);
    }
}
