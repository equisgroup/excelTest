using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class EmpleadosController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(IEmpleadoService empleadoService, ILogger<EmpleadosController> logger)
    {
        _empleadoService = empleadoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var empleados = await _empleadoService.ObtenerTodosAsync();
            return View(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleados");
            return View(new List<Empleado>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Empleado empleado)
    {
        try
        {
            if (ModelState.IsValid)
            {
                empleado.Activo = true;
                empleado.FechaIngreso = DateTime.Now;
                await _empleadoService.CrearAsync(empleado);
                TempData["Success"] = "Empleado creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            ModelState.AddModelError("", "Error al crear el empleado");
        }
        return View(empleado);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var empleado = await _empleadoService.ObtenerPorIdAsync(id);
            if (empleado == null)
                return NotFound();
            return View(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar empleado {id}");
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Empleado empleado)
    {
        if (id != empleado.Id)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                await _empleadoService.ActualizarAsync(empleado);
                TempData["Success"] = "Empleado actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar empleado {id}");
            ModelState.AddModelError("", "Error al actualizar el empleado");
        }
        return View(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _empleadoService.EliminarAsync(id);
            TempData["Success"] = "Empleado eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar empleado {id}");
            TempData["Error"] = "Error al eliminar el empleado";
        }
        return RedirectToAction(nameof(Index));
    }
}
