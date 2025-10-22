using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
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