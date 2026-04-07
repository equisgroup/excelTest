using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class UbicacionesController : Controller
{
    private readonly IUbicacionService _ubicacionService;
    private readonly ILogger<UbicacionesController> _logger;

    public UbicacionesController(IUbicacionService ubicacionService, ILogger<UbicacionesController> logger)
    {
        _ubicacionService = ubicacionService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var ubicaciones = await _ubicacionService.ObtenerTodasAsync();
            return View(ubicaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ubicaciones");
            return View(new List<Ubicacion>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Ubicacion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ubicacion ubicacion)
    {
        if (!ModelState.IsValid)
            return View(ubicacion);

        try
        {
            await _ubicacionService.AgregarUbicacionAsync(ubicacion);
            TempData["Success"] = "Ubicación creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear ubicación");
            ModelState.AddModelError("", "Error al crear la ubicación");
            return View(ubicacion);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var ubicacion = await _ubicacionService.ObtenerPorIdAsync(id);
        if (ubicacion == null)
            return NotFound();
        return View(ubicacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ubicacion ubicacion)
    {
        if (id != ubicacion.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(ubicacion);

        try
        {
            await _ubicacionService.ActualizarUbicacionAsync(ubicacion);
            TempData["Success"] = "Ubicación actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ubicación {Id}", id);
            ModelState.AddModelError("", "Error al actualizar la ubicación");
            return View(ubicacion);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            var ubicacion = await _ubicacionService.ObtenerPorIdAsync(id);
            if (ubicacion == null)
                return NotFound();

            ubicacion.Activo = !ubicacion.Activo;
            await _ubicacionService.ActualizarUbicacionAsync(ubicacion);
            TempData["Success"] = ubicacion.Activo ? "Ubicación activada" : "Ubicación desactivada";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de la ubicación {Id}", id);
            TempData["Error"] = "Error al cambiar el estado de la ubicación";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _ubicacionService.EliminarUbicacionAsync(id);
            TempData["Success"] = "Ubicación eliminada exitosamente";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar ubicación {Id}", id);
            TempData["Error"] = "Error al eliminar la ubicación";
        }
        return RedirectToAction(nameof(Index));
    }
}
