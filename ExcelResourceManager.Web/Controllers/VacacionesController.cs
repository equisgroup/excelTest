using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class VacacionesController : Controller
{
    private readonly IVacacionService _vacacionService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IValidationService _validationService;
    private readonly ILogger<VacacionesController> _logger;

    public VacacionesController(
        IVacacionService vacacionService,
        IEmpleadoService empleadoService,
        IValidationService validationService,
        ILogger<VacacionesController> logger)
    {
        _vacacionService = vacacionService;
        _empleadoService = empleadoService;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var vacaciones = await _vacacionService.ObtenerTodasAsync();
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            return View(vacaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar vacaciones");
            return View(new List<Vacacion>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vacacion vacacion)
    {
        try
        {
            if (ModelState.IsValid)
            {
                vacacion.Estado = ExcelResourceManager.Core.Enums.EstadoVacacion.Solicitada;
                
                // Validar conflictos
                var conflictos = await _validationService.ValidarVacacionAsync(vacacion);
                
                if (conflictos.Any(c => c.Nivel == ExcelResourceManager.Core.Enums.NivelConflicto.Critico))
                {
                    TempData["Error"] = "No se puede crear la vacación: existen conflictos críticos";
                    return RedirectToAction(nameof(Index));
                }
                
                await _vacacionService.CrearAsync(vacacion);
                TempData["Success"] = "Vacación creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vacación");
            TempData["Error"] = "Error al crear la vacación";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
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
        return RedirectToAction(nameof(Index));
    }
}
