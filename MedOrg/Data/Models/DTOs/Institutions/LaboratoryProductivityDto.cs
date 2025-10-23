namespace MedOrg.Data.Models.DTOs.Institutions
{
    public class LaboratoryProductivityDto
    {
        public string LaboratoryName { get; set; } = string.Empty;
        public string InstitutionName { get; set; } = string.Empty;
        public decimal AverageExaminationsPerDay { get; set; }
        public int TotalExaminations { get; set; }
    }
}