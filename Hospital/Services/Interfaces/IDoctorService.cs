using Hospital.Models.Doctor;
using Hospital.Models.General;

namespace Hospital.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> Register(DoctorRegisterModel data);

        Task<TokenResponseModel> Login(LoginCredentialsModel data);

        Task Logout(string token);

        Task<DoctorModel> GetProfile(Guid doctorId);
    }
}
