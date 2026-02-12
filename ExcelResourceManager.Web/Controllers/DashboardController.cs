using Microsoft.AspNetCore.Mvc;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IClienteService _clienteService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IEmpleadoService empleadoService,
        IClienteService clienteService,
        IUnitOfWork unitOfWork,
        ILogger<DashboardController> logger)
    {
        _empleadoService = empleadoService;
        _clienteService = clienteService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Load all data
            var empleados = await _empleadoService.ObtenerTodosAsync();
            var empleadosActivos = await _empleadoService.ObtenerActivosAsync();
            var clientes = await _clienteService.ObtenerTodosAsync();
            var clientesActivos = await _clienteService.ObtenerActivosAsync();
            var conflictos = await _unitOfWork.Conflictos.GetAllAsync();
            var vacaciones = await _unitOfWork.Vacaciones.GetAllAsync();

            var conflictosNoResueltos = conflictos.Where(c => !c.Resuelto).ToList();
            var fechaHoy = DateTime.Today;
            var fechaLimite = fechaHoy.AddDays(30);

            var proximasVacaciones = vacaciones
                .Where(v => v.FechaInicio >= fechaHoy && v.FechaInicio <= fechaLimite)
                .OrderBy(v => v.FechaInicio)
                .ToList();

            // Create view model
            var viewModel = new DashboardViewModel
            {
                TotalEmpleados = empleados.Count,
                EmpleadosActivos = empleadosActivos.Count,
                TotalClientes = clientes.Count,
                ClientesActivos = clientesActivos.Count,
                TotalConflictos = conflictosNoResueltos.Count,
                ConflictosCriticos = conflictosNoResueltos.Count(c => c.Nivel == NivelConflicto.Critico),
                ProximasVacaciones = proximasVacaciones.Select(v => new VacacionDisplay
                {
                    EmpleadoNombre = empleados.FirstOrDefault(e => e.Id == v.EmpleadoId)?.NombreCompleto ?? "Desconocido",
                    FechaInicio = v.FechaInicio,
                    FechaFin = v.FechaFin,
                    DiasHabiles = v.DiasHabiles,
                    Estado = v.Estado.ToString()
                }).ToList()
            };

            _logger.LogInformation("Dashboard loaded successfully");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View("Error");
        }
    }
}

public class DashboardViewModel
{
    public int TotalEmpleados { get; set; }
    public int EmpleadosActivos { get; set; }
    public int TotalClientes { get; set; }
    public int ClientesActivos { get; set; }
    public int TotalConflictos { get; set; }
    public int ConflictosCriticos { get; set; }
    public List<VacacionDisplay> ProximasVacaciones { get; set; } = new();
}

public class VacacionDisplay
{
    public string EmpleadoNombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int DiasHabiles { get; set; }
    public string Estado { get; set; } = string.Empty;
}
