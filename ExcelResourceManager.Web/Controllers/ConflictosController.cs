using ExcelResourceManager.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class ConflictosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConflictosController> _logger;

    public ConflictosController(IUnitOfWork unitOfWork, ILogger<ConflictosController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var conflictos = await _unitOfWork.Conflictos.GetAllAsync();
            return View(conflictos.OrderByDescending(c => c.FechaConflicto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar conflictos");
            return View(new List<ExcelResourceManager.Core.Models.Conflicto>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Resolver(int id)
    {
        try
        {
            var conflicto = await _unitOfWork.Conflictos.GetByIdAsync(id);
            if (conflicto != null)
            {
                conflicto.Resuelto = true;
                await _unitOfWork.Conflictos.UpdateAsync(conflicto);
                await _unitOfWork.CommitAsync();
                TempData["Success"] = "Conflicto marcado como resuelto";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al resolver conflicto {id}");
            TempData["Error"] = "Error al resolver el conflicto";
        }
        return RedirectToAction(nameof(Index));
    }
}
