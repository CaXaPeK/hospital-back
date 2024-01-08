using Hospital.Models.Doctor;
using Hospital.Models.General;

namespace Hospital.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> Register(DoctorRegisterModel data);
    }
}
