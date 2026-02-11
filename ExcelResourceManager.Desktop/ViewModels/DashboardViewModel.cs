using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using ReactiveUI;
using Serilog;

namespace ExcelResourceManager.Desktop.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IEmpleadoService? _empleadoService;
    private readonly IClienteService? _clienteService;
    private readonly IUnitOfWork? _unitOfWork;
    private readonly IValidationService? _validationService;
    private bool _isTestMode;

    private int _totalEmpleados;
    private int _empleadosActivos;
    private int _totalClientes;
    private int _clientesActivos;
    private int _totalConflictos;
    private int _conflictosCriticos;
    private ObservableCollection<VacacionDisplay> _proximasVacaciones = new();
    private bool _esCargando;

    public int TotalEmpleados
    {
        get => _totalEmpleados;
        set => this.RaiseAndSetIfChanged(ref _totalEmpleados, value);
    }

    public int EmpleadosActivos
    {
        get => _empleadosActivos;
        set => this.RaiseAndSetIfChanged(ref _empleadosActivos, value);
    }

    public int TotalClientes
    {
        get => _totalClientes;
        set => this.RaiseAndSetIfChanged(ref _totalClientes, value);
    }

    public int ClientesActivos
    {
        get => _clientesActivos;
        set => this.RaiseAndSetIfChanged(ref _clientesActivos, value);
    }

    public int TotalConflictos
    {
        get => _totalConflictos;
        set => this.RaiseAndSetIfChanged(ref _totalConflictos, value);
    }

    public int ConflictosCriticos
    {
        get => _conflictosCriticos;
        set => this.RaiseAndSetIfChanged(ref _conflictosCriticos, value);
    }

    public ObservableCollection<VacacionDisplay> ProximasVacaciones
    {
        get => _proximasVacaciones;
        set => this.RaiseAndSetIfChanged(ref _proximasVacaciones, value);
    }

    public bool EsCargando
    {
        get => _esCargando;
        set => this.RaiseAndSetIfChanged(ref _esCargando, value);
    }

    public ReactiveCommand<Unit, Unit> CargarDatosCommand { get; }

    public DashboardViewModel(bool isTestMode = true)
    {
        _isTestMode = isTestMode;
        
        // Crear servicios con el modo actual
        try
        {
            _empleadoService = App.CreateService<IEmpleadoService>(isTestMode);
            _clienteService = App.CreateService<IClienteService>(isTestMode);
            _unitOfWork = App.CreateService<IUnitOfWork>(isTestMode);
            _validationService = App.CreateService<IValidationService>(isTestMode);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error al crear servicios para DashboardViewModel en modo {Modo}", isTestMode ? "Test" : "Producción");
        }

        // Crear comando con scheduler de UI thread
        CargarDatosCommand = ReactiveCommand.CreateFromTask(
            CargarDatosAsync,
            outputScheduler: RxApp.MainThreadScheduler);

        Log.Information("DashboardViewModel inicializado en modo {Modo}", isTestMode ? "Test" : "Producción");
    }
    
    /// <summary>
    /// Inicializa el ViewModel cargando los datos.
    /// Debe ser llamado después de que el ViewModel esté en el contexto de UI.
    /// </summary>
    public void Initialize()
    {
        if (_empleadoService != null && _clienteService != null)
        {
            CargarDatosCommand.Execute().Subscribe();
        }
    }

    private async Task CargarDatosAsync()
    {
        if (_empleadoService == null || _clienteService == null || _unitOfWork == null)
            return;

        try
        {
            EsCargando = true;
            Log.Information("Cargando datos del dashboard");

            // Cargar empleados
            var todosEmpleados = await _empleadoService.ObtenerTodosAsync();
            var empleadosActivos = await _empleadoService.ObtenerActivosAsync();
            TotalEmpleados = todosEmpleados.Count;
            EmpleadosActivos = empleadosActivos.Count;

            // Cargar clientes
            var todosClientes = await _clienteService.ObtenerTodosAsync();
            var clientesActivos = await _clienteService.ObtenerActivosAsync();
            TotalClientes = todosClientes.Count;
            ClientesActivos = clientesActivos.Count;

            // Cargar conflictos
            var conflictos = await _unitOfWork.Conflictos.GetAllAsync();
            var conflictosNoResueltos = conflictos.Where(c => !c.Resuelto).ToList();
            TotalConflictos = conflictosNoResueltos.Count;
            ConflictosCriticos = conflictosNoResueltos.Count(c => c.Nivel == NivelConflicto.Critico);

            // Cargar próximas vacaciones (próximos 30 días)
            var vacaciones = await _unitOfWork.Vacaciones.GetAllAsync();
            var fechaHoy = DateTime.Today;
            var fechaLimite = fechaHoy.AddDays(30);

            var proximasVacaciones = vacaciones
                .Where(v => v.FechaInicio >= fechaHoy && v.FechaInicio <= fechaLimite)
                .OrderBy(v => v.FechaInicio)
                .ToList();

            var vacacionesDisplay = new List<VacacionDisplay>();
            foreach (var vacacion in proximasVacaciones)
            {
                var empleado = todosEmpleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
                vacacionesDisplay.Add(new VacacionDisplay
                {
                    Vacacion = vacacion,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido"
                });
            }

            ProximasVacaciones = new ObservableCollection<VacacionDisplay>(vacacionesDisplay);

            Log.Information(
                "Dashboard actualizado - Empleados: {TotalEmpleados}/{EmpleadosActivos}, Clientes: {TotalClientes}/{ClientesActivos}, Conflictos: {TotalConflictos}/{ConflictosCriticos}",
                TotalEmpleados, EmpleadosActivos, TotalClientes, ClientesActivos, TotalConflictos, ConflictosCriticos);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar datos del dashboard");
        }
        finally
        {
            EsCargando = false;
        }
    }

    public class VacacionDisplay
    {
        public Vacacion Vacacion { get; set; } = null!;
        public string EmpleadoNombre { get; set; } = string.Empty;

        public DateTime FechaInicio => Vacacion.FechaInicio;
        public DateTime FechaFin => Vacacion.FechaFin;
        public int DiasHabiles => Vacacion.DiasHabiles;
        public EstadoVacacion Estado => Vacacion.Estado;
    }
}
