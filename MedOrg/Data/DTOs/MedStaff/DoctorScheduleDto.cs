namespace MedOrg.Data.DTOs.MedStaff
{
    public class DoctorScheduleDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string OfficeNumber { get; set; } = string.Empty;
        public List<ScheduleEntry> Schedule { get; set; } = new();
    }
}