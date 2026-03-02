using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class VacacionesController : Controller
{
    private readonly IVacacionService _vacacionService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VacacionesController> _logger;

    public VacacionesController(
        IVacacionService vacacionService,
        IEmpleadoService empleadoService,
        IValidationService validationService,
        IUnitOfWork unitOfWork,
        ILogger<VacacionesController> logger)
    {
        _vacacionService = vacacionService;
        _empleadoService = empleadoService;
        _validationService = validationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? empleadoId)
    {
        try
        {
            IEnumerable<Vacacion> vacaciones;
            if (empleadoId.HasValue)
                vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => v.EmpleadoId == empleadoId.Value);
            else
                vacaciones = await _vacacionService.ObtenerTodasAsync();

            var empleados = await _empleadoService.ObtenerTodosAsync();
            
            // Crear ViewModels con información del empleado
            var vacacionesViewModel = vacaciones.Select(v =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == v.EmpleadoId);
                return new VacacionViewModel
                {
                    Id = v.Id,
                    EmpleadoId = v.EmpleadoId,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido",
                    EmpleadoEmail = empleado?.Email ?? "",
                    FechaInicio = v.FechaInicio,
                    FechaFin = v.FechaFin,
                    Estado = v.Estado,
                    DiasHabiles = v.DiasHabiles,
                    TieneConflictos = v.TieneConflictos,
                    Observaciones = v.Observaciones
                };
            }).ToList();
            
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.EmpleadoId = empleadoId;
            return View(vacacionesViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar vacaciones");
            return View(new List<VacacionViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vacacion vacacion, int? empleadoId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                vacacion.Estado = ExcelResourceManager.Core.Enums.EstadoVacacion.Solicitada;
                
                // Validar conflictos (solo para mostrar advertencia, no se guardan)
                var conflictos = await _validationService.ValidarVacacionAsync(vacacion);
                vacacion.TieneConflictos = conflictos.Any();
                
                // Crear la vacación
                var vacacionId = await _vacacionService.CrearAsync(vacacion);
                
                if (conflictos.Any(c => c.Nivel == ExcelResourceManager.Core.Enums.NivelConflicto.Critico))
                {
                    TempData["Warning"] = $"Vacación creada con {conflictos.Count} conflicto(s) detectado(s). Puedes verlos en la página de Conflictos.";
                }
                else
                {
                    TempData["Success"] = "Vacación creada exitosamente";
                }

                if (empleadoId.HasValue)
                    return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });
                
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vacación");
            TempData["Error"] = "Error al crear la vacación";
        }

        if (empleadoId.HasValue)
            return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int? empleadoId)
    {
        try
        {
            await _vacacionService.EliminarAsync(id);
            TempData["Success"] = "Vacación eliminada exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar vacación {id}");
            TempData["Error"] = "Error al eliminar la vacación";
        }

        if (empleadoId.HasValue)
            return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Aprobar(int id, int? empleadoId)
    {
        try
        {
            var vacacion = await _vacacionService.ObtenerPorIdAsync(id);
            if (vacacion != null)
            {
                vacacion.Estado = ExcelResourceManager.Core.Enums.EstadoVacacion.Aprobada;
                await _vacacionService.ActualizarAsync(vacacion);
                TempData["Success"] = "Vacación aprobada exitosamente";
            }
            else
            {
                TempData["Error"] = "Vacación no encontrada";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al aprobar vacación {id}");
            TempData["Error"] = "Error al aprobar la vacación";
        }

        if (empleadoId.HasValue)
            return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Rechazar(int id, int? empleadoId)
    {
        try
        {
            var vacacion = await _vacacionService.ObtenerPorIdAsync(id);
            if (vacacion != null)
            {
                vacacion.Estado = ExcelResourceManager.Core.Enums.EstadoVacacion.Rechazada;
                await _vacacionService.ActualizarAsync(vacacion);
                TempData["Success"] = "Vacación rechazada exitosamente";
            }
            else
            {
                TempData["Error"] = "Vacación no encontrada";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al rechazar vacación {id}");
            TempData["Error"] = "Error al rechazar la vacación";
        }

        if (empleadoId.HasValue)
            return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });

        return RedirectToAction(nameof(Index));
    }
}
