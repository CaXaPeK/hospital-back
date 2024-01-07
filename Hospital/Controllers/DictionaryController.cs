using Microsoft.AspNetCore.Mvc;
using Hospital.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Hospital.Models.General;
using Hospital.Models.Speciality;
using System.ComponentModel;
using Hospital.Models.Icd;

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

        /// <summary>
        /// Get specialities list
        /// </summary>
        /// <param name="name">part of the name for filtering</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <response code="200">Specialties paged list retrieved</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("speciality")]
        [ProducesResponseType(typeof(SpecialtiesPagedListModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
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

        /// <summary>
        /// Search for diagnoses in ICD-10 dictionary
        /// </summary>
        /// <param name="request">part of the diagnosis name or code</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <response code="200">Searching result extracted</response>
        /// <response code="400">Some fields in request are invalid</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("icd10")]
        [ProducesResponseType(typeof(Icd10SearchModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetDiagnoses(
            [MaxLength(255)] string? request,
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

                var list = await _dictionaryService.GetDiagnoses(request, page, size);
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

        /// <summary>
        /// Get root ICD-10 elements
        /// </summary>
        /// <response code="200">Root ICD-10 elements retrieved</response>
        /// <response code="500">InternalServerError</response>
        [HttpGet("icd10/roots")]
        [ProducesResponseType(typeof(List<Icd10RecordModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> GetRootDiagnoses()
        {
            try
            {
                var list = await _dictionaryService.GetRootDiagnoses();
                return Ok(list);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
