using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.DTOs.Auth
{
    /// <summary>
    /// DTO для смены пароля
    /// </summary>
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}