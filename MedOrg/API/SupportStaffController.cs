using MedOrg.Data.Models.DTOs;
using MedOrg.Data.Models.Entities.MedStaff;
using MedOrg.Services.Ex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedOrg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "MedicalStaff,Admin")]
    public class SupportStaffController : ControllerBase
    {
        private readonly SupportStaffService _supportStaffService;

        public SupportStaffController(SupportStaffService supportStaffService)
        {
            _supportStaffService = supportStaffService;
        }

        /// <summary>
        /// Получить весь вспомогательный персонал
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(QueryResult<List<SupportStaff>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _supportStaffService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить сотрудника по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QueryResult<SupportStaff>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supportStaffService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<SupportStaff>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SupportStaff staff)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supportStaffService.CreateAsync(staff);

            if (result.Success && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<SupportStaff>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SupportStaff staff)
        {
            if (id != staff.Id)
                return BadRequest(new QueryResult { Success = false, Message = "ID не совпадает" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supportStaffService.UpdateAsync(staff);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supportStaffService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}