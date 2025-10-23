using MedOrg.Data;
using Microsoft.EntityFrameworkCore;
using MedOrg.Data.DTOs;
using MedOrg.Data.Entities.Institutions;

namespace MedOrg.Services.Ex
{
    public class ClinicService
    {
        private readonly MedOrgDbContext _context;

        public ClinicService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<Clinic>>> GetAllAsync()
        {
            try
            {
                var clinics = await _context.Clinics
                    .Include(c => c.AttachedHospital)
                    .Include(c => c.Offices)
                    .ToListAsync();

                return new QueryResult<List<Clinic>>
                {
                    Success = true,
                    Data = clinics,
                    TotalCount = clinics.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<Clinic>>
                {
                    Success = false,
                    Message = $"Ошибка получения списка поликлиник: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Clinic>> GetByIdAsync(int id)
        {
            try
            {
                var clinic = await _context.Clinics
                    .Include(c => c.AttachedHospital)
                    .Include(c => c.Offices)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (clinic == null)
                {
                    return new QueryResult<Clinic>
                    {
                        Success = false,
                        Message = "Поликлиника не найдена"
                    };
                }

                return new QueryResult<Clinic>
                {
                    Success = true,
                    Data = clinic
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Clinic>
                {
                    Success = false,
                    Message = $"Ошибка получения поликлиники: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Clinic>> CreateAsync(Clinic clinic)
        {
            try
            {
                if (clinic.AttachedHospitalId.HasValue)
                {
                    var hospitalExists = await _context.Hospitals
                        .AnyAsync(h => h.Id == clinic.AttachedHospitalId.Value);

                    if (!hospitalExists)
                    {
                        return new QueryResult<Clinic>
                        {
                            Success = false,
                            Message = "Указанная больница не найдена"
                        };
                    }
                }

                _context.Clinics.Add(clinic);
                await _context.SaveChangesAsync();

                return new QueryResult<Clinic>
                {
                    Success = true,
                    Data = clinic,
                    Message = "Поликлиника успешно добавлена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Clinic>
                {
                    Success = false,
                    Message = $"Ошибка добавления поликлиники: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Clinic>> UpdateAsync(Clinic clinic)
        {
            try
            {
                var existing = await _context.Clinics.FindAsync(clinic.Id);
                if (existing == null)
                {
                    return new QueryResult<Clinic>
                    {
                        Success = false,
                        Message = "Поликлиника не найдена"
                    };
                }

                existing.Name = clinic.Name;
                existing.Address = clinic.Address;
                existing.AttachedHospitalId = clinic.AttachedHospitalId;

                await _context.SaveChangesAsync();

                return new QueryResult<Clinic>
                {
                    Success = true,
                    Data = existing,
                    Message = "Поликлиника успешно обновлена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Clinic>
                {
                    Success = false,
                    Message = $"Ошибка обновления поликлиники: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var clinic = await _context.Clinics
                    .Include(c => c.Offices)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (clinic == null)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Поликлиника не найдена"
                    };
                }

                if (clinic.Offices.Any())
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Невозможно удалить поликлинику, так как у неё есть кабинеты"
                    };
                }

                _context.Clinics.Remove(clinic);
                await _context.SaveChangesAsync();

                return new QueryResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Поликлиника успешно удалена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<bool>
                {
                    Success = false,
                    Message = $"Ошибка удаления поликлиники: {ex.Message}"
                };
            }
        }
    }
}