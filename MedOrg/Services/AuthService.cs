using MedOrg.Configuration;
using MedOrg.Data;
using MedOrg.Models.DTOs;
using MedOrg.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MedOrg.Services
{
    public class AuthService
    {
        private readonly MedOrgDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(MedOrgDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                // Проверка существования пользователя
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Пользователь с таким логином уже существует"
                    };
                }

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Пользователь с таким email уже существует"
                    };
                }

                // Получение роли
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName);
                if (role == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Некорректная роль"
                    };
                }

                // Валидация связей
                if (dto.ExistingPatientId.HasValue)
                {
                    var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.ExistingPatientId.Value);
                    if (!patientExists)
                    {
                        return new AuthResultDto
                        {
                            Success = false,
                            Message = "Указанный пациент не найден"
                        };
                    }
                }

                if (dto.ExistingDoctorId.HasValue)
                {
                    var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == dto.ExistingDoctorId.Value);
                    if (!doctorExists)
                    {
                        return new AuthResultDto
                        {
                            Success = false,
                            Message = "Указанный врач не найден"
                        };
                    }
                }

                // Создание пользователя
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleId = role.Id,
                    PatientId = dto.ExistingPatientId,
                    DoctorId = dto.ExistingDoctorId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Загрузка навигационных свойств
                await _context.Entry(user).Reference(u => u.Role).LoadAsync();

                return new AuthResultDto
                {
                    Success = true,
                    Message = "Регистрация прошла успешно",
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = $"Ошибка регистрации: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            try
            {
                // Поиск пользователя
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == dto.Username);

                if (user == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Неверный логин или пароль"
                    };
                }

                // Проверка активности
                if (!user.IsActive)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Учетная запись деактивирована"
                    };
                }

                // Проверка пароля
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Неверный логин или пароль"
                    };
                }

                // Обновление времени последнего входа
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Генерация токенов
                var accessToken = GenerateAccessToken(user);
                var refreshToken = await GenerateRefreshTokenAsync(user.Id);

                return new AuthResultDto
                {
                    Success = true,
                    Message = "Вход выполнен успешно",
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = $"Ошибка входа: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Обновление access токена через refresh токен
        /// </summary>
        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            try
            {
                // Валидация access токена (игнорируя истечение)
                var principal = GetPrincipalFromExpiredToken(dto.Token);
                if (principal == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Некорректный токен"
                    };
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Некорректный токен"
                    };
                }

                // Проверка refresh токена
                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                        .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(rt =>
                        rt.UserId == userId &&
                        rt.Token == dto.RefreshToken &&
                        !rt.IsRevoked);

                if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Refresh токен недействителен или истек"
                    };
                }

                // Генерация новых токенов
                var newAccessToken = GenerateAccessToken(refreshToken.User);
                var newRefreshToken = await GenerateRefreshTokenAsync(userId);

                // Отзыв старого refresh токена
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();

                return new AuthResultDto
                {
                    Success = true,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = MapToUserDto(refreshToken.User)
                };
            }
            catch (Exception ex)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = $"Ошибка обновления токена: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Выход пользователя (отзыв всех refresh токенов)
        /// </summary>
        public async Task<QueryResult> LogoutAsync(int userId)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }

                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Выход выполнен успешно"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка выхода: {ex.Message}"
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
                    Data = MapToUserDto(user)
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
        /// Смена пароля
        /// </summary>
        public async Task<QueryResult> ChangePasswordAsync(int userId, ChangePasswordDto dto)
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

                // Проверка старого пароля
                if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                {
                    return new QueryResult
                    {
                        Success = false,
                        Message = "Неверный текущий пароль"
                    };
                }

                // Установка нового пароля
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return new QueryResult
                {
                    Success = true,
                    Message = "Пароль успешно изменен"
                };
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Success = false,
                    Message = $"Ошибка смены пароля: {ex.Message}"
                };
            }
        }

        #region Private Methods

        /// <summary>
        /// Генерация JWT access токена
        /// </summary>
        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("PatientId", user.PatientId?.ToString() ?? string.Empty),
                new Claim("DoctorId", user.DoctorId?.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Генерация refresh токена
        /// </summary>
        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        /// <summary>
        /// Извлечение principal из истекшего токена
        /// </summary>
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Маппинг User -> UserDto
        /// </summary>
        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleName = user.Role.Name,
                PatientId = user.PatientId,
                DoctorId = user.DoctorId,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        #endregion
    }
}