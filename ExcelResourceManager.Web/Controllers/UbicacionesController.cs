using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class UbicacionesController : Controller
{
    private readonly IUbicacionService _ubicacionService;
    private readonly IExcelImportExportService _excelService;
    private readonly ILogger<UbicacionesController> _logger;

    public UbicacionesController(IUbicacionService ubicacionService, IExcelImportExportService excelService, ILogger<UbicacionesController> logger)
    {
        _ubicacionService = ubicacionService;
        _excelService = excelService;
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

    [HttpGet]
    public async Task<IActionResult> ExportarExcel()
    {
        try
        {
            var ubicaciones = await _ubicacionService.ObtenerTodasAsync();
            var bytes = _excelService.ExportarUbicaciones(ubicaciones);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Ubicaciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar ubicaciones");
            TempData["Error"] = "Error al exportar ubicaciones";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult ImportarExcel()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportarExcel(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            TempData["Error"] = "Debe seleccionar un archivo Excel válido";
            return View();
        }

        try
        {
            var existentes = await _ubicacionService.ObtenerTodasAsync();

            using var stream = archivo.OpenReadStream();
            var result = await _excelService.ImportarUbicacionesAsync(stream, existentes, async u =>
            {
                await _ubicacionService.AgregarUbicacionAsync(u);
            });

            result.ReturnUrl = Url.Action(nameof(Index)) ?? "/Ubicaciones";
            TempData["ImportResult"] = System.Text.Json.JsonSerializer.Serialize(result);
            return RedirectToAction(nameof(ResultadoImportacion));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar ubicaciones");
            TempData["Error"] = "Error al procesar el archivo de importación";
            return View();
        }
    }

    [HttpGet]
    public IActionResult ResultadoImportacion()
    {
        var json = TempData["ImportResult"] as string;
        if (string.IsNullOrEmpty(json))
            return RedirectToAction(nameof(Index));

        var result = System.Text.Json.JsonSerializer.Deserialize<ExcelResourceManager.Reports.ImportResult>(json);
        if (result == null)
            return RedirectToAction(nameof(Index));

        TempData.Keep("ImportResult");
        return View("ImportResult", result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExportarErrores(string erroresJson, string entidad)
    {
        try
        {
            var errores = System.Text.Json.JsonSerializer.Deserialize<List<ExcelResourceManager.Reports.ImportError>>(erroresJson) ?? new();
            var bytes = _excelService.ExportarErroresImportacion(errores, entidad);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Errores_{entidad}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar errores de importación");
            TempData["Error"] = "Error al exportar los errores";
            return RedirectToAction(nameof(Index));
        }
    }
}
