using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public class Ward
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Number { get; set; } = string.Empty;

        public int TotalBeds { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ICollection<PatientHospitalization> CurrentPatients { get; set; } = new List<PatientHospitalization>();
    }
}