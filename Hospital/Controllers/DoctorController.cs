using Hospital.Models.Doctor;
using Hospital.Models.General;
using Hospital.Models.Speciality;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Hospital.Services.Interfaces;
using System.Security.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Hospital.Services.Logic;

namespace Hospital.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ITokenService _tokenService;

        public DoctorController(IDoctorService doctorService, ITokenService tokenService)
        {
            _doctorService = doctorService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DoctorRegisterModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await _doctorService.Register(data);
                return Ok(token);
            }
            catch (InvalidCredentialException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await _doctorService.Login(data);
                return Ok(token);
            }
            catch (InvalidCredentialException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                await _doctorService.Logout(token);

                return Ok(new ResponseModel { Status = null, Message = "Logged Out" });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);
                var doctorId = _tokenService.GetDoctorId(token);

                var profile = await _doctorService.GetProfile(doctorId);

                return Ok(profile);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
