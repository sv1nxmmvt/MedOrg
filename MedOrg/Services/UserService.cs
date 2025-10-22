using MedOrg.Data;
using MedOrg.Models.DTOs;
using MedOrg.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedOrg.Services
{
    /// <summary>
    /// Сервис для управления пользователями (CRUD операции)
    /// </summary>
    public class UserService
    {
        private readonly MedOrgDbContext _context;

        public UserService(MedOrgDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        public async Task<QueryResult<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Patient)
                    .Include(u => u.Doctor)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        RoleName = u.Role.Name,
                        PatientId = u.PatientId,
                        DoctorId = u.DoctorId,
                        CreatedAt = u.CreatedAt,
                        LastLoginAt = u.LastLoginAt
                    })
                    .ToListAsync();

                return new QueryResult<List<UserDto>>
                {
                    Success = true,
                    Data = users,
                    TotalCount = users.Count
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<UserDto>>
                {
                    Success = false,
                    Message = $"Ошибка получения пользователей: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        public async Task<QueryResult<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Patient)
                    .Include(u => u.Doctor)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new QueryResult<UserDto>
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                return new QueryResult<UserDto>
                {
                    Success = true,
                    Data = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        RoleName = user.Role.Name,
                        PatientId = user.PatientId,
                        DoctorId = user.DoctorId,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<UserDto>
                {
                    Success = false,
                    Message = $"Ошибка получения пользователя: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Обновление роли пользователя (только для администратора)
        /// </summary>
        public async Task<QueryResult> UpdateUserRoleAsync(int userId, string newRoleName)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRoleName);
                if (role == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Роль не найдена"
                    };
                }

                user.RoleId = role.Id;
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Роль пользователя успешно обновлена"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка обновления роли: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Деактивация пользователя
        /// </summary>
        public async Task<QueryResult> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                user.IsActive = false;
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пользователь деактивирован"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка деактивации: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Активация пользователя
        /// </summary>
        public async Task<QueryResult> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                user.IsActive = true;
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пользователь активирован"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка активации: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public async Task<QueryResult> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId)
                    .ToListAsync();

                _context.RefreshTokens.RemoveRange(tokens);
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пользователь удален"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка удаления: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Связывание пользователя с пациентом
        /// </summary>
        public async Task<QueryResult> LinkUserToPatientAsync(int userId, int patientId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                var patient = await _context.Patients.FindAsync(patientId);
                if (patient == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пациент не найден"
                    };
                }

                user.PatientId = patientId;
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пользователь успешно связан с пациентом"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка связывания: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Связывание пользователя с врачом
        /// </summary>
        public async Task<QueryResult> LinkUserToDoctorAsync(int userId, int doctorId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };
                }

                var doctor = await _context.Doctors.FindAsync(doctorId);
                if (doctor == null)
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Врач не найден"
                    };
                }

                user.DoctorId = doctorId;
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пользователь успешно связан с врачом"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка связывания: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Получение всех ролей
        /// </summary>
        public async Task<QueryResult<List<Role>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();

                return new QueryResult<List<Role>>
                {
                    Success = true,
                    Data = roles
                };
            }
            catch (Exception ex)
            {
                return new QueryResult<List<Role>>
                {
                    Success = false,
                    Message = $"Ошибка получения ролей: {ex.Message}"
                };
            }
        }
    }
}