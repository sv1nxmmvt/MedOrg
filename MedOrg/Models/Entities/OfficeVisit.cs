using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public class OfficeVisit
    {
        [Key]
        public int Id { get; set; }

        public int OfficeId { get; set; }
        public ClinicsOffice Office { get; set; } = null!;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime VisitDate { get; set; }
    }
}