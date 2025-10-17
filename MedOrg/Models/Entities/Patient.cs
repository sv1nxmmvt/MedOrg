using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MedOrg.Models.Entities
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string MiddleName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string InsuranceNumber { get; set; } = string.Empty;

        public int? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public ICollection<PatientHospitalization> Hospitalizations { get; set; } = new List<PatientHospitalization>();

        public ICollection<PatientDoctor> Doctors { get; set; } = new List<PatientDoctor>();

        public ICollection<Operation> Operations { get; set; } = new List<Operation>();

        public ICollection<SickLeave> SickLeaves { get; set; } = new List<SickLeave>();

        public ICollection<MedicalCertificate> Certificates { get; set; } = new List<MedicalCertificate>();

        public ICollection<OfficeVisit> OfficeVisits { get; set; } = new List<OfficeVisit>();
    }

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

    public class PatientDoctor
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Operation
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;

        public DateTime OperationDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string OperationType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool IsFatal { get; set; }

        public OperationResult Result { get; set; }
    }

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

    public enum OperationResult
    {
        Success = 1,
        PartialSuccess = 2,
        Complications = 3,
        Fatal = 4
    }
}