using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using ReactiveUI;
using Serilog;

namespace ExcelResourceManager.Desktop.ViewModels;

public class VacacionesViewModel : ViewModelBase
{
    private readonly IVacacionService _vacacionService;
    private readonly IValidationService _validationService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IFeriadoService _feriadoService;
    private readonly IUnitOfWork _unitOfWork;

    private ObservableCollection<VacacionDisplay> _vacaciones = new();
    private ObservableCollection<Empleado> _empleados = new();
    private Empleado? _empleadoSeleccionado;
    private DateTime? _fechaInicio;
    private DateTime? _fechaFin;
    private string _observaciones = string.Empty;
    private int _diasHabiles;
    private ObservableCollection<Conflicto> _conflictos = new();
    private bool _mostrarAlerta;
    private string _mensajeAlerta = string.Empty;
    private bool _estaValidando;

    public ObservableCollection<VacacionDisplay> Vacaciones
    {
        get => _vacaciones;
        set => this.RaiseAndSetIfChanged(ref _vacaciones, value);
    }

    public ObservableCollection<Empleado> Empleados
    {
        get => _empleados;
        set => this.RaiseAndSetIfChanged(ref _empleados, value);
    }

    public Empleado? EmpleadoSeleccionado
    {
        get => _empleadoSeleccionado;
        set => this.RaiseAndSetIfChanged(ref _empleadoSeleccionado, value);
    }

    public DateTime? FechaInicio
    {
        get => _fechaInicio;
        set => this.RaiseAndSetIfChanged(ref _fechaInicio, value);
    }

    public DateTime? FechaFin
    {
        get => _fechaFin;
        set => this.RaiseAndSetIfChanged(ref _fechaFin, value);
    }

    public string Observaciones
    {
        get => _observaciones;
        set => this.RaiseAndSetIfChanged(ref _observaciones, value);
    }

    public int DiasHabiles
    {
        get => _diasHabiles;
        set => this.RaiseAndSetIfChanged(ref _diasHabiles, value);
    }

    public ObservableCollection<Conflicto> Conflictos
    {
        get => _conflictos;
        set => this.RaiseAndSetIfChanged(ref _conflictos, value);
    }

    public bool MostrarAlerta
    {
        get => _mostrarAlerta;
        set => this.RaiseAndSetIfChanged(ref _mostrarAlerta, value);
    }

    public string MensajeAlerta
    {
        get => _mensajeAlerta;
        set => this.RaiseAndSetIfChanged(ref _mensajeAlerta, value);
    }

    public bool EstaValidando
    {
        get => _estaValidando;
        set => this.RaiseAndSetIfChanged(ref _estaValidando, value);
    }

    public ReactiveCommand<Unit, Unit> GuardarCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelarCommand { get; }
    public ReactiveCommand<Unit, Unit> CargarDatosCommand { get; }

    public VacacionesViewModel(
        IVacacionService vacacionService,
        IValidationService validationService,
        IEmpleadoService empleadoService,
        IFeriadoService feriadoService,
        IUnitOfWork unitOfWork)
    {
        _vacacionService = vacacionService;
        _validationService = validationService;
        _empleadoService = empleadoService;
        _feriadoService = feriadoService;
        _unitOfWork = unitOfWork;

        var canGuardar = this.WhenAnyValue(
            x => x.EmpleadoSeleccionado,
            x => x.FechaInicio,
            x => x.FechaFin,
            (empleado, inicio, fin) => empleado != null && inicio.HasValue && fin.HasValue);

        GuardarCommand = ReactiveCommand.CreateFromTask(
            GuardarAsync, 
            canGuardar,
            RxApp.MainThreadScheduler);
        CancelarCommand = ReactiveCommand.Create(Cancelar);
        CargarDatosCommand = ReactiveCommand.CreateFromTask(
            CargarDatosAsync,
            outputScheduler: RxApp.MainThreadScheduler);

        // Configurar validación reactiva
        this.WhenAnyValue(
                x => x.EmpleadoSeleccionado,
                x => x.FechaInicio,
                x => x.FechaFin)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Where(tuple => tuple.Item1 != null && tuple.Item2.HasValue && tuple.Item3.HasValue)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async _ => await ValidarEnTiempoRealAsync());

