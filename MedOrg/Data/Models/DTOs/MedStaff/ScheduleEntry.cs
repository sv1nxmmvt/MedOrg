namespace MedOrg.Data.Models.DTOs.MedStaff
{
    public class ScheduleEntry
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}