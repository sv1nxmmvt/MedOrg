using MedOrg.Data.Models.Entities.MedStaff;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.DTOs.MedStaff
{
    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "ФИО не может превышать 200 символов")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Специализация обязательна для заполнения")]
        [StringLength(100, ErrorMessage = "Специализация не может превышать 100 символов")]
        public string Specialization { get; set; } = string.Empty;

        [Range(0, 70, ErrorMessage = "Стаж должен быть от 0 до 70 лет")]
        public int YearsOfExperience { get; set; }

        public AcademicDegree? Degree { get; set; }
        public AcademicTitle? Title { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Количество операций не может быть отрицательным")]
        public int? TotalOperations { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Количество летальных исходов не может быть отрицательным")]
        public int? FatalOperations { get; set; }

        [Range(0, 10, ErrorMessage = "Коэффициент должен быть от 0 до 10")]
        public decimal? HazardPayCoefficient { get; set; }

        public bool ExtendedVacation { get; set; }

        public int? PrimaryInstitutionId { get; set; }
        public int? SecondaryInstitutionId { get; set; }
    }

    public class UpdateDoctorDto : CreateDoctorDto
    {
        [Required]
        public int Id { get; set; }
    }
}