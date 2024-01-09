using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        /// <summary>
        /// Create inspection for specified patient
        /// </summary>
        /// /// <param name="id">Patient's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Can't create new inspection</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("{id}/inspections")]
        [Authorize]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreateInspection(InspectionCreateModel data, Guid id)
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

                var inspectionId = await _patientService.CreateInspection(data, id, doctorId);

                return Ok(inspectionId);
            }
            catch (InvalidCredentialException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (MethodAccessException e)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (NotFoundException e)
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
        /// Get patient card
        /// </summary>
        /// /// <param name="id">Patient's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(PatientModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var patient = await _patientService.GetPatient(id);

                return Ok(patient);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (NotFoundException e)
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
