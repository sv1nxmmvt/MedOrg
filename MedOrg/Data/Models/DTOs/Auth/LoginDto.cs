using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.DTOs.Auth
{
    /// <summary>
    /// DTO для входа
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}