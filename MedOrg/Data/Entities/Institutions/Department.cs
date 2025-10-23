using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Institutions
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Specialization { get; set; } = string.Empty;

        public int BuildingId { get; set; }
        public HospitalBuilding Building { get; set; } = null!;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}