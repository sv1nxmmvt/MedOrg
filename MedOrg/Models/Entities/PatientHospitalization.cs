using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public class PatientHospitalization
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; } = null!;

        public int WardId { get; set; }
        public Ward Ward { get; set; } = null!;

        public int AttendingDoctorId { get; set; }
        public Doctor AttendingDoctor { get; set; } = null!;

        public DateTime AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }

        [MaxLength(100)]
        public string Condition { get; set; } = string.Empty;

        public decimal? Temperature { get; set; }

        [MaxLength(200)]
        public string Diagnosis { get; set; } = string.Empty;
    }
}