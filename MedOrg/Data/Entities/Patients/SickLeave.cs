using MedOrg.Data.Entities.MedStaff;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Patients
{
    public class SickLeave
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Diagnosis { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;
    }
}