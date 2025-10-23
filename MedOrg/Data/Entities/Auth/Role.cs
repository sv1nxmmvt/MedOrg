using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Auth
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}