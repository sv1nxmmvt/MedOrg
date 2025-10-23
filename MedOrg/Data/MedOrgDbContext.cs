using Microsoft.EntityFrameworkCore;
using MedOrg.Data.Models.Entities.Auth;
using MedOrg.Data.Models.Entities.Institutions;
using MedOrg.Data.Models.Entities.MedStaff;
using MedOrg.Data.Models.Entities.Operations;
using MedOrg.Data.Models.Entities.Patients;

namespace MedOrg.Data
{
    public class MedOrgDbContext : DbContext
    {
        public MedOrgDbContext(DbContextOptions<MedOrgDbContext> options) : base(options)
        {
        }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<HospitalBuilding> HospitalBuildings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<ClinicsOffice> ClinicsOffices { get; set; }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<SupportStaff> SupportStaff { get; set; }
        public DbSet<ConsultingContract> ConsultingContracts { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<PatientHospitalization> PatientHospitalizations { get; set; }
        public DbSet<PatientDoctor> PatientDoctors { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<MedicalCertificate> MedicalCertificates { get; set; }
        public DbSet<OfficeVisit> OfficeVisits { get; set; }

        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<LaboratoryProfile> LaboratoryProfiles { get; set; }
        public DbSet<LaboratoryContract> LaboratoryContracts { get; set; }
        public DbSet<LabExamination> LabExaminations { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicalInstitution>()
                .HasDiscriminator<string>("InstitutionType")
                .HasValue<Hospital>("Hospital")
                .HasValue<Clinic>("Clinic");

            modelBuilder.Entity<Staff>()
                .HasDiscriminator<string>("StaffType")
                .HasValue<Doctor>("Doctor")
                .HasValue<SupportStaff>("SupportStaff");

            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.AttachedHospital)
                .WithMany(h => h.AttachedClinics)
                .HasForeignKey(c => c.AttachedHospitalId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.SecondaryInstitution)
                .WithMany()
                .HasForeignKey(d => d.SecondaryInstitutionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PatientHospitalization>()
                .HasOne(ph => ph.AttendingDoctor)
                .WithMany(d => d.HospitalizedPatients)
                .HasForeignKey(ph => ph.AttendingDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Operation>()
                .HasOne(o => o.Doctor)
                .WithMany(d => d.PerformedOperations)
                .HasForeignKey(o => o.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Specialization);

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => new { d.Degree, d.Title });

            modelBuilder.Entity<PatientHospitalization>()
                .HasIndex(ph => ph.AdmissionDate);

            modelBuilder.Entity<Operation>()
                .HasIndex(o => o.OperationDate);

            modelBuilder.Entity<OfficeVisit>()
                .HasIndex(ov => ov.VisitDate);

            modelBuilder.Entity<LabExamination>()
                .HasIndex(le => le.ExaminationDate);

            modelBuilder.Entity<Staff>()
                .Property(s => s.BaseSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.HazardPayCoefficient)
                .HasPrecision(5, 2);

            modelBuilder.Entity<PatientHospitalization>()
                .Property(ph => ph.Temperature)
                .HasPrecision(4, 1);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithMany()
                .HasForeignKey(u => u.PatientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithMany()
                .HasForeignKey(u => u.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = RoleNames.Patient, Description = "Пациент - доступ к своим данным" },
                new Role { Id = 2, Name = RoleNames.MedicalStaff, Description = "Медицинский работник - доступ к запросам и статистике" },
                new Role { Id = 3, Name = RoleNames.Admin, Description = "Администратор - полный доступ" }
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(
                            new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                            )
                        );
                    }
                }
            }
        }
    }
}