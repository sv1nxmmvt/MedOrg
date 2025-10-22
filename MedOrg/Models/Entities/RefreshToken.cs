using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    /// <summary>
    /// Токен обновления для JWT
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
    }
}