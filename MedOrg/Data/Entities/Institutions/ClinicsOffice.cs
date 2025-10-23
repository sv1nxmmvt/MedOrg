using MedOrg.Data.Entities.Patients;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Institutions
{
    public class ClinicsOffice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Number { get; set; } = string.Empty;

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;

        public ICollection<OfficeVisit> Visits { get; set; } = new List<OfficeVisit>();
    }
}