using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class ConflictosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmpleadoService _empleadoService;
    private readonly IValidationService _validationService;
    private readonly ILogger<ConflictosController> _logger;

    public ConflictosController(
        IUnitOfWork unitOfWork, 
        IEmpleadoService empleadoService,
        IValidationService validationService,
        ILogger<ConflictosController> logger)
    {
        _unitOfWork = unitOfWork;
        _empleadoService = empleadoService;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var conflictos = await _unitOfWork.Conflictos.GetAllAsync();
            var empleados = await _empleadoService.ObtenerTodosAsync();
            
            // Crear ViewModels con información del empleado
            var conflictosViewModel = conflictos.Select(c =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == c.EmpleadoId);
                return new ConflictoViewModel
                {
                    Id = c.Id,
                    Tipo = c.Tipo,
                    Nivel = c.Nivel,
                    EmpleadoId = c.EmpleadoId,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido",
                    EmpleadoEmail = empleado?.Email ?? "",
                    FechaConflicto = c.FechaConflicto,
                    Descripcion = c.Descripcion,
                    Recomendacion = c.Recomendacion,
                    Resuelto = c.Resuelto
                };
            }).OrderByDescending(c => c.FechaConflicto).ToList();
            
            return View(conflictosViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar conflictos");
            return View(new List<ConflictoViewModel>());
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

    [HttpPost]
    public async Task<IActionResult> RecalcularConflictos(int año)
    {
        try
        {
            // Eliminar conflictos existentes del año seleccionado
            var conflictosExistentes = await _unitOfWork.Conflictos.FindAsync(c => 
                c.FechaConflicto.Year == año);
            
            foreach (var conflicto in conflictosExistentes)
            {
                await _unitOfWork.Conflictos.DeleteAsync(conflicto.Id);
            }
            await _unitOfWork.CommitAsync();
            
            // Recalcular todos los conflictos
            var nuevosConflictos = await _validationService.ValidarTodosAsync();
            
            // Filtrar solo los del año seleccionado y guardar
            var conflictosDelAño = nuevosConflictos.Where(c => c.FechaConflicto.Year == año).ToList();
            
            foreach (var conflicto in conflictosDelAño)
            {
                await _unitOfWork.Conflictos.InsertAsync(conflicto);
            }
            await _unitOfWork.CommitAsync();
            
            TempData["Success"] = $"Se recalcularon {conflictosDelAño.Count} conflicto(s) para el año {año}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al recalcular conflictos del año {año}");
            TempData["Error"] = "Error al recalcular conflictos";
        }
        return RedirectToAction(nameof(Index));
    }
}
