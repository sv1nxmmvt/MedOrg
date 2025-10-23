namespace MedOrg.Data.Models.DTOs.Institutions
{
    public class WardStatisticsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalWards { get; set; }
        public int TotalBeds { get; set; }
        public int OccupiedBeds { get; set; }
        public int FreeBeds { get; set; }
        public int FullyFreeWards { get; set; }
    }
}