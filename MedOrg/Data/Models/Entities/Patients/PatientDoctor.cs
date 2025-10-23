using MedOrg.Data.Models.Entities.MedStaff;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.Entities.Patients
{
    public class PatientDoctor
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}