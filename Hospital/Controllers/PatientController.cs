using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        private readonly ITokenService _tokenService;

        public PatientController(IPatientService patientService, ITokenService tokenService)
        {
            _patientService = patientService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Create new patient
        /// </summary>
        /// <response code="200">Patient was registered</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreatePatient(PatientCreateModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var patientId = await _patientService.CreatePatient(data);

                return Ok(patientId);
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
