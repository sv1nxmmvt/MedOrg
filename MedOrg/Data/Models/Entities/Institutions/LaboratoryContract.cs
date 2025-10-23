using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.Entities.Institutions
{
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
}