using System.ComponentModel.DataAnnotations;
using MedOrg.Data.Entities.MedStaff;
using MedOrg.Data.Entities.Patients;

namespace MedOrg.Data.Entities.Institutions
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
}