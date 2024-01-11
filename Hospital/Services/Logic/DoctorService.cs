using Hospital.Database;
using Hospital.Models.Doctor;
using Hospital.Models.General;
using Hospital.Database.TableModels;
using Hospital.Models.Speciality;
using Hospital.Services.Interfaces;
using Hospital.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography;
using Hospital.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital.Services.Logic
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDictionaryService _dictionaryService;
        private readonly ITokenService _tokenService;

        public DoctorService(AppDbContext dbContext, IDictionaryService dictionaryService, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _dictionaryService = dictionaryService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseModel> Register(DoctorRegisterModel newDoctor)
        {
            if (!IsEmailUnique(newDoctor.Email))
            {
                throw new InvalidCredentialException($"Email \"{newDoctor.Email}\" is already taken");
            }

            if (!_dictionaryService.SpecialityExists(newDoctor.Speciality))
            {
                throw new InvalidCredentialException("Invalid speciality ID");
            }

            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                Name = newDoctor.Name,
                CreateTime = DateTime.UtcNow,
                Password = EncodePassword(newDoctor.Password),
                Email = newDoctor.Email,
                BirthDate = newDoctor.Birthday,
                Gender = newDoctor.Gender,
                Phone = newDoctor.Phone,
                SpecialityId = newDoctor.Speciality
            };

            await _dbContext.Doctors.AddAsync(doctor);
            await _dbContext.SaveChangesAsync();

            var token = _tokenService.GenerateToken(doctor);
            return new TokenResponseModel{ Token = token };
        }

        public async Task<TokenResponseModel> Login(LoginCredentialsModel credentials)
        {
            var doctor = FindDoctor(credentials.Email, credentials.Password);

            if (doctor == null)
            {
                throw new InvalidCredentialException("Login failed");
            }

            var token = _tokenService.GenerateToken(doctor);
            return new TokenResponseModel { Token = token };
        }

        public async Task Logout(string token)
        {
            _tokenService.BanToken(token);
        }

        public async Task<DoctorModel> GetProfile(Guid doctorId)
        {
            var doctor = FindDoctor(doctorId);

            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }

            var profile = new DoctorModel
            {
                Id = doctor.Id,
                CreateTime = doctor.CreateTime,
                Name = doctor.Name,
                Birthday = doctor.BirthDate,
                Gender = doctor.Gender,
                Email = doctor.Email,
                Phone = doctor.Phone
            };

            return profile;
        }

        public async Task EditProfile(DoctorEditModel editedDoctor, Guid doctorId)
        {
            var doctor = FindDoctor(doctorId);

            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }

            if (!IsEmailUnique(editedDoctor.Email) && doctor.Email != editedDoctor.Email)
            {
                throw new InvalidCredentialException($"Email \"{editedDoctor.Email}\" is already taken");
            }

            doctor.Email = editedDoctor.Email;
            doctor.Name = editedDoctor.Name;
            doctor.BirthDate = editedDoctor.Birthday;
            doctor.Gender = editedDoctor.Gender;
            doctor.Phone = editedDoctor.Phone;

            await _dbContext.SaveChangesAsync();
        }

        private Doctor? FindDoctor(Guid id)
        {
            return _dbContext.Doctors
                .FirstOrDefault(x => x.Id == id);
        }

        private Doctor? FindDoctor(string email, string password)
        {
            return _dbContext.Doctors
                .FirstOrDefault(x => x.Email == email && x.Password == EncodePassword(password));
        }

        private bool IsEmailUnique(string email)
        {
            return _dbContext.Doctors.Where(x => x.Email == email).Count() == 0;
        }

        private string EncodePassword(string password)
        {
            using SHA256 hash = SHA256.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }
    }
}
