using MedOrg.Models.DTOs;
using MedOrg.Models.Entities;
using MedOrg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedOrg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "MedicalStaff,Admin")]
    public class ClinicsController : ControllerBase
    {
        private readonly ClinicService _clinicService;

        public ClinicsController(ClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        /// <summary>
        /// Получить все поликлиники
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(QueryResult<List<Clinic>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clinicService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить поликлинику по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QueryResult<Clinic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _clinicService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Создать новую поликлинику
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Clinic>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Clinic clinic)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clinicService.CreateAsync(clinic);

            if (result.Success && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Обновить данные поликлиники
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Clinic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Clinic clinic)
        {
            if (id != clinic.Id)
                return BadRequest(new QueryResult { Success = false, Message = "ID не совпадает" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clinicService.UpdateAsync(clinic);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Удалить поликлинику
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _clinicService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}