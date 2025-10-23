using System.ComponentModel.DataAnnotations;
using System.Numerics;
using MedOrg.Data.Models.Entities.Institutions;
using MedOrg.Data.Models.Entities.MedStaff;
using MedOrg.Data.Models.Entities.Operations;

namespace MedOrg.Data.Models.Entities.Patients
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
}