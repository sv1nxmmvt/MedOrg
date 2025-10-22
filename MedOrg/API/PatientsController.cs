using MedOrg.Models.DTOs;
using MedOrg.Models.Entities;
using MedOrg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedOrg.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Получить всех пациентов
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<Patient>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _patientService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить пациента по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QueryResult<Patient>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // Пациенты могут видеть только свои данные
            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst("PatientId")?.Value;
                if (string.IsNullOrEmpty(patientIdClaim) ||
                    !int.TryParse(patientIdClaim, out int patientId) ||
                    patientId != id)
                {
                    return Forbid();
                }
            }

            var result = await _patientService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Создать нового пациента
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<Patient>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.CreateAsync(patient);

            if (result.Success && result.Data != null)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Обновить данные пациента
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(QueryResult<Patient>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
                return BadRequest(new QueryResult { Success = false, Message = "ID не совпадает" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst("PatientId")?.Value;
                if (string.IsNullOrEmpty(patientIdClaim) ||
                    !int.TryParse(patientIdClaim, out int patientId) ||
                    patientId != id)
                {
                    return Forbid();
                }
            }

            var result = await _patientService.UpdateAsync(patient);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Удалить пациента
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(QueryResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}