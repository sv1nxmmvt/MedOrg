using MedOrg.Services;
using MedOrg.Models.Entities;

namespace MedOrg.API
{
    public static class MedicalEndpoints
    {
        public static void MapMedicalEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/medical");

            group.MapGet("/doctors/by-specialization", async (
                QueryService service,
                string specialization,
                int? institutionId = null,
                string? institutionType = null) =>
            {
                var result = await service.GetDoctorsBySpecializationAsync(specialization, institutionId, institutionType);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorsBySpecialization");

            group.MapGet("/staff/by-position", async (
                QueryService service,
                string position,
                int? institutionId = null,
                string? institutionType = null) =>
            {
                var result = await service.GetSupportStaffByPositionAsync(position, institutionId, institutionType);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetSupportStaffByPosition");

            group.MapGet("/doctors/by-min-operations", async (
                QueryService service,
                string specialization,
                int minOperations,
                int? institutionId = null,
                string? institutionType = null) =>
            {
                var result = await service.GetDoctorsByMinOperationsAsync(specialization, minOperations, institutionId, institutionType);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorsByMinOperations");

            group.MapGet("/doctors/by-min-experience", async (
                QueryService service,
                string specialization,
                int minYears,
                int? institutionId = null,
                string? institutionType = null) =>
            {
                var result = await service.GetDoctorsByMinExperienceAsync(specialization, minYears, institutionId, institutionType);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorsByMinExperience");

            group.MapGet("/doctors/by-degree-title", async (
                QueryService service,
                string specialization,
                AcademicDegree? degree = null,
                AcademicTitle? title = null,
                int? institutionId = null,
                string? institutionType = null) =>
            {
                var result = await service.GetDoctorsByDegreeAndTitleAsync(specialization, degree, title, institutionId, institutionType);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorsByDegreeAndTitle");

            group.MapGet("/patients/hospitalized", async (
                QueryService service,
                int? hospitalId = null,
                int? departmentId = null,
                int? wardId = null) =>
            {
                var result = await service.GetHospitalizedPatientsAsync(hospitalId, departmentId, wardId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetHospitalizedPatients");

            group.MapGet("/patients/discharged", async (
                QueryService service,
                int? hospitalId = null,
                int? doctorId = null,
                DateTime? startDate = null,
                DateTime? endDate = null) =>
            {
                var result = await service.GetDischargedPatientsAsync(hospitalId, doctorId, startDate, endDate);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDischargedPatients");

            group.MapGet("/patients/clinic", async (
                QueryService service,
                string specialization,
                int clinicId) =>
            {
                var result = await service.GetClinicPatientsByDoctorAsync(specialization, clinicId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetClinicPatients");

            group.MapGet("/statistics/wards", async (
                QueryService service,
                int hospitalId) =>
            {
                var result = await service.GetWardStatisticsAsync(hospitalId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetWardStatistics");

            group.MapGet("/statistics/offices", async (
                QueryService service,
                int clinicId,
                DateTime startDate,
                DateTime endDate) =>
            {
                var result = await service.GetOfficeStatisticsAsync(clinicId, startDate, endDate);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetOfficeStatistics");

            group.MapGet("/statistics/doctor-productivity", async (
                QueryService service,
                int? doctorId = null,
                int? clinicId = null,
                string? specialization = null,
                DateTime? startDate = null,
                DateTime? endDate = null) =>
            {
                var result = await service.GetDoctorProductivityAsync(doctorId, clinicId, specialization, startDate, endDate);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorProductivity");

            group.MapGet("/statistics/doctor-workload", async (
                QueryService service,
                int? doctorId = null,
                int? hospitalId = null,
                string? specialization = null) =>
            {
                var result = await service.GetDoctorWorkloadAsync(doctorId, hospitalId, specialization);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetDoctorWorkload");

            group.MapGet("/operations", async (
                QueryService service,
                int? hospitalId = null,
                int? clinicId = null,
                int? doctorId = null,
                DateTime? startDate = null,
                DateTime? endDate = null) =>
            {
                var result = await service.GetOperationsAsync(hospitalId, clinicId, doctorId, startDate, endDate);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetOperations");

            group.MapGet("/statistics/laboratory-productivity", async (
                QueryService service,
                int? institutionId = null,
                DateTime? startDate = null,
                DateTime? endDate = null) =>
            {
                var result = await service.GetLaboratoryProductivityAsync(institutionId, startDate, endDate);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GetLaboratoryProductivity");

            group.MapPost("/documents/sick-leave", async (
                DocumentService service,
                int patientId,
                int doctorId,
                DateTime startDate,
                DateTime endDate,
                string diagnosis) =>
            {
                var result = await service.IssueSickLeaveAsync(patientId, doctorId, startDate, endDate, diagnosis);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("IssueSickLeave");

            group.MapPost("/documents/certificate", async (
                DocumentService service,
                int patientId,
                int doctorId) =>
            {
                var result = await service.IssueMedicalCertificateAsync(patientId, doctorId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("IssueCertificate");

            group.MapGet("/documents/schedule", async (
                DocumentService service,
                int clinicId) =>
            {
                var result = await service.GenerateClinicScheduleAsync(clinicId);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            }).WithName("GenerateSchedule");
        }
    }
}