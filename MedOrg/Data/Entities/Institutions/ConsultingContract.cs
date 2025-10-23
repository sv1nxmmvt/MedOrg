using MedOrg.Data.Entities.MedStaff;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Entities.Institutions
{
    public class ConsultingContract
    {
        [Key]
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int InstitutionId { get; set; }
        public MedicalInstitution Institution { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}