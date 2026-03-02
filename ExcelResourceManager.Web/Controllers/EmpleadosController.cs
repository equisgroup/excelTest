using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class EmpleadosController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(IEmpleadoService empleadoService, IUnitOfWork unitOfWork, ILogger<EmpleadosController> logger)
    {
        _empleadoService = empleadoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var empleado = await _empleadoService.ObtenerPorIdAsync(id);
            if (empleado == null)
                return NotFound();

            var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => v.EmpleadoId == id);
            var viajes = await _unitOfWork.Viajes.FindAsync(v => v.EmpleadoId == id);
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();

            var viewModel = new EmpleadoDetailsViewModel
            {
                Empleado = empleado,
                Vacaciones = vacaciones.Select(v => new VacacionViewModel
                {
                    Id = v.Id,
                    EmpleadoId = v.EmpleadoId,
                    EmpleadoNombre = empleado.NombreCompleto,
                    EmpleadoEmail = empleado.Email,
                    FechaInicio = v.FechaInicio,
                    FechaFin = v.FechaFin,
                    Estado = v.Estado,
                    DiasHabiles = v.DiasHabiles,
                    TieneConflictos = v.TieneConflictos,
                    Observaciones = v.Observaciones
                }).ToList(),
                Viajes = viajes.Select(v =>
                {
                    var cliente = clientes.FirstOrDefault(c => c.Id == v.ClienteDestinoId);
                    var ubicacion = ubicaciones.FirstOrDefault(u => u.Id == v.UbicacionDestinoId);
                    return new ViajeViewModel
                    {
                        Id = v.Id,
                        EmpleadoId = v.EmpleadoId,
                        EmpleadoNombre = empleado.NombreCompleto,
                        EmpleadoEmail = empleado.Email,
                        ClienteDestinoId = v.ClienteDestinoId,
                        ClienteNombre = cliente?.Nombre ?? "Desconocido",
                        UbicacionDestinoId = v.UbicacionDestinoId,
                        UbicacionNombre = ubicacion?.Ciudad ?? "Desconocido",
                        FechaInicio = v.FechaInicio,
                        FechaFin = v.FechaFin,
                        Estado = v.Estado,
                        TieneConflictos = v.TieneConflictos,
                        Observaciones = v.Observaciones
                    };
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalles del empleado {EmpleadoId}", id);
            return NotFound();
        }
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
