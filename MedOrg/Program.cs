using MedOrg.Components;
using MedOrg.Configuration;
using MedOrg.Data;
using MedOrg.Services;
using MedOrg.Services.Auth;
using MedOrg.Services.Db;
using MedOrg.Services.Ex;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine();
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║            MedOrg - Медицинская организация               ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

var dbConfig = await DatabaseConnectionConfigurator.EnsureConfigurationAsync();

if (!await DatabaseConnectionConfigurator.TestConnectionAsync(dbConfig))
{
    Console.WriteLine();
    Console.WriteLine("✗ Не удалось подключиться к базе данных.");
    Console.WriteLine("  Проверьте параметры подключения и повторите попытку.");
    Console.WriteLine();
    Console.WriteLine("  Для сброса конфигурации удалите файл: dbconfig.json");
    Console.WriteLine();
    return;
}

Console.WriteLine();
Console.WriteLine("─────────────────────────────────────────────────────────────");
Console.WriteLine("Запуск приложения...");
Console.WriteLine();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MedOrg API",
        Version = "v1",
        Description = "API для управления медицинской организацией"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = builder.Configuration["Jwt:SecretKey"]
        ?? "MedOrgSecretKey_2024_VerySecure_MinimumLength32Characters!@#$";
    options.Issuer = builder.Configuration["Jwt:Issuer"] ?? "MedOrgAPI";
    options.Audience = builder.Configuration["Jwt:Audience"] ?? "MedOrgClient";
    options.AccessTokenExpirationMinutes = int.Parse(
        builder.Configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");
    options.RefreshTokenExpirationDays = int.Parse(
        builder.Configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
});

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? "MedOrgSecretKey_2024_VerySecure_MinimumLength32Characters!@#$";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "MedOrgAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "MedOrgClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PatientOnly", policy =>
        policy.RequireRole("Patient"));

    options.AddPolicy("MedicalStaffOnly", policy =>
        policy.RequireRole("MedicalStaff"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("MedicalStaffOrAdmin", policy =>
        policy.RequireRole("MedicalStaff", "Admin"));
});

builder.Services.AddCascadingAuthenticationState();

var connectionString = DatabaseConnectionConfigurator.GetConnectionString(dbConfig);
builder.Services.AddDbContext<MedOrgDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<DatabaseConfigService>();
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<QueryService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddScoped<ClinicService>();
builder.Services.AddScoped<SupportStaffService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
        Console.WriteLine("✓ База данных инициализирована успешно");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Ошибка инициализации базы данных: {ex.Message}");
        return;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MedOrg API v1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine();
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              Приложение успешно запущено!                 ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

app.Run();