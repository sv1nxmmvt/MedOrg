using Microsoft.EntityFrameworkCore;
using static MedOrg.Services.Db.DatabaseConnectionConfigurator;

namespace MedOrg.Services.Db
{
    /// <summary>
    /// Сервис для управления конфигурацией подключения к базе данных
    /// </summary>
    public class DatabaseConfigService
    {
        private readonly IConfiguration _configuration;
        private DatabaseConfig? _cachedConfig;

        public DatabaseConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Получает строку подключения (из конфигурации или интерактивно)
        /// </summary>
        public async Task<string> GetConnectionStringAsync()
        {
            var configFromSettings = TryGetConfigFromAppSettings();

            if (configFromSettings != null && !string.IsNullOrEmpty(configFromSettings.Password))
            {
                _cachedConfig = configFromSettings;
                return DatabaseConnectionConfigurator.GetConnectionString(configFromSettings);
            }

            _cachedConfig = await DatabaseConnectionConfigurator.EnsureConfigurationAsync();
            return DatabaseConnectionConfigurator.GetConnectionString(_cachedConfig);
        }

        /// <summary>
        /// Синхронный метод для обратной совместимости
        /// </summary>
        public string GetConnectionString()
        {
            return GetConnectionStringAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Получает DbContext опции с корректной строкой подключения
        /// </summary>
        public async Task<DbContextOptions<Data.MedOrgDbContext>> GetDbContextOptionsAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Data.MedOrgDbContext>();
            var connectionString = await GetConnectionStringAsync();
            optionsBuilder.UseNpgsql(connectionString);
            return optionsBuilder.Options;
        }

        /// <summary>
        /// Синхронный метод для обратной совместимости
        /// </summary>
        public DbContextOptions<Data.MedOrgDbContext> GetDbContextOptions()
        {
            return GetDbContextOptionsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Попытка получить конфигурацию из appsettings.json
        /// </summary>
        private DatabaseConfig? TryGetConfigFromAppSettings()
        {
            var host = _configuration["Database:Host"];
            var port = _configuration["Database:Port"];
            var database = _configuration["Database:Database"];
            var username = _configuration["Database:Username"];
            var password = _configuration["Database:Password"];

            if (!string.IsNullOrEmpty(host) &&
                !string.IsNullOrEmpty(port) &&
                !string.IsNullOrEmpty(database) &&
                !string.IsNullOrEmpty(username) &&
                !string.IsNullOrEmpty(password))
            {
                return new DatabaseConfig
                {
                    Host = host,
                    Port = port,
                    Database = database,
                    Username = username,
                    Password = password
                };
            }

            return null;
        }

        /// <summary>
        /// Тестирование текущего подключения
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            if (_cachedConfig == null)
            {
                await GetConnectionStringAsync();
            }

            return await DatabaseConnectionConfigurator.TestConnectionAsync(_cachedConfig!);
        }
    }
}