using Hospital.Models.Doctor;
using Hospital.Models.General;

namespace Hospital.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> Register(DoctorRegisterModel newDoctor);

        Task<TokenResponseModel> Login(LoginCredentialsModel credentials);

        Task Logout(string token);

        Task<DoctorModel> GetProfile(Guid doctorId);

        Task EditProfile(DoctorEditModel editedDoctor, Guid doctorId);
    }
}
