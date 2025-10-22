using MedOrg.Data;
using MedOrg.Models.Entities;
using MedOrg.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedOrg.Services
{
    public class SupportStaffService
    {
        private readonly MedOrgDbContext _context;

        public SupportStaffService(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<List<SupportStaff>>> GetAllAsync()
        {
            try
            {
                var staff = await _context.SupportStaff
                    .Include(s => s.Institution)
                    .ToListAsync();

                return new QueryResult<List<SupportStaff>>
                {
                    Success = true,
                    Data = staff,
                    TotalCount = staff.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<SupportStaff>>
                {
                    Success = false,
                    Message = $"Ошибка получения списка персонала: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<SupportStaff>> GetByIdAsync(int id)
        {
            try
            {
                var staff = await _context.SupportStaff
                    .Include(s => s.Institution)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (staff == null)
                {
                    return new QueryResult<SupportStaff>
                    {
                        Success = false,
                        Message = "Сотрудник не найден"
                    };
                }

                return new QueryResult<SupportStaff>
                {
                    Success = true,
                    Data = staff
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<SupportStaff>
                {
                    Success = false,
                    Message = $"Ошибка получения сотрудника: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<SupportStaff>> CreateAsync(SupportStaff staff)
        {
            try
            {
                _context.SupportStaff.Add(staff);
                await _context.SaveChangesAsync();

                return new QueryResult<SupportStaff>
                {
                    Success = true,
                    Data = staff,
                    Message = "Сотрудник успешно добавлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<SupportStaff>
                {
                    Success = false,
                    Message = $"Ошибка добавления сотрудника: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<SupportStaff>> UpdateAsync(SupportStaff staff)
        {
            try
            {
                var existing = await _context.SupportStaff.FindAsync(staff.Id);
                if (existing == null)
                {
                    return new QueryResult<SupportStaff>
                    {
                        Success = false,
                        Message = "Сотрудник не найден"
                    };
                }

                existing.FirstName = staff.FirstName;
                existing.LastName = staff.LastName;
                existing.MiddleName = staff.MiddleName;
                existing.Position = staff.Position;
                existing.InstitutionId = staff.InstitutionId;
                existing.BirthDate = staff.BirthDate;
                existing.HireDate = staff.HireDate;
                existing.Phone = staff.Phone;
                existing.BaseSalary = staff.BaseSalary;

                await _context.SaveChangesAsync();

                return new QueryResult<SupportStaff>
                {
                    Success = true,
                    Data = existing,
                    Message = "Сотрудник успешно обновлен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<SupportStaff>
                {
                    Success = false,
                    Message = $"Ошибка обновления сотрудника: {ex.Message}"
                };
            }
        }

        public async Task<QueryResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var staff = await _context.SupportStaff.FindAsync(id);
                if (staff == null)
                {
                    return new QueryResult<bool>
                    {
                        Success = false,
                        Message = "Сотрудник не найден"
                    };
                }

                _context.SupportStaff.Remove(staff);
                await _context.SaveChangesAsync();

                return new QueryResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Сотрудник успешно удален"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<bool>
                {
                    Success = false,
                    Message = $"Ошибка удаления сотрудника: {ex.Message}"
                };
            }
        }
    }
}