using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<IActionResult> Index()
    {
        try
        {
            var viajes = await _viajeService.ObtenerTodosAsync();
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
            
            return View(viajesViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar viajes");
            return View(new List<ViajeViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar formulario de creación");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Viaje viaje)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Validar conflictos
                var conflictos = await _validationService.ValidarViajeAsync(viaje);
                viaje.TieneConflictos = conflictos.Any();
                
                // Crear el viaje
                var viajeId = await _viajeService.CrearAsync(viaje);
                
                // Guardar los conflictos detectados
                foreach (var conflicto in conflictos)
                {
                    conflicto.ViajeId = viajeId;
                    await _unitOfWork.Conflictos.InsertAsync(conflicto);
                }
                await _unitOfWork.CommitAsync();
                
                if (conflictos.Any())
                {
                    TempData["Warning"] = $"Viaje creado con {conflictos.Count} conflicto(s) detectado(s)";
                }
                else
                {
                    TempData["Success"] = "Viaje creado exitosamente";
                }
                
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear viaje");
            TempData["Error"] = "Error al crear el viaje";
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
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
            
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
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
            
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            return View(viaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar viaje");
            TempData["Error"] = "Error al actualizar el viaje";
            ViewBag.Empleados = await _empleadoService.ObtenerActivosAsync();
            ViewBag.Clientes = await _clienteService.ObtenerActivosAsync();
            ViewBag.Ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            return View(viaje);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _viajeService.EliminarAsync(id);
            TempData["Success"] = "Viaje eliminado exitosamente";
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
