using MedOrg.Components;
using MedOrg.Configuration;
using MedOrg.Data;
using MedOrg.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Blazor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Controllers + API Explorer для Swagger
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

    // Настройка JWT авторизации в Swagger
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

    // Включение XML комментариев (опционально)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// JWT Settings
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

// Authentication & Authorization
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

// Database
builder.Services.AddDbContext<MedOrgDbContext>(options =>
{
    var configService = new DatabaseConfigService(builder.Configuration);
    options.UseNpgsql(configService.GetConnectionString());
});

// Services
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

// CORS (если нужно для внешних клиентов)
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

// Database Initialization
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline
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

// Map Controllers
app.MapControllers();

// Map Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();