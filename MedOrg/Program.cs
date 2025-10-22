using MedOrg.Components;
using MedOrg.Configuration;
using MedOrg.Data;
using MedOrg.Services;
using MedOrg.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

builder.Services.AddDbContext<MedOrgDbContext>(options =>
{
    var configService = new DatabaseConfigService(builder.Configuration);
    options.UseNpgsql(configService.GetConnectionString());
});

builder.Services.AddScoped<DatabaseConfigService>();
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<QueryService>();
builder.Services.AddScoped<DocumentService>();

builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddScoped<SupportStaffService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapMedicalEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();