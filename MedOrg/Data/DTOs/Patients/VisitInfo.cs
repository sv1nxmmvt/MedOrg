namespace MedOrg.Data.DTOs.Patients
{
    public class VisitInfo
    {
        public DateTime VisitDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }
}