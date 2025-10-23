namespace MedOrg.Data.DTOs.Patients
{
    public class SickLeaveDto
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string PatientFullName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string DoctorFullName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
    }
}