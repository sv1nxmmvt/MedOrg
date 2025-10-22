namespace MedOrg.Models.DTOs
{
    public class SupportStaffDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
    }
}