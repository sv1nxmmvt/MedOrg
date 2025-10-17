using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public abstract class MedicalInstitution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<Staff> Staff { get; set; } = new List<Staff>();
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public ICollection<LaboratoryContract> LaboratoryContracts { get; set; } = new List<LaboratoryContract>();
    }

    public class Hospital : MedicalInstitution
    {
        public ICollection<HospitalBuilding> Buildings { get; set; } = new List<HospitalBuilding>();
        public ICollection<Clinic> AttachedClinics { get; set; } = new List<Clinic>();
    }

    public class Clinic : MedicalInstitution
    {
        public int? AttachedHospitalId { get; set; }
        public Hospital? AttachedHospital { get; set; }

        public ICollection<ClinicsOffice> Offices { get; set; } = new List<ClinicsOffice>();
    }

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

    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Specialization { get; set; } = string.Empty;

        public int BuildingId { get; set; }
        public HospitalBuilding Building { get; set; } = null!;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }

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