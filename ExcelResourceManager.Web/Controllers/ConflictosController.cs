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
            _logger.LogInformation("Calculando conflictos futuros on-demand");
            
            // Calcular conflictos on-demand (solo futuros)
            var conflictos = await _validationService.ValidarTodosFuturosAsync();
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
            }).OrderByDescending(c => c.Nivel).ThenBy(c => c.FechaConflicto).ToList();
            
            _logger.LogInformation("Se encontraron {Count} conflictos futuros", conflictosViewModel.Count);
            
            return View(conflictosViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular conflictos");
            return View(new List<ConflictoViewModel>());
        }
    }
}
