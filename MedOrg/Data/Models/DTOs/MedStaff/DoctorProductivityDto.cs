namespace MedOrg.Data.Models.DTOs.MedStaff
{
    public class DoctorProductivityDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public decimal AveragePatientsPerDay { get; set; }
        public int TotalPatients { get; set; }
    }
}