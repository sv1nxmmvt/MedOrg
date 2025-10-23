using System.Text;
using System.Text.Json;

namespace MedOrg.Services.Db
{
    /// <summary>
    /// Интерактивный конфигуратор для настройки подключения к базе данных
    /// </summary>
    public class DatabaseConnectionConfigurator
    {
        private const string ConfigFileName = "dbconfig.json";

        public class DatabaseConfig
        {
            public string Host { get; set; } = "localhost";
            public string Port { get; set; } = "5432";
            public string Database { get; set; } = "medorg_db";
            public string Username { get; set; } = "postgres";
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// Проверяет наличие конфигурации и запрашивает её при необходимости
        /// </summary>
        public static async Task<DatabaseConfig> EnsureConfigurationAsync()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);

            if (File.Exists(configPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(configPath);
                    var config = JsonSerializer.Deserialize<DatabaseConfig>(json);

                    if (config != null && !string.IsNullOrEmpty(config.Password))
                    {
                        Console.WriteLine("✓ Найдена существующая конфигурация базы данных");
                        return config;
                    }
                }
                catch
                {
                    // Если файл поврежден, продолжаем создание новой конфигурации
                }
            }

            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   Конфигурация подключения к базе данных PostgreSQL       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var config = await ConfigureInteractivelyAsync();

            // Сохраняем конфигурацию
            await SaveConfigurationAsync(config, configPath);

            Console.WriteLine();
            Console.WriteLine("✓ Конфигурация сохранена в файл: " + ConfigFileName);
            Console.WriteLine();

            return config;
        }

        /// <summary>
        /// Интерактивная настройка параметров подключения
        /// </summary>
        private static async Task<DatabaseConfig> ConfigureInteractivelyAsync()
        {
            var config = new DatabaseConfig();

            config.Host = ReadLineWithDefault("Server", "localhost");
            config.Database = ReadLineWithDefault("Database", "medorg_db");
            config.Port = ReadLineWithDefault("Port", "5432");
            config.Username = ReadLineWithDefault("Username", "postgres");
            config.Password = ReadPassword($"Пароль пользователя {config.Username}");

            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("Проверка подключения...");

            return config;
        }

        /// <summary>
        /// Чтение строки с значением по умолчанию
        /// </summary>
        private static string ReadLineWithDefault(string prompt, string defaultValue)
        {
            Console.Write($"{prompt} [{defaultValue}]: ");
            var input = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        /// <summary>
        /// Безопасный ввод пароля (символы не отображаются)
        /// </summary>
        private static string ReadPassword(string prompt)
        {
            Console.Write($"{prompt}: ");

            var password = new StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b"); // Стираем символ с экрана
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    // Очистка всего введенного
                    while (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*"); // Показываем звездочку вместо символа
                }
            }

            return password.ToString();
        }

        /// <summary>
        /// Сохранение конфигурации в файл
        /// </summary>
        private static async Task SaveConfigurationAsync(DatabaseConfig config, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(path, json);
        }

        /// <summary>
        /// Генерация строки подключения из конфигурации
        /// </summary>
        public static string GetConnectionString(DatabaseConfig config)
        {
            return $"Host={config.Host};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password}";
        }

        /// <summary>
        /// Удаление сохраненной конфигурации (для пересоздания)
        /// </summary>
        public static void ResetConfiguration()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                Console.WriteLine("✓ Конфигурация удалена");
            }
        }

        /// <summary>
        /// Тестирование подключения к базе данных
        /// </summary>
        public static async Task<bool> TestConnectionAsync(DatabaseConfig config)
        {
            try
            {
                var connectionString = GetConnectionString(config);
                using var connection = new Npgsql.NpgsqlConnection(connectionString);

                await connection.OpenAsync();
                await connection.CloseAsync();

                Console.WriteLine("✓ Подключение успешно установлено");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка подключения: {ex.Message}");
                return false;
            }
        }
    }
}