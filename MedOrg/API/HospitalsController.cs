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
    public class HospitalsController : ControllerBase
    {
        private readonly HospitalService _hospitalService;

        public HospitalsController(HospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        /// <summary>
        /// Получить все больницы
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(QueryResult<List<Hospital>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _hospitalService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить больницу по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QueryResult<Hospital>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _hospitalService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Создать новую больницу
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Hospital>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Hospital hospital)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _hospitalService.CreateAsync(hospital);

            if (result.Success && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Обновить данные больницы
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Hospital>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Hospital hospital)
        {
            if (id != hospital.Id)
                return BadRequest(new QueryResult { Success = false, Message = "ID не совпадает" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _hospitalService.UpdateAsync(hospital);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Удалить больницу
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hospitalService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}