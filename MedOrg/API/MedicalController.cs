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
    public class MedicalController : ControllerBase
    {
        private readonly QueryService _queryService;
        private readonly DocumentService _documentService;

        public MedicalController(QueryService queryService, DocumentService documentService)
        {
            _queryService = queryService;
            _documentService = documentService;
        }

        /// <summary>
        /// Получить врачей по специализации
        /// </summary>
        [HttpGet("doctors/by-specialization")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsBySpecialization(
            [FromQuery] string specialization,
            [FromQuery] int? institutionId = null,
            [FromQuery] string? institutionType = null)
        {
            var result = await _queryService.GetDoctorsBySpecializationAsync(
                specialization, institutionId, institutionType);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить вспомогательный персонал по должности
        /// </summary>
        [HttpGet("staff/by-position")]
        [ProducesResponseType(typeof(QueryResult<List<SupportStaffDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSupportStaffByPosition(
            [FromQuery] string position,
            [FromQuery] int? institutionId = null,
            [FromQuery] string? institutionType = null)
        {
            var result = await _queryService.GetSupportStaffByPositionAsync(
                position, institutionId, institutionType);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить врачей с минимальным количеством операций
        /// </summary>
        [HttpGet("doctors/by-min-operations")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsByMinOperations(
            [FromQuery] string specialization,
            [FromQuery] int minOperations,
            [FromQuery] int? institutionId = null,
            [FromQuery] string? institutionType = null)
        {
            var result = await _queryService.GetDoctorsByMinOperationsAsync(
                specialization, minOperations, institutionId, institutionType);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить врачей с минимальным стажем
        /// </summary>
        [HttpGet("doctors/by-min-experience")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsByMinExperience(
            [FromQuery] string specialization,
            [FromQuery] int minYears,
            [FromQuery] int? institutionId = null,
            [FromQuery] string? institutionType = null)
        {
            var result = await _queryService.GetDoctorsByMinExperienceAsync(
                specialization, minYears, institutionId, institutionType);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить врачей по ученой степени и званию
        /// </summary>
        [HttpGet("doctors/by-degree-title")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsByDegreeAndTitle(
            [FromQuery] string specialization,
            [FromQuery] AcademicDegree? degree = null,
            [FromQuery] AcademicTitle? title = null,
            [FromQuery] int? institutionId = null,
            [FromQuery] string? institutionType = null)
        {
            var result = await _queryService.GetDoctorsByDegreeAndTitleAsync(
                specialization, degree, title, institutionId, institutionType);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить госпитализированных пациентов
        /// </summary>
        [HttpGet("patients/hospitalized")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<HospitalizedPatientDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitalizedPatients(
            [FromQuery] int? hospitalId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? wardId = null)
        {
            var result = await _queryService.GetHospitalizedPatientsAsync(
                hospitalId, departmentId, wardId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить выписанных пациентов
        /// </summary>
        [HttpGet("patients/discharged")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<HospitalizedPatientDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDischargedPatients(
            [FromQuery] int? hospitalId = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _queryService.GetDischargedPatientsAsync(
                hospitalId, doctorId, startDate, endDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить пациентов поликлиники
        /// </summary>
        [HttpGet("patients/clinic")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<ClinicPatientDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClinicPatients(
            [FromQuery] string specialization,
            [FromQuery] int clinicId)
        {
            var result = await _queryService.GetClinicPatientsByDoctorAsync(
                specialization, clinicId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить статистику по палатам
        /// </summary>
        [HttpGet("statistics/wards")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<WardStatisticsDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWardStatistics([FromQuery] int hospitalId)
        {
            var result = await _queryService.GetWardStatisticsAsync(hospitalId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить статистику по кабинетам
        /// </summary>
        [HttpGet("statistics/offices")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<OfficeStatisticsDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOfficeStatistics(
            [FromQuery] int clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _queryService.GetOfficeStatisticsAsync(
                clinicId, startDate, endDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить статистику производительности врачей
        /// </summary>
        [HttpGet("statistics/doctor-productivity")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorProductivityDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorProductivity(
            [FromQuery] int? doctorId = null,
            [FromQuery] int? clinicId = null,
            [FromQuery] string? specialization = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _queryService.GetDoctorProductivityAsync(
                doctorId, clinicId, specialization, startDate, endDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить загруженность врачей
        /// </summary>
        [HttpGet("statistics/doctor-workload")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorWorkloadDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorWorkload(
            [FromQuery] int? doctorId = null,
            [FromQuery] int? hospitalId = null,
            [FromQuery] string? specialization = null)
        {
            var result = await _queryService.GetDoctorWorkloadAsync(
                doctorId, hospitalId, specialization);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить список операций
        /// </summary>
        [HttpGet("operations")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<OperationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperations(
            [FromQuery] int? hospitalId = null,
            [FromQuery] int? clinicId = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _queryService.GetOperationsAsync(
                hospitalId, clinicId, doctorId, startDate, endDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Получить статистику производительности лабораторий
        /// </summary>
        [HttpGet("statistics/laboratory-productivity")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<LaboratoryProductivityDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLaboratoryProductivity(
            [FromQuery] int? institutionId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _queryService.GetLaboratoryProductivityAsync(
                institutionId, startDate, endDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Выдать больничный лист
        /// </summary>
        [HttpPost("documents/sick-leave")]
        [Authorize(Roles = "MedicalStaff")]
        [ProducesResponseType(typeof(QueryResult<SickLeaveDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IssueSickLeave(
            [FromQuery] int patientId,
            [FromQuery] int doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string diagnosis)
        {
            var result = await _documentService.IssueSickLeaveAsync(
                patientId, doctorId, startDate, endDate, diagnosis);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Выдать медицинскую справку
        /// </summary>
        [HttpPost("documents/certificate")]
        [Authorize(Roles = "MedicalStaff")]
        [ProducesResponseType(typeof(QueryResult<MedicalCertificateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IssueMedicalCertificate(
            [FromQuery] int patientId,
            [FromQuery] int doctorId)
        {
            var result = await _documentService.IssueMedicalCertificateAsync(
                patientId, doctorId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Сформировать расписание поликлиники
        /// </summary>
        [HttpGet("documents/schedule")]
        [Authorize(Roles = "MedicalStaff,Admin")]
        [ProducesResponseType(typeof(QueryResult<List<DoctorScheduleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateClinicSchedule([FromQuery] int clinicId)
        {
            var result = await _documentService.GenerateClinicScheduleAsync(clinicId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}