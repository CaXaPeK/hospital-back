using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Icd;
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
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ITokenService _tokenService;

        public ReportController(IReportService reportService, ITokenService tokenService)
        {
            _reportService = reportService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get a report on patients' visits based on ICD-10 roots for a specified time interval
        /// </summary>
        /// <param name="start">Start of tome interval</param>
        /// <param name="end">End of time interval</param>
        /// <param name="icdRoots">Set of ICD-10 roots. All possible roots if null</param>
        /// <response code="200">Report extracted successfully</response>
        /// <response code="400">Some fields in request are invalid</response>
        /// <response code="401">No authentication data in request</response>
        /// <response code="404">Diagnosis not found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("icdrootsreport")]
        [Authorize]
        [ProducesResponseType(typeof(IcdRootsReportModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetIcdRootsReport(
            [Required] DateTime start,
            [Required] DateTime end,
            [FromQuery] List<Guid> icdRoots
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

                var report = await _reportService.GetIcdRootsReport(start, end, icdRoots);

                return Ok(report);
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
