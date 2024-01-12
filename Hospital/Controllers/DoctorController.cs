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

        /// <summary>
        /// Register new user
        /// </summary>
        /// <response code="200">Doctor was registered</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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

        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <response code="200">Logged in successfully</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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

        /// <summary>
        /// Log out system user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(DoctorModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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
            catch (KeyNotFoundException e)
            {
                return NotFound(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }

        /// <summary>
        /// Edit user Profile
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        [Authorize]
        public async Task<IActionResult> EditProfile(DoctorEditModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);
                var doctorId = _tokenService.GetDoctorId(token);

                await _doctorService.EditProfile(data, doctorId);

                return Ok();
            }
            catch (InvalidCredentialException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
