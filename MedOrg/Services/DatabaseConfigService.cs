using Microsoft.EntityFrameworkCore;

namespace MedOrg.Services
{
    public class DatabaseConfigService
    {
        private readonly IConfiguration _configuration;

        public DatabaseConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            var host = _configuration["Database:Host"] ?? "localhost";
            var port = _configuration["Database:Port"] ?? "5432";
            var database = _configuration["Database:Database"] ?? "medorg_db";
            var username = _configuration["Database:Username"] ?? "postgres";
            var password = _configuration["Database:Password"] ?? "zasada1324";

            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public DbContextOptions<Data.MedOrgDbContext> GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Data.MedOrgDbContext>();
            optionsBuilder.UseNpgsql(GetConnectionString());
            return optionsBuilder.Options;
        }
    }
}