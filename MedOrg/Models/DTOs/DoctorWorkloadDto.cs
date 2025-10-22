namespace MedOrg.Models.DTOs
{
    public class DoctorWorkloadDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int CurrentPatients { get; set; }
    }
}