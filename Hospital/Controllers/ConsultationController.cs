using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace Hospital.Controllers
{
    [Route("api/consultation")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;
        private readonly ITokenService _tokenService;

        public ConsultationController(IConsultationService consultationService, ITokenService tokenService)
        {
            _consultationService = consultationService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get a list of medical inspections for consultation
        /// </summary>
        /// <param name="grouped">flag - whether grouping by inspection chain is required - for filtration</param>
        /// <param name="icdRoots">root elements for ICD-10 - for filtration</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <response code="200">Inspections for consultation list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetYourSpecialityInspections(
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
                var doctorId = _tokenService.GetDoctorId(token);

                var list = await _consultationService.GetYourSpecialityInspections(doctorId, icdRoots, grouped, page, size);

                return Ok(list);
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
    }
}
