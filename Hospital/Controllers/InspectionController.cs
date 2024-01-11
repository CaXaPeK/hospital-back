using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Models.Inspection;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace Hospital.Controllers
{
    [Route("api/inspection")]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;
        private readonly ITokenService _tokenService;

        public InspectionController(IInspectionService inspectionService, ITokenService tokenService)
        {
            _inspectionService = inspectionService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get full information about specified inspection
        /// </summary>
        /// <param name="id">Inspection's identifier</param>
        /// <response code="200">Inspection found and successfully extracted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(InspectionModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetFullInspection(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var inspection = await _inspectionService.GetFullInspection(id);

                return Ok(inspection);
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
        /// Edit concrete inspection
        /// </summary>
        /// <param name="id">Inspection's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid arguments</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">User doesn't have editing rights (not the inspection author)</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> EditInspection(Guid id, InspectionEditModel data)
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

               await _inspectionService.EditInspection(id, data, doctorId);

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
        /// Get medical inspection chain for root inspection
        /// </summary>
        /// <param name="id">Root inspection's identifier</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("{id}/chain")]
        [Authorize]
        public async Task<IActionResult> GetInspectionChain(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await HttpContext.GetTokenAsync("access_token");
                _tokenService.ValidateToken(token);

                var chain = await _inspectionService.GetInspectionChain(id);

                return Ok(chain);
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
    }
}
