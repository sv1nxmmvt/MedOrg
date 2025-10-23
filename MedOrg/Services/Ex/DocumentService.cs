using Microsoft.EntityFrameworkCore;
using MedOrg.Data;
using MedOrg.Models.Entities.Patients;
using MedOrg.Models.Entities.MedStaff;
using MedOrg.Models.DTOs.Patients;
using MedOrg.Data.Models.DTOs;
using MedOrg.Data.Models.DTOs.Institutions;
using MedOrg.Data.Models.DTOs.MedStaff;
using MedOrg.Data.Models.DTOs.Patients;
using MedOrg.Data.Models.Entities.Institutions;

namespace MedOrg.Services.Ex
{
    public class DocumentService
    {
        private readonly MedOrgDbContext _context;

        public DocumentService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<SickLeaveDto>> IssueSickLeaveAsync(
            int patientId,
            int doctorId,
            DateTime startDate,
            DateTime endDate,
            string diagnosis)
        {
            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == patientId);

                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Id == doctorId);

                if (patient == null || doctor == null)
                {
                    return new QueryResult<SickLeaveDto>
                    {
                        Success = false,
                        Message = "Пациент или врач не найден"
                    };
                }

                var documentNumber = $"БЛ-{DateTime.UtcNow:yyyyMMdd}-{patientId:D6}";

                var sickLeave = new SickLeave
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Diagnosis = diagnosis,
                    DocumentNumber = documentNumber
                };

                _context.SickLeaves.Add(sickLeave);
                await _context.SaveChangesAsync();

                var dto = new SickLeaveDto
                {
                    DocumentNumber = documentNumber,
                    PatientFullName = $"{patient.LastName} {patient.FirstName} {patient.MiddleName}",
                    StartDate = startDate,
                    EndDate = endDate,
                    Diagnosis = diagnosis,
                    DoctorFullName = $"{doctor.LastName} {doctor.FirstName} {doctor.MiddleName}",
                    IssueDate = DateTime.UtcNow
                };

                return new QueryResult<SickLeaveDto>
                {
                    Success = true,
                    Data = dto,
                    Message = "Больничный лист успешно выдан"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<SickLeaveDto>
                {
                    Success = false,
                    Message = $"Ошибка при выдаче больничного листа: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<MedicalCertificateDto>> IssueMedicalCertificateAsync(
            int patientId,
            int doctorId)
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                var startOfYear = new DateTime(currentYear, 1, 1);
                var endOfYear = new DateTime(currentYear, 12, 31);

                var patient = await _context.Patients
                    .Include(p => p.MedicalRecords.Where(mr => mr.RecordDate >= startOfYear && mr.RecordDate <= endOfYear))
                    .FirstOrDefaultAsync(p => p.Id == patientId);

                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Id == doctorId);

                if (patient == null || doctor == null)
                {
                    return new QueryResult<MedicalCertificateDto>
                    {
                        Success = false,
                        Message = "Пациент или врач не найден"
                    };
                }

                var documentNumber = $"СПР-{DateTime.UtcNow:yyyyMMdd}-{patientId:D6}";

                var visits = patient.MedicalRecords
                    .Select(mr => new VisitInfo
                    {
                        VisitDate = mr.RecordDate,
                        Reason = mr.Symptoms,
                        Diagnosis = mr.Diagnosis
                    })
                    .OrderBy(v => v.VisitDate)
                    .ToList();

                var content = $"Справка выдана пациенту {patient.LastName} {patient.FirstName} {patient.MiddleName} " +
                             $"о том, что он(а) обращался(лась) в поликлинику в {currentYear} году.";

                var certificate = new MedicalCertificate
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    IssueDate = DateTime.UtcNow,
                    DocumentNumber = documentNumber,
                    Content = content
                };

                _context.MedicalCertificates.Add(certificate);
                await _context.SaveChangesAsync();

                var dto = new MedicalCertificateDto
                {
                    DocumentNumber = documentNumber,
                    PatientFullName = $"{patient.LastName} {patient.FirstName} {patient.MiddleName}",
                    IssueDate = DateTime.UtcNow,
                    Visits = visits,
                    DoctorFullName = $"{doctor.LastName} {doctor.FirstName} {doctor.MiddleName}"
                };

                return new QueryResult<MedicalCertificateDto>
                {
                    Success = true,
                    Data = dto,
                    Message = "Справка успешно выдана"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<MedicalCertificateDto>
                {
                    Success = false,
                    Message = $"Ошибка при выдаче справки: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorScheduleDto>>> GenerateClinicScheduleAsync(int clinicId)
        {
            try
            {
                var doctors = await _context.Doctors
                    .Include(d => d.Institution)
                    .Where(d => d.InstitutionId == clinicId && d.Institution is Clinic)
                    .ToListAsync();

                var schedules = doctors.Select(d => new DoctorScheduleDto
                {
                    DoctorName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                    Specialization = d.Specialization,
                    OfficeNumber = "101",
                    Schedule = GenerateDefaultSchedule()
                }).ToList();

                return new QueryResult<List<DoctorScheduleDto>>
                {
                    Success = true,
                    Data = schedules,
                    TotalCount = schedules.Count,
                    Message = "Расписание успешно сформировано"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorScheduleDto>>
                {
                    Success = false,
                    Message = $"Ошибка при формировании расписания: {ex.Message}"
                };
            }
        }

        private List<ScheduleEntry> GenerateDefaultSchedule()
        {
            return new List<ScheduleEntry>
            {
                new() { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                new() { DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                new() { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                new() { DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                new() { DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) }
            };
        }
    }
}