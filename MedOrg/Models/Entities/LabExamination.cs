using System.ComponentModel.DataAnnotations;

namespace MedOrg.Models.Entities
{
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
}