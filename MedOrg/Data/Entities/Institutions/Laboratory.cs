using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Institutions
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
}