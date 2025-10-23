using MedOrg.Data.DTOs.Patients;

namespace MedOrg.Data.DTOs.Institutions
{
    public class MedicalCertificateDto
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string PatientFullName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public List<VisitInfo> Visits { get; set; } = new();
        public string DoctorFullName { get; set; } = string.Empty;
    }
}