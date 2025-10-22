using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public abstract class Staff
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
        public DateTime HireDate { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;

        public decimal BaseSalary { get; set; }
    }

    public class Doctor : Staff
    {
        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        public AcademicDegree? Degree { get; set; }

        public AcademicTitle? Title { get; set; }

        public int? TotalOperations { get; set; }
        public int? FatalOperations { get; set; }

        public decimal? HazardPayCoefficient { get; set; }

        public int? ExtraVacationDays { get; set; }

        public int YearsOfExperience { get; set; }

        public int? SecondaryInstitutionId { get; set; }
        public MedicalInstitution? SecondaryInstitution { get; set; }

        public ICollection<ConsultingContract> ConsultingContracts { get; set; } = new List<ConsultingContract>();

        public ICollection<PatientHospitalization> HospitalizedPatients { get; set; } = new List<PatientHospitalization>();
        public ICollection<PatientDoctor> ClinicPatients { get; set; } = new List<PatientDoctor>();
        public ICollection<Operation> PerformedOperations { get; set; } = new List<Operation>();
        public ICollection<OfficeVisit> OfficeVisits { get; set; } = new List<OfficeVisit>();
    }

    public class SupportStaff : Staff
    {
        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;
    }
}