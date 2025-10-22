namespace MedOrg.Models.DTOs
{
    public class QueryResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class QueryResult<T> : QueryResult
    {
        public T? Data { get; set; }
        public int TotalCount { get; set; }
    }
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

    public class SupportStaffDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
    }

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

    public class ClinicPatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
    }

    public class WardStatisticsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalWards { get; set; }
        public int TotalBeds { get; set; }
        public int OccupiedBeds { get; set; }
        public int FreeBeds { get; set; }
        public int FullyFreeWards { get; set; }
    }

    public class OfficeStatisticsDto
    {
        public string OfficeNumber { get; set; } = string.Empty;
        public int VisitsCount { get; set; }
    }

    public class DoctorProductivityDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public decimal AveragePatientsPerDay { get; set; }
        public int TotalPatients { get; set; }
    }

    public class DoctorWorkloadDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int CurrentPatients { get; set; }
    }

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

    public class LaboratoryProductivityDto
    {
        public string LaboratoryName { get; set; } = string.Empty;
        public string InstitutionName { get; set; } = string.Empty;
        public decimal AverageExaminationsPerDay { get; set; }
        public int TotalExaminations { get; set; }
    }

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

    public class MedicalCertificateDto
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string PatientFullName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public List<VisitInfo> Visits { get; set; } = new();
        public string DoctorFullName { get; set; } = string.Empty;
    }

    public class VisitInfo
    {
        public DateTime VisitDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }

    public class DoctorScheduleDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string OfficeNumber { get; set; } = string.Empty;
        public List<ScheduleEntry> Schedule { get; set; } = new();
    }

    public class ScheduleEntry
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}