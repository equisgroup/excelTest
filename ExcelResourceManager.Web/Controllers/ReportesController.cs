using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class ReportesController : Controller
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(IReportService reportService, ILogger<ReportesController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerarConflictos()
    {
        try
        {
            var filePath = await _reportService.GenerarReporteConflictosAsync();
            
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            
            TempData["Error"] = "No se pudo generar el reporte";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de conflictos");
            TempData["Error"] = "Error al generar el reporte";
            return RedirectToAction(nameof(Index));
        }
    }
}
