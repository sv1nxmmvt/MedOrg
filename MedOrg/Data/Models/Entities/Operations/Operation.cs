using System.ComponentModel.DataAnnotations;
using MedOrg.Data.Models.Entities.Institutions;
using MedOrg.Data.Models.Entities.MedStaff;
using MedOrg.Data.Models.Entities.Patients;

namespace MedOrg.Data.Models.Entities.Operations
{
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
}