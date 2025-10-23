using MedOrg.Data.Models.DTOs;
using MedOrg.Data.Models.DTOs.MedStaff;
using MedOrg.Data.Models.Entities.MedStaff;
using MedOrg.Services.Ex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedOrg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "MedicalStaff,Admin")]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Получить всех врачей
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(QueryResult<List<DoctorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doctorService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить врача по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QueryResult<Doctor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Создать нового врача
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Doctor>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _doctorService.CreateAsync(doctor);

            if (result.Success && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Обновить данные врача
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<Doctor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
        {
            if (id != doctor.Id)
                return BadRequest(new QueryResult { Success = false, Message = "ID не совпадает" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _doctorService.UpdateAsync(doctor);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Удалить врача
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}