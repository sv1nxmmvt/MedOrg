using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.DTOs.Auth
{
    /// <summary>
    /// DTO для обновления токена
    /// </summary>
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}