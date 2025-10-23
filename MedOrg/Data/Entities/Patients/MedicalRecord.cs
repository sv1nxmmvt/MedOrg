using MedOrg.Data.Entities.Institutions;
using MedOrg.Data.Entities.MedStaff;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Patients
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public DateTime RecordDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Diagnosis { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Symptoms { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Treatment { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;
    }
}