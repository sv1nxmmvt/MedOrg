using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
    public class Laboratory
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

        public ICollection<LaboratoryContract> Contracts { get; set; } = new List<LaboratoryContract>();

        public ICollection<LaboratoryProfile> Profiles { get; set; } = new List<LaboratoryProfile>();

        public ICollection<LabExamination> Examinations { get; set; } = new List<LabExamination>();
    }

    public class LaboratoryProfile
    {
        [Key]
        public int Id { get; set; }

        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = null!;

        public ProfileType ProfileType { get; set; }
    }

    public class LaboratoryContract
    {
        [Key]
        public int Id { get; set; }

        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = null!;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(100)]
        public string ContractNumber { get; set; } = string.Empty;
    }

    public class LabExamination
    {
        [Key]
        public int Id { get; set; }

        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = null!;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;

        public DateTime ExaminationDate { get; set; }

        public ProfileType ProfileType { get; set; }

        [Required]
        [MaxLength(200)]
        public string ExaminationType { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Results { get; set; } = string.Empty;
    }

    public enum ProfileType
    {
        Biochemical = 1,
        Physiological = 2,
        Chemical = 3
    }
}