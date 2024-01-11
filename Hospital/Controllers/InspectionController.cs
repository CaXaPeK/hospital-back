using Hospital.Exceptions;
using Hospital.Models.General;
using Hospital.Services.Interfaces;
using Hospital.Services.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFullInspection(Guid id)
        {
            try
            {
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
    }
}
