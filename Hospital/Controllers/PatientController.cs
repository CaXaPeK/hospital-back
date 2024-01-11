using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Models.Patient;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        /// Get patients list
        /// </summary>
        /// <param name="name">part of the name for filtering</param>
        /// <param name="conclusions">conclusion list to filter by conclusions</param>
        /// <param name="sorting">option to sort patients</param>
        /// <param name="scheduledVisits">show only scheduled visits</param>
        /// <param name="onlyMine">show inspections done by this doctor</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <response code="200">Patients paged list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination/sorting</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(PatientPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetPatientList(
            [MaxLength(255)] string? name,
            [FromQuery] List<Conclusion>? conclusions,
            [FromQuery] PatientSorting? sorting,
            [FromQuery] bool scheduledVisits = false,
            [FromQuery] bool onlyMine = false,
            [Range(1, Int32.MaxValue)] int page = 1,
            [Range(1, Int32.MaxValue)] int size = 5
            )
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

                var list = await _patientService.GetPatientList(name, conclusions, sorting, scheduledVisits, onlyMine, page, size, doctorId);

                return Ok(list);
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

        /// <summary>
        /// Create inspection for specified patient
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
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
        /// Get a list of patient medical inspections
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <param name="grouped">flag - whether grouping by inspection chain is required - for filtration</param>
        /// <param name="icdRoots">root elements for ICD-10 - for filtration</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <response code="200">Patients inspections list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Can't create new inspection</response>
        /// <response code="404">Patient not found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}/inspections")]
        [Authorize]
        [ProducesResponseType(typeof(InspectionPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetInspectionList(
            Guid id,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] bool grouped = false,
            [Range(1, Int32.MaxValue)] int page = 1,
            [Range(1, Int32.MaxValue)] int size = 5
            )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var list = await _patientService.GetInspectionList(id, icdRoots, grouped, page, size);

                return Ok(list);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
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

        /// <summary>
        /// Get patient card
        /// </summary>
        /// <param name="id">Patient's identifier</param>
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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

        /// <summary>
        /// Search for patient medical inspections without child inspections
        /// </summary>
        /// <param name="id">Patient's identifier</param>
        /// <param name="request">part of the diagnosis name or code</param>
        /// <response code="200">Patients inspections list retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}/inspections/search")]
        [Authorize]

        [ProducesResponseType(typeof(InspectionShortModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetInspectionsWithoutChildren(Guid id, [FromQuery] string? request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var list = await _patientService.GetInspectionsWithoutChildren(id, request);

                return Ok(list);
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
