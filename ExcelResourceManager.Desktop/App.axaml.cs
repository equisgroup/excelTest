using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using ExcelResourceManager.Desktop.ViewModels;
using ExcelResourceManager.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Serilog;
using ExcelResourceManager.Data;
using ExcelResourceManager.Data.Repositories;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Repositories;

namespace ExcelResourceManager.Desktop;

public partial class App : Application
{
    private static IConfiguration? _configuration;
    private static string _currentConnectionString = string.Empty;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Cargar configuración desde appsettings.json
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configurar Serilog desde el archivo de configuración
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();

        // Inicializar con TestDatabase por defecto
        _currentConnectionString = _configuration.GetConnectionString("TestDatabase") 
            ?? "Filename=database-test.db;Connection=shared";

        Log.Information("Aplicación inicializada correctamente");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // Verificar y cargar datos de prueba si es necesario
            EnsureTestDataExists();

            // Crear ventana principal con ViewModel
            var mainViewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            Log.Information("Ventana principal creada");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void EnsureTestDataExists()
    {
        var dbPath = "database-test.db";
        var needsSeeding = !File.Exists(dbPath);
        
        if (!needsSeeding)
        {
            try
            {
                using var context = new LiteDbContext(_currentConnectionString);
                using var unitOfWork = new UnitOfWork(context);
                var empleados = unitOfWork.Empleados.GetAllAsync().Result;
                needsSeeding = !empleados.Any();
                
                if (needsSeeding)
                {
                    Log.Information("Base de datos existe pero está vacía. Se cargarán datos de prueba.");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al verificar contenido de la base de datos.");
                needsSeeding = true;
            }
        }
        else
        {
            Log.Information("Base de datos no encontrada. Iniciando proceso de carga de datos de prueba...");
        }
        
        if (needsSeeding)
        {
            Task.Run(async () =>
            {
                using var context = new LiteDbContext(_currentConnectionString);
                using var unitOfWork = new UnitOfWork(context);
                var feriadoService = new FeriadoService(unitOfWork);
                var dataSeedService = new DataSeedService(unitOfWork, feriadoService);
                await dataSeedService.SeedTestDataAsync();
                Log.Information("Datos de prueba cargados exitosamente");
            }).Wait();
        }
    }

    /// <summary>
    /// Crea servicios con el modo actual
    /// </summary>
    public static T CreateService<T>(bool isTestMode) where T : class
    {
        var connectionString = isTestMode 
            ? (_configuration?.GetConnectionString("TestDatabase") ?? "Filename=database-test.db;Connection=shared")
            : (_configuration?.GetConnectionString("ProdDatabase") ?? "Filename=database-prod.db;Connection=shared");

        var context = new LiteDbContext(connectionString);
        var unitOfWork = new UnitOfWork(context);

        if (typeof(T) == typeof(IEmpleadoService))
            return (new EmpleadoService(unitOfWork) as T)!;
        if (typeof(T) == typeof(IClienteService))
            return (new ClienteService(unitOfWork) as T)!;
        if (typeof(T) == typeof(IFeriadoService))
            return (new FeriadoService(unitOfWork) as T)!;
        if (typeof(T) == typeof(IVacacionService))
        {
            var feriadoService = new FeriadoService(unitOfWork);
            return (new VacacionService(unitOfWork, feriadoService) as T)!;
        }
        if (typeof(T) == typeof(IValidationService))
        {
            var feriadoService = new FeriadoService(unitOfWork);
            return (new ValidationService(unitOfWork, feriadoService) as T)!;
        }
        if (typeof(T) == typeof(IUnitOfWork))
            return (unitOfWork as T)!;

        throw new InvalidOperationException($"Servicio {typeof(T).Name} no está registrado");
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}