using Microsoft.EntityFrameworkCore;
using MedOrg.Data;
using MedOrg.Data.Models.DTOs;
using MedOrg.Data.Models.DTOs.Institutions;
using MedOrg.Data.Models.DTOs.MedStaff;
using MedOrg.Data.Models.DTOs.Operations;
using MedOrg.Data.Models.DTOs.Patients;
using MedOrg.Data.Models.Entities.Institutions;
using MedOrg.Data.Models.Entities.MedStaff;

namespace MedOrg.Services
{
    public class QueryService
    {
        private readonly MedOrgDbContext _context;

        public QueryService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<DoctorDto>>> GetDoctorsBySpecializationAsync(
            string specialization,
            int? institutionId = null,
            string? institutionType = null)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.Institution)
                    .Include(d => d.SecondaryInstitution)
                    .Where(d => d.Specialization == specialization);

                if (institutionId.HasValue)
                {
                    query = query.Where(d => d.InstitutionId == institutionId.Value);
                }
                else if (!string.IsNullOrEmpty(institutionType))
                {
                    if (institutionType == "Hospital")
                        query = query.Where(d => d.Institution is Hospital);
                    else if (institutionType == "Clinic")
                        query = query.Where(d => d.Institution is Clinic);
                }

                var doctors = await query
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                        Specialization = d.Specialization,
                        Degree = d.Degree.HasValue ? d.Degree.ToString() : null,
                        Title = d.Title.HasValue ? d.Title.ToString() : null,
                        TotalOperations = d.TotalOperations,
                        YearsOfExperience = DateTime.UtcNow.Year - d.HireDate.Year,
                        InstitutionName = d.Institution.Name,
                        SecondaryInstitutionName = d.SecondaryInstitution != null ? d.SecondaryInstitution.Name : null
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorDto>>
                {
                    Success = true,
                    Data = doctors,
                    TotalCount = doctors.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorDto>>
                {
                    Success = false,
                    Message = $"Ошибка при получении врачей: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<SupportStaffDto>>> GetSupportStaffByPositionAsync(
            string position,
            int? institutionId = null,
            string? institutionType = null)
        {
            try
            {
                var query = _context.SupportStaff
                    .Include(s => s.Institution)
                    .Where(s => s.Position == position);

                if (institutionId.HasValue)
                {
                    query = query.Where(s => s.InstitutionId == institutionId.Value);
                }
                else if (!string.IsNullOrEmpty(institutionType))
                {
                    if (institutionType == "Hospital")
                        query = query.Where(s => s.Institution is Hospital);
                    else if (institutionType == "Clinic")
                        query = query.Where(s => s.Institution is Clinic);
                }

                var staff = await query
                    .Select(s => new SupportStaffDto
                    {
                        Id = s.Id,
                        FullName = $"{s.LastName} {s.FirstName} {s.MiddleName}",
                        Position = s.Position,
                        YearsOfExperience = DateTime.UtcNow.Year - s.HireDate.Year,
                        InstitutionName = s.Institution.Name
                    })
                    .ToListAsync();

                return new QueryResult<List<SupportStaffDto>>
                {
                    Success = true,
                    Data = staff,
                    TotalCount = staff.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<SupportStaffDto>>
                {
                    Success = false,
                    Message = $"Ошибка при получении персонала: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorDto>>> GetDoctorsByMinOperationsAsync(
            string specialization,
            int minOperations,
            int? institutionId = null,
            string? institutionType = null)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.Institution)
                    .Where(d => d.Specialization == specialization &&
                               d.TotalOperations.HasValue &&
                               d.TotalOperations >= minOperations);

                if (institutionId.HasValue)
                {
                    query = query.Where(d => d.InstitutionId == institutionId.Value);
                }
                else if (!string.IsNullOrEmpty(institutionType))
                {
                    if (institutionType == "Hospital")
                        query = query.Where(d => d.Institution is Hospital);
                    else if (institutionType == "Clinic")
                        query = query.Where(d => d.Institution is Clinic);
                }

                var doctors = await query
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                        Specialization = d.Specialization,
                        TotalOperations = d.TotalOperations,
                        YearsOfExperience = DateTime.UtcNow.Year - d.HireDate.Year,
                        InstitutionName = d.Institution.Name
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorDto>>
                {
                    Success = true,
                    Data = doctors,
                    TotalCount = doctors.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorDto>>> GetDoctorsByMinExperienceAsync(
            string specialization,
            int minYears,
            int? institutionId = null,
            string? institutionType = null)
        {
            try
            {
                var minDate = DateTime.UtcNow.AddYears(-minYears);

                var query = _context.Doctors
                    .Include(d => d.Institution)
                    .Where(d => d.Specialization == specialization && d.HireDate <= minDate);

                if (institutionId.HasValue)
                {
                    query = query.Where(d => d.InstitutionId == institutionId.Value);
                }
                else if (!string.IsNullOrEmpty(institutionType))
                {
                    if (institutionType == "Hospital")
                        query = query.Where(d => d.Institution is Hospital);
                    else if (institutionType == "Clinic")
                        query = query.Where(d => d.Institution is Clinic);
                }

                var doctors = await query
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                        Specialization = d.Specialization,
                        YearsOfExperience = DateTime.UtcNow.Year - d.HireDate.Year,
                        InstitutionName = d.Institution.Name
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorDto>>
                {
                    Success = true,
                    Data = doctors,
                    TotalCount = doctors.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorDto>>> GetDoctorsByDegreeAndTitleAsync(
            string specialization,
            AcademicDegree? degree = null,
            AcademicTitle? title = null,
            int? institutionId = null,
            string? institutionType = null)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.Institution)
                    .Where(d => d.Specialization == specialization);

                if (degree.HasValue)
                    query = query.Where(d => d.Degree == degree);

                if (title.HasValue)
                    query = query.Where(d => d.Title == title);

                if (institutionId.HasValue)
                {
                    query = query.Where(d => d.InstitutionId == institutionId.Value);
                }
                else if (!string.IsNullOrEmpty(institutionType))
                {
                    if (institutionType == "Hospital")
                        query = query.Where(d => d.Institution is Hospital);
                    else if (institutionType == "Clinic")
                        query = query.Where(d => d.Institution is Clinic);
                }

                var doctors = await query
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                        Specialization = d.Specialization,
                        Degree = d.Degree.HasValue ? d.Degree.ToString() : null,
                        Title = d.Title.HasValue ? d.Title.ToString() : null,
                        YearsOfExperience = DateTime.UtcNow.Year - d.HireDate.Year,
                        InstitutionName = d.Institution.Name
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorDto>>
                {
                    Success = true,
                    Data = doctors,
                    TotalCount = doctors.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<HospitalizedPatientDto>>> GetHospitalizedPatientsAsync(
            int? hospitalId = null,
            int? departmentId = null,
            int? wardId = null)
        {
            try
            {
                var query = _context.PatientHospitalizations
                    .Include(ph => ph.Patient)
                    .Include(ph => ph.AttendingDoctor)
                    .Include(ph => ph.Ward)
                        .ThenInclude(w => w.Department)
                    .Where(ph => ph.DischargeDate == null);

                if (wardId.HasValue)
                    query = query.Where(ph => ph.WardId == wardId.Value);
                else if (departmentId.HasValue)
                    query = query.Where(ph => ph.Ward.DepartmentId == departmentId.Value);
                else if (hospitalId.HasValue)
                    query = query.Where(ph => ph.HospitalId == hospitalId.Value);

                var patients = await query
                    .Select(ph => new HospitalizedPatientDto
                    {
                        Id = ph.Patient.Id,
                        FullName = $"{ph.Patient.LastName} {ph.Patient.FirstName} {ph.Patient.MiddleName}",
                        AdmissionDate = ph.AdmissionDate,
                        Condition = ph.Condition,
                        Temperature = ph.Temperature,
                        AttendingDoctorName = $"{ph.AttendingDoctor.LastName} {ph.AttendingDoctor.FirstName}",
                        WardNumber = ph.Ward.Number,
                        DepartmentName = ph.Ward.Department.Name,
                        Diagnosis = ph.Diagnosis
                    })
                    .ToListAsync();

                return new QueryResult<List<HospitalizedPatientDto>>
                {
                    Success = true,
                    Data = patients,
                    TotalCount = patients.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<HospitalizedPatientDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<HospitalizedPatientDto>>> GetDischargedPatientsAsync(
            int? hospitalId = null,
            int? doctorId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var query = _context.PatientHospitalizations
                    .Include(ph => ph.Patient)
                    .Include(ph => ph.AttendingDoctor)
                    .Include(ph => ph.Ward)
                        .ThenInclude(w => w.Department)
                    .Include(ph => ph.Hospital)
                    .Where(ph => ph.DischargeDate != null);

                if (hospitalId.HasValue)
                    query = query.Where(ph => ph.HospitalId == hospitalId.Value);

                if (doctorId.HasValue)
                    query = query.Where(ph => ph.AttendingDoctorId == doctorId.Value);

                if (startDate.HasValue)
                    query = query.Where(ph => ph.DischargeDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(ph => ph.DischargeDate <= endDate.Value);

                var patients = await query
                    .Select(ph => new HospitalizedPatientDto
                    {
                        Id = ph.Patient.Id,
                        FullName = $"{ph.Patient.LastName} {ph.Patient.FirstName} {ph.Patient.MiddleName}",
                        AdmissionDate = ph.AdmissionDate,
                        Condition = ph.Condition,
                        Temperature = ph.Temperature,
                        AttendingDoctorName = $"{ph.AttendingDoctor.LastName} {ph.AttendingDoctor.FirstName}",
                        WardNumber = ph.Ward.Number,
                        DepartmentName = ph.Ward.Department.Name,
                        Diagnosis = ph.Diagnosis
                    })
                    .ToListAsync();

                return new QueryResult<List<HospitalizedPatientDto>>
                {
                    Success = true,
                    Data = patients,
                    TotalCount = patients.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<HospitalizedPatientDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<ClinicPatientDto>>> GetClinicPatientsByDoctorAsync(
            string specialization,
            int clinicId)
        {
            try
            {
                var patients = await _context.PatientDoctors
                    .Include(pd => pd.Patient)
                    .Include(pd => pd.Doctor)
                    .Where(pd => pd.Doctor.Specialization == specialization &&
                                pd.Doctor.InstitutionId == clinicId &&
                                pd.EndDate == null)
                    .Select(pd => new ClinicPatientDto
                    {
                        Id = pd.Patient.Id,
                        FullName = $"{pd.Patient.LastName} {pd.Patient.FirstName} {pd.Patient.MiddleName}",
                        DoctorName = $"{pd.Doctor.LastName} {pd.Doctor.FirstName}",
                        DoctorSpecialization = pd.Doctor.Specialization,
                        StartDate = pd.StartDate
                    })
                    .ToListAsync();

                return new QueryResult<List<ClinicPatientDto>>
                {
                    Success = true,
                    Data = patients,
                    TotalCount = patients.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<ClinicPatientDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<WardStatisticsDto>>> GetWardStatisticsAsync(int hospitalId)
        {
            try
            {
                var departments = await _context.Departments
                    .Include(d => d.Wards)
                        .ThenInclude(w => w.CurrentPatients)
                    .Include(d => d.Building)
                    .Where(d => d.Building.HospitalId == hospitalId)
                    .ToListAsync();

                var statistics = departments.Select(d => new WardStatisticsDto
                {
                    DepartmentName = d.Name,
                    TotalWards = d.Wards.Count,
                    TotalBeds = d.Wards.Sum(w => w.TotalBeds),
                    OccupiedBeds = d.Wards.Sum(w => w.CurrentPatients.Count(p => p.DischargeDate == null)),
                    FreeBeds = d.Wards.Sum(w => w.TotalBeds) - d.Wards.Sum(w => w.CurrentPatients.Count(p => p.DischargeDate == null)),
                    FullyFreeWards = d.Wards.Count(w => !w.CurrentPatients.Any(p => p.DischargeDate == null))
                }).ToList();

                return new QueryResult<List<WardStatisticsDto>>
                {
                    Success = true,
                    Data = statistics,
                    TotalCount = statistics.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<WardStatisticsDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<OfficeStatisticsDto>>> GetOfficeStatisticsAsync(
            int clinicId,
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                var statistics = await _context.ClinicsOffices
                    .Where(o => o.ClinicId == clinicId)
                    .Select(o => new OfficeStatisticsDto
                    {
                        OfficeNumber = o.Number,
                        VisitsCount = o.Visits.Count(v => v.VisitDate >= startDate && v.VisitDate <= endDate)
                    })
                    .ToListAsync();

                return new QueryResult<List<OfficeStatisticsDto>>
                {
                    Success = true,
                    Data = statistics,
                    TotalCount = statistics.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<OfficeStatisticsDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorProductivityDto>>> GetDoctorProductivityAsync(
            int? doctorId = null,
            int? clinicId = null,
            string? specialization = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;
                var days = (end - start).Days + 1;

                var query = _context.Doctors
                    .Include(d => d.OfficeVisits)
                    .AsQueryable();

                if (doctorId.HasValue)
                    query = query.Where(d => d.Id == doctorId.Value);
                else if (clinicId.HasValue)
                    query = query.Where(d => d.InstitutionId == clinicId.Value);
                else if (!string.IsNullOrEmpty(specialization))
                    query = query.Where(d => d.Specialization == specialization);

                var productivity = await query
                    .Select(d => new DoctorProductivityDto
                    {
                        DoctorName = $"{d.LastName} {d.FirstName}",
                        Specialization = d.Specialization,
                        TotalPatients = d.OfficeVisits.Count(v => v.VisitDate >= start && v.VisitDate <= end),
                        AveragePatientsPerDay = d.OfficeVisits.Count(v => v.VisitDate >= start && v.VisitDate <= end) / (decimal)days
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorProductivityDto>>
                {
                    Success = true,
                    Data = productivity,
                    TotalCount = productivity.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorProductivityDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<DoctorWorkloadDto>>> GetDoctorWorkloadAsync(
            int? doctorId = null,
            int? hospitalId = null,
            string? specialization = null)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.HospitalizedPatients)
                    .AsQueryable();

                if (doctorId.HasValue)
                    query = query.Where(d => d.Id == doctorId.Value);
                else if (hospitalId.HasValue)
                    query = query.Where(d => d.InstitutionId == hospitalId.Value);
                else if (!string.IsNullOrEmpty(specialization))
                    query = query.Where(d => d.Specialization == specialization);

                var workload = await query
                    .Select(d => new DoctorWorkloadDto
                    {
                        DoctorName = $"{d.LastName} {d.FirstName}",
                        Specialization = d.Specialization,
                        CurrentPatients = d.HospitalizedPatients.Count(p => p.DischargeDate == null)
                    })
                    .ToListAsync();

                return new QueryResult<List<DoctorWorkloadDto>>
                {
                    Success = true,
                    Data = workload,
                    TotalCount = workload.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<DoctorWorkloadDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<OperationDto>>> GetOperationsAsync(
            int? hospitalId = null,
            int? clinicId = null,
            int? doctorId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var query = _context.Operations
                    .Include(o => o.Patient)
                    .Include(o => o.Doctor)
                    .Include(o => o.Institution)
                    .AsQueryable();

                if (hospitalId.HasValue)
                    query = query.Where(o => o.InstitutionId == hospitalId.Value);
                else if (clinicId.HasValue)
                    query = query.Where(o => o.InstitutionId == clinicId.Value);

                if (doctorId.HasValue)
                    query = query.Where(o => o.DoctorId == doctorId.Value);

                if (startDate.HasValue)
                    query = query.Where(o => o.OperationDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(o => o.OperationDate <= endDate.Value);

                var operations = await query
                    .Select(o => new OperationDto
                    {
                        Id = o.Id,
                        PatientName = $"{o.Patient.LastName} {o.Patient.FirstName}",
                        DoctorName = $"{o.Doctor.LastName} {o.Doctor.FirstName}",
                        OperationDate = o.OperationDate,
                        OperationType = o.OperationType,
                        Result = o.Result.ToString(),
                        InstitutionName = o.Institution.Name
                    })
                    .ToListAsync();

                return new QueryResult<List<OperationDto>>
                {
                    Success = true,
                    Data = operations,
                    TotalCount = operations.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<OperationDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<List<LaboratoryProductivityDto>>> GetLaboratoryProductivityAsync(
            int? institutionId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;
                var days = (end - start).Days + 1;

                var query = _context.LabExaminations
                    .Include(le => le.Laboratory)
                    .Include(le => le.Institution)
                    .Where(le => le.ExaminationDate >= start && le.ExaminationDate <= end);

                if (institutionId.HasValue)
                    query = query.Where(le => le.InstitutionId == institutionId.Value);

                var productivity = await query
                    .GroupBy(le => new {
                        le.LaboratoryId,
                        LaboratoryName = le.Laboratory.Name,
                        le.InstitutionId,
                        InstitutionName = le.Institution.Name
                    })
                    .Select(g => new LaboratoryProductivityDto
                    {
                        LaboratoryName = g.Key.LaboratoryName,
                        InstitutionName = g.Key.InstitutionName,
                        TotalExaminations = g.Count(),
                        AverageExaminationsPerDay = g.Count() / (decimal)days
                    })
                    .ToListAsync();

                return new QueryResult<List<LaboratoryProductivityDto>>
                {
                    Success = true,
                    Data = productivity,
                    TotalCount = productivity.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<LaboratoryProductivityDto>>
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }
    }
}