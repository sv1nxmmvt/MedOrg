namespace MedOrg.Data.DTOs.Patients
{
    public class HospitalizedPatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime AdmissionDate { get; set; }
        public string Condition { get; set; } = string.Empty;
        public decimal? Temperature { get; set; }
        public string AttendingDoctorName { get; set; } = string.Empty;
        public string WardNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }
}