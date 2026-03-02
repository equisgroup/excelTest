using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Web.Helpers;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class AsignacionesClienteController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AsignacionesClienteController> _logger;

    public AsignacionesClienteController(IUnitOfWork unitOfWork, ILogger<AsignacionesClienteController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var asignaciones = await _unitOfWork.AsignacionesCliente.GetAllAsync();
            var empleados = await _unitOfWork.Empleados.GetAllAsync();
            var clientes = await _unitOfWork.Clientes.GetAllAsync();

            var viewModels = asignaciones.Select(a =>
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == a.EmpleadoId);
                var cliente = clientes.FirstOrDefault(c => c.Id == a.ClienteId);
                return new AsignacionClienteViewModel
                {
                    Id = a.Id,
                    EmpleadoId = a.EmpleadoId,
                    EmpleadoNombre = empleado?.NombreCompleto ?? "Desconocido",
                    ClienteId = a.ClienteId,
                    ClienteNombre = cliente?.Nombre ?? "Desconocido",
                    Rol = a.Rol,
                    FechaInicio = a.FechaInicio,
                    FechaFin = a.FechaFin,
                    PorcentajeAsignacion = a.PorcentajeAsignacion,
                    Activa = a.Activa
                };
            }).ToList();

            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar asignaciones de cliente");
            return View(new List<AsignacionClienteViewModel>());
        }
    }

    public async Task<IActionResult> Create(int? clienteId, string? rol)
    {
        var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
        var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
        var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

        ViewBag.Empleados = empleados.ToList();
        ViewBag.Clientes = clientes.ToList();
        ViewBag.RolesCliente = rolesCliente.ToList();
        ViewBag.Roles = RolesDisponibles.Roles;
        ViewBag.ClienteId = clienteId;

        var asignacion = new AsignacionCliente
        {
            FechaInicio = DateTime.Today,
            Activa = true,
            PorcentajeAsignacion = 100
        };

        if (clienteId.HasValue)
            asignacion.ClienteId = clienteId.Value;
        if (!string.IsNullOrEmpty(rol))
            asignacion.Rol = rol;

        return View(asignacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsignacionCliente asignacion, int? clienteId)
    {
        if (!ModelState.IsValid)
        {
            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
            var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
            var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

            ViewBag.Empleados = empleados.ToList();
            ViewBag.Clientes = clientes.ToList();
            ViewBag.RolesCliente = rolesCliente.ToList();
            ViewBag.Roles = RolesDisponibles.Roles;
            ViewBag.ClienteId = clienteId;
            return View(asignacion);
        }

        try
        {
            asignacion.Activa = true;
            await _unitOfWork.AsignacionesCliente.InsertAsync(asignacion);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Asignación creada exitosamente";

            if (clienteId.HasValue)
                return RedirectToAction("Details", "Clientes", new { id = clienteId.Value });

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear asignación de cliente");
            ModelState.AddModelError("", "Error al crear la asignación");

            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
            var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
            var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

            ViewBag.Empleados = empleados.ToList();
            ViewBag.Clientes = clientes.ToList();
            ViewBag.RolesCliente = rolesCliente.ToList();
            ViewBag.Roles = RolesDisponibles.Roles;
            ViewBag.ClienteId = clienteId;
            return View(asignacion);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var asignacion = await _unitOfWork.AsignacionesCliente.GetByIdAsync(id);
        if (asignacion == null)
            return NotFound();

        var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
        var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
        var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

        ViewBag.Empleados = empleados.ToList();
        ViewBag.Clientes = clientes.ToList();
        ViewBag.RolesCliente = rolesCliente.ToList();
        ViewBag.Roles = RolesDisponibles.Roles;

        return View(asignacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AsignacionCliente asignacion, int? clienteId)
    {
        if (id != asignacion.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
            var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
            var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

            ViewBag.Empleados = empleados.ToList();
            ViewBag.Clientes = clientes.ToList();
            ViewBag.RolesCliente = rolesCliente.ToList();
            ViewBag.Roles = RolesDisponibles.Roles;
            return View(asignacion);
        }

        try
        {
            await _unitOfWork.AsignacionesCliente.UpdateAsync(asignacion);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Asignación actualizada exitosamente";

            if (clienteId.HasValue)
                return RedirectToAction("Details", "Clientes", new { id = clienteId.Value });

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar asignación {AsignacionId}", id);
            ModelState.AddModelError("", "Error al actualizar la asignación");

            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
            var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
            var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();

            ViewBag.Empleados = empleados.ToList();
            ViewBag.Clientes = clientes.ToList();
            ViewBag.RolesCliente = rolesCliente.ToList();
            ViewBag.Roles = RolesDisponibles.Roles;
            return View(asignacion);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int? clienteId)
    {
        try
        {
            var asignacion = await _unitOfWork.AsignacionesCliente.GetByIdAsync(id);
            if (asignacion == null)
                return NotFound();

            var cid = clienteId ?? asignacion.ClienteId;

            await _unitOfWork.AsignacionesCliente.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Asignación eliminada exitosamente";

            if (clienteId.HasValue)
                return RedirectToAction("Details", "Clientes", new { id = cid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asignación {AsignacionId}", id);
            TempData["Error"] = "Error al eliminar la asignación";
        }

        return RedirectToAction(nameof(Index));
    }
}
