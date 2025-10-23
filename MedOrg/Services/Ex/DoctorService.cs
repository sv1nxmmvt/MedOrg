using MedOrg.Data;
using Microsoft.EntityFrameworkCore;
using MedOrg.Data.Models.DTOs;
using MedOrg.Data.Models.DTOs.MedStaff;
using MedOrg.Data.Models.Entities.MedStaff;

namespace MedOrg.Services.Ex
{
    public class DoctorService
    {
        private readonly MedOrgDbContext _context;

        public DoctorService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<DoctorDto>>> GetAllAsync()
        {
            try
            {
                var doctors = await _context.Doctors
                    .Include(d => d.Institution)
                    .Include(d => d.SecondaryInstitution)
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = $"{d.LastName} {d.FirstName} {d.MiddleName}",
                        Specialization = d.Specialization,
                        YearsOfExperience = d.YearsOfExperience,
                        Degree = d.Degree.HasValue ? d.Degree.Value.ToString() : null,
                        Title = d.Title.HasValue ? d.Title.Value.ToString() : null,
                        TotalOperations = d.TotalOperations,
                        FatalOperations = d.FatalOperations,
                        HazardPayCoefficient = d.HazardPayCoefficient,
                        ExtendedVacation = d.ExtraVacationDays.HasValue && d.ExtraVacationDays > 0,
                        InstitutionName = d.Institution != null ? d.Institution.Name : "Не указано",
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
                    Message = $"Ошибка получения списка врачей: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Doctor>> GetByIdAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Institution)
                    .Include(d => d.SecondaryInstitution)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null)
                {
                    return new QueryResult<Doctor>
                    {
                        Success = false,
                        Message = "Врач не найден"
                    };
                }

                return new QueryResult<Doctor>
                {
                    Success = true,
                    Data = doctor
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Doctor>
                {
                    Success = false,
                    Message = $"Ошибка получения врача: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Doctor>> CreateAsync(Doctor doctor)
        {
            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return new QueryResult<Doctor>
                {
                    Success = true,
                    Data = doctor,
                    Message = "Врач успешно добавлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Doctor>
                {
                    Success = false,
                    Message = $"Ошибка добавления врача: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Doctor>> UpdateAsync(Doctor doctor)
        {
            try
            {
                var existing = await _context.Doctors.FindAsync(doctor.Id);
                if (existing == null)
                {
                    return new QueryResult<Doctor>
                    {
                        Success = false,
                        Message = "Врач не найден"
                    };
                }

                existing.FirstName = doctor.FirstName;
                existing.LastName = doctor.LastName;
                existing.MiddleName = doctor.MiddleName;
                existing.Specialization = doctor.Specialization;
                existing.Degree = doctor.Degree;
                existing.Title = doctor.Title;
                existing.TotalOperations = doctor.TotalOperations;
                existing.FatalOperations = doctor.FatalOperations;
                existing.HazardPayCoefficient = doctor.HazardPayCoefficient;
                existing.ExtraVacationDays = doctor.ExtraVacationDays;
                existing.InstitutionId = doctor.InstitutionId;
                existing.SecondaryInstitutionId = doctor.SecondaryInstitutionId;

                await _context.SaveChangesAsync();

                return new QueryResult<Doctor>
                {
                    Success = true,
                    Data = existing,
                    Message = "Врач успешно обновлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Doctor>
                {
                    Success = false,
                    Message = $"Ошибка обновления врача: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Врач не найден"
                    };
                }

                var hasPatients = await _context.PatientHospitalizations
                    .AnyAsync(h => h.AttendingDoctorId == id);

                if (hasPatients)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Невозможно удалить врача, так как за ним закреплены пациенты"
                    };
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                return new QueryResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Врач успешно удален"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<bool>
                {
                    Success = false,
                    Message = $"Ошибка удаления врача: {ex.Message}"
                };
            }
        }
    }
}