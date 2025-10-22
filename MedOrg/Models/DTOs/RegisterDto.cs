using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.DTOs
{
    /// <summary>
    /// DTO для регистрации
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите роль")]
        public string RoleName { get; set; } = string.Empty;

        public int? ExistingPatientId { get; set; }
        public int? ExistingDoctorId { get; set; }
    }
}