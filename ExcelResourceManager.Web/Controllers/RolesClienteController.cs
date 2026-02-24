using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class RolesClienteController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RolesClienteController> _logger;

    public RolesClienteController(IUnitOfWork unitOfWork, ILogger<RolesClienteController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var rolesCliente = await _unitOfWork.RolesCliente.GetAllAsync();
        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        
        var viewModels = rolesCliente.Select(rc =>
        {
            var cliente = clientes.FirstOrDefault(c => c.Id == rc.ClienteId);
            return new RolClienteViewModel
            {
                Id = rc.Id,
                ClienteId = rc.ClienteId,
                ClienteNombre = cliente?.Nombre ?? "Desconocido",
                Rol = rc.Rol,
                CantidadRequerida = rc.CantidadRequerida,
                FechaInicio = rc.FechaInicio,
                FechaFin = rc.FechaFin
            };
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        ViewBag.Clientes = clientes.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolCliente rolCliente)
    {
        if (!ModelState.IsValid)
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            ViewBag.Clientes = clientes.ToList();
            return View(rolCliente);
        }

        try
        {
            await _unitOfWork.RolesCliente.InsertAsync(rolCliente);
            await _unitOfWork.CommitAsync();
            
            TempData["Success"] = "Rol del cliente creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando rol del cliente");
            ModelState.AddModelError("", "Error al crear el rol del cliente");
            
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            ViewBag.Clientes = clientes.ToList();
            return View(rolCliente);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rolCliente = await _unitOfWork.RolesCliente.GetByIdAsync(id);
        if (rolCliente == null)
        {
            return NotFound();
        }

        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        ViewBag.Clientes = clientes.ToList();
        return View(rolCliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RolCliente rolCliente)
    {
        if (id != rolCliente.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            ViewBag.Clientes = clientes.ToList();
            return View(rolCliente);
        }

        try
        {
            await _unitOfWork.RolesCliente.UpdateAsync(rolCliente);
            await _unitOfWork.CommitAsync();
            
            TempData["Success"] = "Rol del cliente actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando rol del cliente");
            ModelState.AddModelError("", "Error al actualizar el rol del cliente");
            
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            ViewBag.Clientes = clientes.ToList();
            return View(rolCliente);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var rolCliente = await _unitOfWork.RolesCliente.GetByIdAsync(id);
            if (rolCliente == null)
            {
                return NotFound();
            }

            await _unitOfWork.RolesCliente.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            
            TempData["Success"] = "Rol del cliente eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando rol del cliente");
            TempData["Error"] = "Error al eliminar el rol del cliente";
        }

        return RedirectToAction(nameof(Index));
    }
}
