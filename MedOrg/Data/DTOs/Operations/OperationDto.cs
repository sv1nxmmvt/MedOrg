namespace MedOrg.Data.DTOs.Operations
{
    public class OperationDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string InstitutionName { get; set; } = string.Empty;
    }
}