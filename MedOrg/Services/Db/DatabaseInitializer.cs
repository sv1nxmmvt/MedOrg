using Microsoft.EntityFrameworkCore;
using MedOrg.Data;
using MedOrg.Data.Entities.Auth;
using MedOrg.Data.Entities.Institutions;
using MedOrg.Data.Entities.MedStaff;
using MedOrg.Data.Entities.Operations;
using MedOrg.Data.Entities.Patients;

namespace MedOrg.Services.Db
{
    public class DatabaseInitializer
    {
        private readonly MedOrgDbContext _context;

        public DatabaseInitializer(MedOrgDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (await _context.Hospitals.AnyAsync())
            {
                return;
            }

            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            var hospital1 = new Hospital
            {
                Name = "Городская больница №1",
                Address = "ул. Ленина, д. 10",
                Phone = "+7 (495) 123-45-67",
                CreatedDate = DateTime.UtcNow
            };

            var hospital2 = new Hospital
            {
                Name = "Городская больница №2",
                Address = "ул. Пушкина, д. 25",
                Phone = "+7 (495) 234-56-78",
                CreatedDate = DateTime.UtcNow
            };

            _context.Hospitals.AddRange(hospital1, hospital2);
            await _context.SaveChangesAsync();

            var clinic1 = new Clinic
            {
                Name = "Поликлиника №1",
                Address = "ул. Гагарина, д. 5",
                Phone = "+7 (495) 345-67-89",
                AttachedHospitalId = hospital1.Id,
                CreatedDate = DateTime.UtcNow
            };

            var clinic2 = new Clinic
            {
                Name = "Поликлиника №2",
                Address = "ул. Чехова, д. 15",
                Phone = "+7 (495) 456-78-90",
                CreatedDate = DateTime.UtcNow
            };

            _context.Clinics.AddRange(clinic1, clinic2);
            await _context.SaveChangesAsync();

            var building1 = new HospitalBuilding
            {
                Name = "Главный корпус",
                HospitalId = hospital1.Id
            };

            var building2 = new HospitalBuilding
            {
                Name = "Хирургический корпус",
                HospitalId = hospital1.Id
            };

            _context.HospitalBuildings.AddRange(building1, building2);
            await _context.SaveChangesAsync();

            var department1 = new Department
            {
                Name = "Терапевтическое отделение",
                Specialization = "Общая терапия",
                BuildingId = building1.Id
            };

            var department2 = new Department
            {
                Name = "Хирургическое отделение",
                Specialization = "Общая хирургия",
                BuildingId = building2.Id
            };

            var department3 = new Department
            {
                Name = "Кардиологическое отделение",
                Specialization = "Кардиология",
                BuildingId = building1.Id
            };

            _context.Departments.AddRange(department1, department2, department3);
            await _context.SaveChangesAsync();

            var wards = new List<Ward>();
            for (int i = 1; i <= 5; i++)
            {
                wards.Add(new Ward
                {
                    Number = $"101-{i}",
                    TotalBeds = 4,
                    DepartmentId = department1.Id
                });
            }

            for (int i = 1; i <= 5; i++)
            {
                wards.Add(new Ward
                {
                    Number = $"201-{i}",
                    TotalBeds = 2,
                    DepartmentId = department2.Id
                });
            }

            _context.Wards.AddRange(wards);
            await _context.SaveChangesAsync();

            var offices = new List<ClinicsOffice>();
            for (int i = 1; i <= 10; i++)
            {
                offices.Add(new ClinicsOffice
                {
                    Number = $"{i}",
                    ClinicId = clinic1.Id
                });
            }

            _context.ClinicsOffices.AddRange(offices);
            await _context.SaveChangesAsync();

            var doctors = new List<Doctor>
            {
                new()
                {
                    FirstName = "Иван",
                    LastName = "Петров",
                    MiddleName = "Сергеевич",
                    BirthDate = new DateTime(1975, 5, 15),
                    HireDate = new DateTime(2000, 9, 1),
                    Phone = "+7 (495) 111-11-11",
                    Specialization = "Терапевт",
                    InstitutionId = clinic1.Id,
                    BaseSalary = 80000,
                    Degree = AcademicDegree.Candidate,
                    Title = AcademicTitle.Docent
                },
                new()
                {
                    FirstName = "Мария",
                    LastName = "Иванова",
                    MiddleName = "Петровна",
                    BirthDate = new DateTime(1980, 8, 20),
                    HireDate = new DateTime(2005, 6, 15),
                    Phone = "+7 (495) 222-22-22",
                    Specialization = "Хирург",
                    InstitutionId = hospital1.Id,
                    BaseSalary = 120000,
                    TotalOperations = 250,
                    FatalOperations = 2,
                    Degree = AcademicDegree.Doctor,
                    Title = AcademicTitle.Professor
                },
                new()
                {
                    FirstName = "Алексей",
                    LastName = "Смирнов",
                    MiddleName = "Владимирович",
                    BirthDate = new DateTime(1985, 3, 10),
                    HireDate = new DateTime(2010, 9, 1),
                    Phone = "+7 (495) 333-33-33",
                    Specialization = "Кардиолог",
                    InstitutionId = hospital1.Id,
                    BaseSalary = 100000
                },
                new()
                {
                    FirstName = "Елена",
                    LastName = "Васильева",
                    MiddleName = "Николаевна",
                    BirthDate = new DateTime(1978, 11, 5),
                    HireDate = new DateTime(2003, 3, 1),
                    Phone = "+7 (495) 444-44-44",
                    Specialization = "Невропатолог",
                    InstitutionId = clinic1.Id,
                    BaseSalary = 85000,
                    ExtraVacationDays = 7
                },
                new()
                {
                    FirstName = "Дмитрий",
                    LastName = "Козлов",
                    MiddleName = "Александрович",
                    BirthDate = new DateTime(1982, 7, 25),
                    HireDate = new DateTime(2008, 1, 15),
                    Phone = "+7 (495) 555-55-55",
                    Specialization = "Рентгенолог",
                    InstitutionId = hospital1.Id,
                    BaseSalary = 95000,
                    HazardPayCoefficient = 1.25m,
                    ExtraVacationDays = 14
                },
                new()
                {
                    FirstName = "Ольга",
                    LastName = "Новикова",
                    MiddleName = "Игоревна",
                    BirthDate = new DateTime(1988, 4, 12),
                    HireDate = new DateTime(2013, 8, 1),
                    Phone = "+7 (495) 666-66-66",
                    Specialization = "Стоматолог",
                    InstitutionId = clinic2.Id,
                    BaseSalary = 90000,
                    HazardPayCoefficient = 1.15m,
                    TotalOperations = 120,
                    FatalOperations = 0
                }
            };

            _context.Doctors.AddRange(doctors);
            await _context.SaveChangesAsync();

            var supportStaff = new List<SupportStaff>
            {
                new()
                {
                    FirstName = "Анна",
                    LastName = "Сидорова",
                    MiddleName = "Петровна",
                    BirthDate = new DateTime(1990, 6, 18),
                    HireDate = new DateTime(2015, 4, 1),
                    Phone = "+7 (495) 777-77-77",
                    Position = "Медсестра",
                    InstitutionId = hospital1.Id,
                    BaseSalary = 45000
                },
                new()
                {
                    FirstName = "Сергей",
                    LastName = "Морозов",
                    MiddleName = "Иванович",
                    BirthDate = new DateTime(1987, 9, 22),
                    HireDate = new DateTime(2012, 11, 15),
                    Phone = "+7 (495) 888-88-88",
                    Position = "Санитар",
                    InstitutionId = hospital1.Id,
                    BaseSalary = 35000
                },
                new()
                {
                    FirstName = "Наталья",
                    LastName = "Федорова",
                    MiddleName = "Сергеевна",
                    BirthDate = new DateTime(1992, 2, 14),
                    HireDate = new DateTime(2017, 6, 1),
                    Phone = "+7 (495) 999-99-99",
                    Position = "Медсестра",
                    InstitutionId = clinic1.Id,
                    BaseSalary = 43000
                }
            };

            _context.SupportStaff.AddRange(supportStaff);
            await _context.SaveChangesAsync();

            var patients = new List<Patient>
            {
                new()
                {
                    FirstName = "Андрей",
                    LastName = "Соколов",
                    MiddleName = "Викторович",
                    BirthDate = new DateTime(1965, 3, 15),
                    Phone = "+7 (495) 111-22-33",
                    Address = "ул. Мира, д. 20, кв. 15",
                    InsuranceNumber = "1234567890123456",
                    ClinicId = clinic1.Id
                },
                new()
                {
                    FirstName = "Татьяна",
                    LastName = "Волкова",
                    MiddleName = "Алексеевна",
                    BirthDate = new DateTime(1972, 7, 8),
                    Phone = "+7 (495) 222-33-44",
                    Address = "пр. Ленина, д. 45, кв. 78",
                    InsuranceNumber = "2345678901234567",
                    ClinicId = clinic1.Id
                },
                new()
                {
                    FirstName = "Владимир",
                    LastName = "Лебедев",
                    MiddleName = "Николаевич",
                    BirthDate = new DateTime(1958, 11, 23),
                    Phone = "+7 (495) 333-44-55",
                    Address = "ул. Садовая, д. 12, кв. 5",
                    InsuranceNumber = "3456789012345678",
                    ClinicId = clinic2.Id
                }
            };

            _context.Patients.AddRange(patients);
            await _context.SaveChangesAsync();

            var hospitalizations = new List<PatientHospitalization>
            {
                new()
                {
                    PatientId = patients[0].Id,
                    HospitalId = hospital1.Id,
                    WardId = wards[0].Id,
                    AttendingDoctorId = doctors[2].Id,
                    AdmissionDate = DateTime.UtcNow.AddDays(-5),
                    Condition = "Удовлетворительное",
                    Temperature = 36.6m,
                    Diagnosis = "Гипертоническая болезнь"
                },
                new()
                {
                    PatientId = patients[1].Id,
                    HospitalId = hospital1.Id,
                    WardId = wards[5].Id,
                    AttendingDoctorId = doctors[1].Id,
                    AdmissionDate = DateTime.UtcNow.AddDays(-10),
                    DischargeDate = DateTime.UtcNow.AddDays(-2),
                    Condition = "Выписана",
                    Temperature = 36.7m,
                    Diagnosis = "Острый аппендицит"
                }
            };

            _context.PatientHospitalizations.AddRange(hospitalizations);
            await _context.SaveChangesAsync();

            var patientDoctors = new List<PatientDoctor>
            {
                new()
                {
                    PatientId = patients[0].Id,
                    DoctorId = doctors[0].Id,
                    StartDate = DateTime.UtcNow.AddMonths(-6)
                },
                new()
                {
                    PatientId = patients[1].Id,
                    DoctorId = doctors[0].Id,
                    StartDate = DateTime.UtcNow.AddYears(-1)
                },
                new()
                {
                    PatientId = patients[1].Id,
                    DoctorId = doctors[3].Id,
                    StartDate = DateTime.UtcNow.AddMonths(-3)
                }
            };

            _context.PatientDoctors.AddRange(patientDoctors);
            await _context.SaveChangesAsync();

            var operations = new List<Operation>
            {
                new()
                {
                    PatientId = patients[1].Id,
                    DoctorId = doctors[1].Id,
                    InstitutionId = hospital1.Id,
                    OperationDate = DateTime.UtcNow.AddDays(-9),
                    OperationType = "Аппендэктомия",
                    Description = "Удаление аппендикса",
                    IsFatal = false,
                    Result = OperationResult.Success
                }
            };

            _context.Operations.AddRange(operations);
            await _context.SaveChangesAsync();

            var laboratories = new List<Laboratory>
            {
                new()
                {
                    Name = "Городская лаборатория №1",
                    Address = "ул. Лабораторная, д. 5",
                    Phone = "+7 (495) 100-00-00"
                },
                new()
                {
                    Name = "Диагностический центр",
                    Address = "пр. Медицинский, д. 10",
                    Phone = "+7 (495) 200-00-00"
                }
            };

            _context.Laboratories.AddRange(laboratories);
            await _context.SaveChangesAsync();

            var labProfiles = new List<LaboratoryProfile>
            {
                new() { LaboratoryId = laboratories[0].Id, ProfileType = ProfileType.Biochemical },
                new() { LaboratoryId = laboratories[0].Id, ProfileType = ProfileType.Chemical },
                new() { LaboratoryId = laboratories[1].Id, ProfileType = ProfileType.Biochemical },
                new() { LaboratoryId = laboratories[1].Id, ProfileType = ProfileType.Physiological }
            };

            _context.LaboratoryProfiles.AddRange(labProfiles);
            await _context.SaveChangesAsync();

            var labContracts = new List<LaboratoryContract>
            {
                new()
                {
                    LaboratoryId = laboratories[0].Id,
                    InstitutionId = hospital1.Id,
                    StartDate = DateTime.UtcNow.AddYears(-1),
                    ContractNumber = "Д-2024-001"
                },
                new()
                {
                    LaboratoryId = laboratories[1].Id,
                    InstitutionId = clinic1.Id,
                    StartDate = DateTime.UtcNow.AddMonths(-6),
                    ContractNumber = "Д-2024-002"
                }
            };

            _context.LaboratoryContracts.AddRange(labContracts);
            await _context.SaveChangesAsync();

            var examinations = new List<LabExamination>
            {
                new()
                {
                    LaboratoryId = laboratories[0].Id,
                    PatientId = patients[0].Id,
                    InstitutionId = hospital1.Id,
                    ExaminationDate = DateTime.UtcNow.AddDays(-7),
                    ProfileType = ProfileType.Biochemical,
                    ExaminationType = "Общий анализ крови",
                    Results = "Все показатели в норме"
                },
                new()
                {
                    LaboratoryId = laboratories[1].Id,
                    PatientId = patients[1].Id,
                    InstitutionId = clinic1.Id,
                    ExaminationDate = DateTime.UtcNow.AddDays(-15),
                    ProfileType = ProfileType.Biochemical,
                    ExaminationType = "Биохимический анализ крови",
                    Results = "Повышен уровень холестерина"
                }
            };

            _context.LabExaminations.AddRange(examinations);
            await _context.SaveChangesAsync();

            var medicalRecords = new List<MedicalRecord>
            {
                new()
                {
                    PatientId = patients[0].Id,
                    RecordDate = DateTime.UtcNow.AddMonths(-2),
                    Diagnosis = "ОРВИ",
                    Symptoms = "Температура, кашель, насморк",
                    Treatment = "Постельный режим, обильное питье, жаропонижающие",
                    DoctorId = doctors[0].Id,
                    InstitutionId = clinic1.Id
                },
                new()
                {
                    PatientId = patients[0].Id,
                    RecordDate = DateTime.UtcNow.AddMonths(-1),
                    Diagnosis = "Гипертоническая болезнь",
                    Symptoms = "Головная боль, повышенное давление",
                    Treatment = "Гипотензивные препараты",
                    DoctorId = doctors[0].Id,
                    InstitutionId = clinic1.Id
                },
                new()
                {
                    PatientId = patients[1].Id,
                    RecordDate = DateTime.UtcNow.AddMonths(-3),
                    Diagnosis = "Остеохондроз",
                    Symptoms = "Боли в спине",
                    Treatment = "Физиотерапия, массаж",
                    DoctorId = doctors[3].Id,
                    InstitutionId = clinic1.Id
                }
            };

            _context.MedicalRecords.AddRange(medicalRecords);
            await _context.SaveChangesAsync();

            var officeVisits = new List<OfficeVisit>();
            var random = new Random();

            for (int i = 0; i < 50; i++)
            {
                officeVisits.Add(new OfficeVisit
                {
                    OfficeId = offices[random.Next(offices.Count)].Id,
                    PatientId = patients[random.Next(patients.Count)].Id,
                    DoctorId = doctors[0].Id,
                    VisitDate = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }

            _context.OfficeVisits.AddRange(officeVisits);
            await _context.SaveChangesAsync();
        }
    }
}