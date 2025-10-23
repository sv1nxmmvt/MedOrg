using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Institutions
{
    public class HospitalBuilding
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; } = null!;

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}