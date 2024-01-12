using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Models.Consultation;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Hospital.Models.Comment;

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
        [ProducesResponseType(typeof(InspectionPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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
        /// Get concrete consultation
        /// </summary>
        /// <param name="id">Consultation's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ConsultationModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetConsultation(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var consultation = await _consultationService.GetConsultation(id);

                return Ok(consultation);
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

        /// <summary>
        /// Add comment to concrete consultation
        /// </summary>
        /// <param name="id">Consultation's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User doesn't have add comment to consultation (unsuitable specialty and not the inspection author)</response>
        /// <response code="404">Consultation or parent comment not found</response>
        /// <response code="500">InternalServerError</response>
        [HttpPost("{id}/comment")]
        [Authorize]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> AddComment(Guid id, CommentCreateModel data)
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

                var newCommentId = await _consultationService.AddComment(id, data, doctorId);

                return Ok(newCommentId);
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
        /// Edit comment
        /// </summary>
        /// <param name="id">Comment's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User is not the author of the comment</response>
        /// <response code="404">Comment not found</response>
        /// <response code="500">InternalServerError</response>
        [HttpPut("comment/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> EditComment(Guid id, InspectionCommentCreateModel data)
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

                await _consultationService.EditComment(id, data, doctorId);

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
            catch (MethodAccessException e)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ResponseModel { Status = "Error", Message = e.Message });
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
