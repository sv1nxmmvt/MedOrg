namespace MedOrg.Data.DTOs.Patients
{
    public class ClinicPatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
    }
}