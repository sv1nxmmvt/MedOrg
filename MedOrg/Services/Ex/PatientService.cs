using MedOrg.Data;
using Microsoft.EntityFrameworkCore;
using MedOrg.Data.DTOs;
using MedOrg.Data.Entities.Patients;

namespace MedOrg.Services.Ex
{
    public class PatientService
    {
        private readonly MedOrgDbContext _context;

        public PatientService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<Patient>>> GetAllAsync()
        {
            try
            {
                var patients = await _context.Patients.ToListAsync();

                return new QueryResult<List<Patient>>
                {
                    Success = true,
                    Data = patients,
                    TotalCount = patients.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<Patient>>
                {
                    Success = false,
                    Message = $"Ошибка получения списка пациентов: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Patient>> GetByIdAsync(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);

                if (patient == null)
                {
                    return new QueryResult<Patient>
                    {
                        Success = false,
                        Message = "Пациент не найден"
                    };
                }

                return new QueryResult<Patient>
                {
                    Success = true,
                    Data = patient
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Patient>
                {
                    Success = false,
                    Message = $"Ошибка получения пациента: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Patient>> CreateAsync(Patient patient)
        {
            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return new QueryResult<Patient>
                {
                    Success = true,
                    Data = patient,
                    Message = "Пациент успешно добавлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Patient>
                {
                    Success = false,
                    Message = $"Ошибка добавления пациента: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Patient>> UpdateAsync(Patient patient)
        {
            try
            {
                var existing = await _context.Patients.FindAsync(patient.Id);
                if (existing == null)
                {
                    return new QueryResult<Patient>
                    {
                        Success = false,
                        Message = "Пациент не найден"
                    };
                }

                existing.FirstName = patient.FirstName;
                existing.LastName = patient.LastName;
                existing.MiddleName = patient.MiddleName;
                existing.BirthDate = patient.BirthDate;
                existing.Address = patient.Address;
                existing.Phone = patient.Phone;
                existing.InsuranceNumber = patient.InsuranceNumber;

                await _context.SaveChangesAsync();

                return new QueryResult<Patient>
                {
                    Success = true,
                    Data = existing,
                    Message = "Пациент успешно обновлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Patient>
                {
                    Success = false,
                    Message = $"Ошибка обновления пациента: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Пациент не найден"
                    };
                }

                var hasRecords = await _context.PatientHospitalizations.AnyAsync(h => h.PatientId == id) ||
                                 await _context.PatientDoctors.AnyAsync(v => v.PatientId == id);

                if (hasRecords)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Невозможно удалить пациента, так как у него есть медицинские записи"
                    };
                }

                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();

                return new QueryResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Пациент успешно удален"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<bool>
                {
                    Success = false,
                    Message = $"Ошибка удаления пациента: {ex.Message}"
                };
            }
        }
    }
}