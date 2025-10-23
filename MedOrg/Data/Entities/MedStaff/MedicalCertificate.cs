using MedOrg.Data.Entities.Patients;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.MedStaff
{
    public class MedicalCertificate
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime IssueDate { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;
    }
}