        Log.Information("VacacionesViewModel inicializado");
    }

    public VacacionesViewModel() : this(null!, null!, null!, null!, null!)
    {
        // Constructor para soporte de diseñador XAML
    }
    
    /// <summary>
    /// Inicializa el ViewModel cargando los datos.
    /// Debe ser llamado después de que el ViewModel esté en el contexto de UI.
    /// </summary>
    public void Initialize()
    {
        if (_vacacionService != null && _empleadoService != null)
        {
            CargarDatosCommand.Execute().Subscribe();
        }
    }

    private async Task ValidarEnTiempoRealAsync()
    {
        if (EmpleadoSeleccionado == null || !FechaInicio.HasValue || !FechaFin.HasValue)
            return;

        // Salir si los servicios no están disponibles (modo diseñador)
        if (_validationService == null || _feriadoService == null)
            return;

        try
        {
            EstaValidando = true;

            // Crear vacación temporal para validación
            var vacacionTemp = new Vacacion
            {
                Id = 0,
                EmpleadoId = EmpleadoSeleccionado.Id,
                FechaInicio = FechaInicio.Value,
                FechaFin = FechaFin.Value,
                Estado = EstadoVacacion.Solicitada,
                Observaciones = Observaciones
            };

            // Validar conflictos
            var conflictos = await _validationService.ValidarVacacionAsync(vacacionTemp);
            Conflictos = new ObservableCollection<Conflicto>(conflictos);

            // Calcular días hábiles
            DiasHabiles = await _feriadoService.CalcularDiasHabilesAsync(
                FechaInicio.Value,
                FechaFin.Value,
                EmpleadoSeleccionado.UbicacionId);

            // Actualizar alerta
            var conflictosCriticos = conflictos.Where(c => c.Nivel == NivelConflicto.Critico).ToList();
            if (conflictosCriticos.Any())
            {
                MostrarAlerta = true;
                MensajeAlerta = $"¡ATENCIÓN! {conflictosCriticos.Count} conflicto(s) crítico(s) detectado(s)";
            }
            else
            {
                MostrarAlerta = false;
                MensajeAlerta = string.Empty;
            }

            Log.Debug("Validación en tiempo real completada. Conflictos: {Count}, Días hábiles: {DiasHabiles}",
                conflictos.Count, DiasHabiles);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error durante validación en tiempo real");
        }
        finally
        {
            EstaValidando = false;
        }
    }

    private async Task GuardarAsync()
    {
        if (EmpleadoSeleccionado == null || !FechaInicio.HasValue || !FechaFin.HasValue)
        {
            Log.Warning("Intento de guardar con datos incompletos");
            return;
        }

        // Salir si los servicios no están disponibles (modo diseñador)
        if (_vacacionService == null || _unitOfWork == null)
            return;

        try
        {
            Log.Information("Guardando nueva vacación para empleado {EmpleadoId}", EmpleadoSeleccionado.Id);

            var vacacion = new Vacacion
            {
                EmpleadoId = EmpleadoSeleccionado.Id,
                FechaInicio = FechaInicio.Value,
                FechaFin = FechaFin.Value,
                Estado = EstadoVacacion.Solicitada,
                DiasHabiles = DiasHabiles,
                TieneConflictos = Conflictos.Any(),
                Observaciones = Observaciones
            };

            var vacacionId = await _vacacionService.CrearAsync(vacacion);
            vacacion.Id = vacacionId;

            // Guardar conflictos asociados
            foreach (var conflicto in Conflictos)
            {
                conflicto.VacacionId = vacacionId;
                await _unitOfWork.Conflictos.InsertAsync(conflicto);
            }

            await _unitOfWork.CommitAsync();

            Log.Information("Vacación guardada exitosamente con ID {VacacionId}", vacacionId);

            // Recargar y limpiar formulario
            await CargarDatosAsync();
            Cancelar();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al guardar vacación");
        }
    }

    private void Cancelar()
    {
        EmpleadoSeleccionado = null;
        FechaInicio = null;
        FechaFin = null;
        Observaciones = string.Empty;
        DiasHabiles = 0;
        Conflictos.Clear();
        MostrarAlerta = false;
        MensajeAlerta = string.Empty;

        Log.Debug("Formulario de vacación cancelado/limpiado");
    }

    private async Task CargarDatosAsync()
    {
        // Salir si los servicios no están disponibles (modo diseñador)
        if (_vacacionService == null || _empleadoService == null)
            return;

        try
        {
            Log.Information("Cargando datos de vacaciones y empleados");

            var vacaciones = await _vacacionService.ObtenerTodasAsync();
            var empleados = await _empleadoService.ObtenerActivosAsync();

            var vacacionesDisplay = new List<VacacionDisplay>();
            foreach (var vacacion in vacaciones)
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
                vacacionesDisplay.Add(new VacacionDisplay
                {
                    Vacacion = vacacion,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido"
                });
            }

            Vacaciones = new ObservableCollection<VacacionDisplay>(vacacionesDisplay);
            Empleados = new ObservableCollection<Empleado>(empleados);

            Log.Information("Datos cargados: {VacacionesCount} vacaciones, {EmpleadosCount} empleados",
                vacaciones.Count, empleados.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar datos");
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
        public bool TieneConflictos => Vacacion.TieneConflictos;
    }
}
