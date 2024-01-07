using Microsoft.AspNetCore.Mvc;
using Hospital.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Hospital.Models.General;

namespace Hospital.Controllers
{
    [Route("api/dictionary")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;

        public DictionaryController(IDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        [HttpGet("speciality")]
        [SwaggerResponse(400)]
        public async Task<IActionResult> GetSpecialitiesList(
            [MaxLength(255)] string? name,
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

                var list = await _dictionaryService.GetSpecialitiesList(name, page, size);
                return Ok(list);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
