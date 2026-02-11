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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using ExcelResourceManager.Data;
using ExcelResourceManager.Data.Repositories;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Reports;
using ExcelResourceManager.Reports.Generators;

namespace ExcelResourceManager.Desktop;

public partial class App : Application
{
    // Proveedor de servicios para inyección de dependencias
    private static IServiceProvider? _serviceProvider;
    private static IServiceScopeFactory? _scopeFactory;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Cargar configuración desde appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configurar Serilog desde el archivo de configuración
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Crear el contenedor de servicios
        var services = new ServiceCollection();

        // Registrar configuración
        services.AddSingleton<IConfiguration>(configuration);

        // Obtener cadena de conexión desde configuración (usar "TestDatabase" por defecto)
        var connectionString = configuration.GetConnectionString("TestDatabase") 
            ?? "Filename=database-test.db;Connection=shared";

        // Registrar contexto de base de datos como scoped para evitar problemas de concurrencia
        services.AddScoped(sp => new LiteDbContext(connectionString));

        // Registrar Unit of Work como scoped
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar servicios de negocio
        services.AddScoped<IFeriadoService, FeriadoService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IDataSeedService, DataSeedService>();
        services.AddScoped<IVacacionService, VacacionService>();
        services.AddScoped<IViajeService, ViajeService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ITurnoSoporteService, TurnoSoporteService>();
        services.AddScoped<IReportService, ConflictosReportGenerator>();

        // Registrar Serilog
        services.AddSingleton<ILogger>(Log.Logger);

        // Registrar ViewModels como transient
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<VacacionesViewModel>();
        services.AddTransient<DashboardViewModel>();

        // Construir el proveedor de servicios
        _serviceProvider = services.BuildServiceProvider();
        _scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        Log.Information("Aplicación inicializada correctamente con inyección de dependencias");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Evitar validaciones duplicadas de Avalonia y CommunityToolkit
            // Más info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // Verificar si existe la base de datos de prueba y si tiene datos
            var dbPath = "database-test.db";
            var needsSeeding = !File.Exists(dbPath);
            
            if (!needsSeeding)
            {
                // Verificar si la base de datos está vacía
                try
                {
                    using var scope = _serviceProvider!.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var empleados = unitOfWork.Empleados.GetAllAsync().Result;
                    needsSeeding = !empleados.Any();
                    
                    if (needsSeeding)
                    {
                        Log.Information("Base de datos existe pero está vacía. Se cargarán datos de prueba.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Error al verificar contenido de la base de datos. Se asume que necesita datos.");
                    needsSeeding = true;
                }
            }
            else
            {
                Log.Information("Base de datos no encontrada. Iniciando proceso de carga de datos de prueba...");
            }
            
            if (needsSeeding)
            {
                // Usar Task.Run para evitar bloquear el hilo de UI durante la inicialización
                Task.Run(async () =>
                {
                    using var scope = _serviceProvider!.CreateScope();
                    var dataSeedService = scope.ServiceProvider.GetRequiredService<IDataSeedService>();
                    await dataSeedService.SeedTestDataAsync();
                    Log.Information("Datos de prueba cargados exitosamente");
                }).Wait();
            }

            // Crear ventana principal con ViewModel desde DI
            var mainViewModel = GetService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            Log.Information("Ventana principal creada");
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Método estático para obtener servicios desde el contenedor de DI
    /// </summary>
    /// <typeparam name="T">Tipo de servicio a obtener</typeparam>
    /// <returns>Instancia del servicio solicitado</returns>
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("El proveedor de servicios no ha sido inicializado");
        }

        // Si es un ViewModel, crearlo con un scope específico
        if (typeof(T).Name.EndsWith("ViewModel"))
        {
            var scope = _scopeFactory!.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        return _serviceProvider.GetRequiredService<T>();
    }
    
    /// <summary>
    /// Crea un scope para servicios scoped
    /// </summary>
    public static IServiceScope CreateScope()
    {
        if (_scopeFactory == null)
        {
            throw new InvalidOperationException("El proveedor de servicios no ha sido inicializado");
        }
        
        return _scopeFactory.CreateScope();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Obtener array de plugins a remover
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // Remover cada plugin encontrado
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}