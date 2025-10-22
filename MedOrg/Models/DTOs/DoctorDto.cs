namespace MedOrg.Models.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public string? Title { get; set; }
        public int? TotalOperations { get; set; }
        public int? FatalOperations { get; set; }
        public decimal? HazardPayCoefficient { get; set; }
        public bool ExtendedVacation { get; set; }
        public int YearsOfExperience { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public string? SecondaryInstitutionName { get; set; }
    }
}