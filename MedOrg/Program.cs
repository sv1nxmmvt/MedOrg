using MedOrg.Components;
using MedOrg.Data;
using MedOrg.Services;
using MedOrg.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<MedOrgDbContext>(options =>
{
    var configService = new DatabaseConfigService(builder.Configuration);
    options.UseNpgsql(configService.GetConnectionString());
});

builder.Services.AddScoped<DatabaseConfigService>();
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<QueryService>();
builder.Services.AddScoped<DocumentService>();

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
app.UseAntiforgery();

app.MapMedicalEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();