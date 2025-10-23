namespace MedOrg.Data.DTOs.Auth
{
    /// <summary>
    /// Результат аутентификации
    /// </summary>
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
    }
}