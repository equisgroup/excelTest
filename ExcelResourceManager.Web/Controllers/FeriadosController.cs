using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelResourceManager.Web.Controllers;

public class FeriadosController : Controller
{
    private readonly IFeriadoService _feriadoService;
    private readonly IUbicacionService _ubicacionService;
    private readonly ILogger<FeriadosController> _logger;

    public FeriadosController(
        IFeriadoService feriadoService,
        IUbicacionService ubicacionService,
        ILogger<FeriadosController> logger)
    {
        _feriadoService = feriadoService;
        _ubicacionService = ubicacionService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? ubicacionId, int? año)
    {
        try
        {
            var feriados = await _feriadoService.ObtenerTodosAsync(ubicacionId, año);
            var ubicaciones = await _ubicacionService.ObtenerTodasAsync();

            var viewModels = feriados.Select(f =>
            {
                var ubicacion = ubicaciones.FirstOrDefault(u => u.Id == f.UbicacionId);
                return new FeriadoViewModel
                {
                    Id = f.Id,
                    UbicacionId = f.UbicacionId,
                    UbicacionNombre = ubicacion != null ? $"{ubicacion.Ciudad} ({ubicacion.Pais})" : "Desconocida",
                    Fecha = f.Fecha,
                    Nombre = f.Nombre,
                    EsNacional = f.EsNacional,
                    Año = f.Año
                };
            }).ToList();

            ViewBag.Ubicaciones = new SelectList(ubicaciones, "Id", "Ciudad", ubicacionId);
            ViewBag.UbicacionId = ubicacionId;
            ViewBag.Año = año;
            ViewBag.AñosDisponibles = feriados.Select(f => f.Año).Distinct().OrderByDescending(a => a)
                .Select(a => new SelectListItem(a.ToString(), a.ToString(), a == año)).ToList();

            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar feriados");
            return View(new List<FeriadoViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateUbicacionesViewBag();
        return View(new Feriado { Fecha = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Feriado feriado)
    {
        if (!ModelState.IsValid)
        {
            await PopulateUbicacionesViewBag(feriado.UbicacionId);
            return View(feriado);
        }

        try
        {
            await _feriadoService.AgregarFeriadoAsync(feriado);
            TempData["Success"] = "Feriado creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear feriado");
            ModelState.AddModelError("", "Error al crear el feriado");
            await PopulateUbicacionesViewBag(feriado.UbicacionId);
            return View(feriado);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var feriado = await _feriadoService.ObtenerPorIdAsync(id);
        if (feriado == null)
            return NotFound();

        await PopulateUbicacionesViewBag(feriado.UbicacionId);
        return View(feriado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Feriado feriado)
    {
        if (id != feriado.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateUbicacionesViewBag(feriado.UbicacionId);
            return View(feriado);
        }

        try
        {
            await _feriadoService.ActualizarFeriadoAsync(feriado);
            TempData["Success"] = "Feriado actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar feriado {Id}", id);
            ModelState.AddModelError("", "Error al actualizar el feriado");
            await PopulateUbicacionesViewBag(feriado.UbicacionId);
            return View(feriado);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _feriadoService.EliminarFeriadoAsync(id);
            TempData["Success"] = "Feriado eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar feriado {Id}", id);
            TempData["Error"] = "Error al eliminar el feriado";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateUbicacionesViewBag(int? selectedId = null)
    {
        var ubicaciones = await _ubicacionService.ObtenerTodasAsync();
        ViewBag.Ubicaciones = new SelectList(ubicaciones, "Id", "Ciudad", selectedId);
    }
}
