using MedOrg.Data;
using MedOrg.Models.Entities;
using MedOrg.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedOrg.Services
{
    public class HospitalService
    {
        private readonly MedOrgDbContext _context;

        public HospitalService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<Hospital>>> GetAllAsync()
        {
            try
            {
                var hospitals = await _context.Hospitals
                    .Include(h => h.Buildings)
                        .ThenInclude(b => b.Departments)
                    .ToListAsync();

                return new QueryResult<List<Hospital>>
                {
                    Success = true,
                    Data = hospitals,
                    TotalCount = hospitals.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<Hospital>>
                {
                    Success = false,
                    Message = $"Ошибка получения списка больниц: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Hospital>> GetByIdAsync(int id)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .Include(h => h.Buildings)
                        .ThenInclude(b => b.Departments)
                            .ThenInclude(d => d.Wards)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (hospital == null)
                {
                    return new QueryResult<Hospital>
                    {
                        Success = false,
                        Message = "Больница не найдена"
                    };
                }

                return new QueryResult<Hospital>
                {
                    Success = true,
                    Data = hospital
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Hospital>
                {
                    Success = false,
                    Message = $"Ошибка получения больницы: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Hospital>> CreateAsync(Hospital hospital)
        {
            try
            {
                _context.Hospitals.Add(hospital);
                await _context.SaveChangesAsync();

                return new QueryResult<Hospital>
                {
                    Success = true,
                    Data = hospital,
                    Message = "Больница успешно добавлена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Hospital>
                {
                    Success = false,
                    Message = $"Ошибка добавления больницы: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<Hospital>> UpdateAsync(Hospital hospital)
        {
            try
            {
                var existing = await _context.Hospitals.FindAsync(hospital.Id);
                if (existing == null)
                {
                    return new QueryResult<Hospital>
                    {
                        Success = false,
                        Message = "Больница не найдена"
                    };
                }

                existing.Name = hospital.Name;
                existing.Address = hospital.Address;

                await _context.SaveChangesAsync();

                return new QueryResult<Hospital>
                {
                    Success = true,
                    Data = existing,
                    Message = "Больница успешно обновлена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<Hospital>
                {
                    Success = false,
                    Message = $"Ошибка обновления больницы: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .Include(h => h.Buildings)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (hospital == null)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Больница не найдена"
                    };
                }

                if (hospital.Buildings.Any())
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Невозможно удалить больницу, так как у неё есть корпуса"
                    };
                }

                _context.Hospitals.Remove(hospital);
                await _context.SaveChangesAsync();

                return new QueryResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Больница успешно удалена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<bool>
                {
                    Success = false,
                    Message = $"Ошибка удаления больницы: {ex.Message}"
                };
            }
        }
    }
}