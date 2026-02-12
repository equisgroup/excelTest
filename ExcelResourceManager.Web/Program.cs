using ExcelResourceManager.Data;
using ExcelResourceManager.Data.Repositories;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Get connection string based on mode
var isTestMode = builder.Configuration.GetValue("App:DefaultMode", "Test") == "Test";
var connectionString = isTestMode 
    ? builder.Configuration.GetConnectionString("TestDatabase") ?? "Filename=database-test.db;Connection=shared"
    : builder.Configuration.GetConnectionString("ProdDatabase") ?? "Filename=database-prod.db;Connection=shared";

// Register database and services
builder.Services.AddScoped(sp => new LiteDbContext(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register business services
builder.Services.AddScoped<IFeriadoService, FeriadoService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IDataSeedService, DataSeedService>();
builder.Services.AddScoped<IVacacionService, VacacionService>();
builder.Services.AddScoped<IViajeService, ViajeService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITurnoSoporteService, TurnoSoporteService>();

// Add MVC services
builder.Services.AddControllersWithViews();

// Add session for mode switching
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed test data if needed
if (isTestMode)
{
    using var scope = app.Services.CreateScope();
    var dbPath = "database-test.db";
    if (!File.Exists(dbPath))
    {
        Log.Information("Seeding test data...");
        var dataSeedService = scope.ServiceProvider.GetRequiredService<IDataSeedService>();
        await dataSeedService.SeedTestDataAsync();
        Log.Information("Test data seeded successfully");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

Log.Information("Application started successfully");
app.Run();
