using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelResourceManager.Web.Controllers;

public class ViajesController : Controller
{
    private readonly IViajeService _viajeService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IClienteService _clienteService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;
    private readonly ILogger<ViajesController> _logger;

    public ViajesController(
        IViajeService viajeService,
        IEmpleadoService empleadoService,
        IClienteService clienteService,
        IUnitOfWork unitOfWork,
        IValidationService validationService,
        ILogger<ViajesController> logger)
    {
        _viajeService = viajeService;
        _empleadoService = empleadoService;
        _clienteService = clienteService;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? empleadoId)
    {
        try
        {
            IEnumerable<Viaje> viajes;
            if (empleadoId.HasValue)
                viajes = await _unitOfWork.Viajes.FindAsync(v => v.EmpleadoId == empleadoId.Value);
            else
                viajes = await _viajeService.ObtenerTodosAsync();

            var empleados = await _empleadoService.ObtenerTodosAsync();
            var clientes = await _clienteService.ObtenerTodosAsync();
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            
            // Crear ViewModels con información del empleado, cliente y ubicación
            var viajesViewModel = viajes.Select(v =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == v.EmpleadoId);
                var cliente = clientes.FirstOrDefault(c => c.Id == v.ClienteDestinoId);
                var ubicacion = ubicaciones.FirstOrDefault(u => u.Id == v.UbicacionDestinoId);
                
                return new ViajeViewModel
                {
                    Id = v.Id,
                    EmpleadoId = v.EmpleadoId,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido",
                    EmpleadoEmail = empleado?.Email ?? "",
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
            }).ToList();

            ViewBag.EmpleadoId = empleadoId;
            return View(viajesViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar viajes");
            return View(new List<ViajeViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? empleadoId)
    {
        try
        {
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            ViewBag.EmpleadoId = empleadoId;

            var viaje = new Viaje();
            if (empleadoId.HasValue)
                viaje.EmpleadoId = empleadoId.Value;

            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar formulario de creación");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Viaje viaje, int? empleadoId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Validar conflictos (solo para mostrar advertencia, no se guardan)
                var conflictos = await _validationService.ValidarViajeAsync(viaje);
                viaje.TieneConflictos = conflictos.Any();
                
                // Crear el viaje
                var viajeId = await _viajeService.CrearAsync(viaje);
                
                if (conflictos.Any())
                {
                    TempData["Warning"] = $"Viaje creado con {conflictos.Count} conflicto(s) detectado(s). Puedes verlos en la página de Conflictos.";
                }
                else
                {
                    TempData["Success"] = "Viaje creado exitosamente";
                }

                if (empleadoId.HasValue)
                    return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });
                
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            ViewBag.EmpleadoId = empleadoId;
            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear viaje");
            TempData["Error"] = "Error al crear el viaje";
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            ViewBag.EmpleadoId = empleadoId;
            return View(viaje);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var viaje = await _viajeService.ObtenerPorIdAsync(id);
            if (viaje == null)
            {
                TempData["Error"] = "Viaje no encontrado";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar viaje para editar");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Viaje viaje)
    {
        try
        {
            if (id != viaje.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _viajeService.ActualizarAsync(viaje);
                TempData["Success"] = "Viaje actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar viaje");
            TempData["Error"] = "Error al actualizar el viaje";
            ViewBag.Empleados = new SelectList(await _empleadoService.ObtenerActivosAsync(), "Id", "NombreCompleto");
            ViewBag.Clientes = new SelectList(await _clienteService.ObtenerActivosAsync(), "Id", "Nombre");
            ViewBag.Ubicaciones = new SelectList(await _unitOfWork.Ubicaciones.GetAllAsync(), "Id", "Ciudad");
            return View(viaje);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int? empleadoId)
    {
        try
        {
            await _viajeService.EliminarAsync(id);
            TempData["Success"] = "Viaje eliminado exitosamente";

            if (empleadoId.HasValue)
                return RedirectToAction("Details", "Empleados", new { id = empleadoId.Value });

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar viaje");
            TempData["Error"] = "Error al eliminar el viaje";
            return RedirectToAction(nameof(Index));
        }
    }
}
