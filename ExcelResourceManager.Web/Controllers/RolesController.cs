using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class RolesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IUnitOfWork unitOfWork, ILogger<RolesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        return View(roles.OrderBy(r => r.Nombre).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rol rol)
    {
        if (!ModelState.IsValid)
            return View(rol);

        try
        {
            var existentes = await _unitOfWork.Roles.GetAllAsync();
            if (existentes.Any(r => r.Nombre.Equals(rol.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Nombre", "Ya existe un rol con ese nombre.");
                return View(rol);
            }

            rol.Activo = true;
            await _unitOfWork.Roles.InsertAsync(rol);
            await _unitOfWork.CommitAsync();
            TempData["Success"] = "Rol creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            ModelState.AddModelError("", "Error al crear el rol");
            return View(rol);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var rol = await _unitOfWork.Roles.GetByIdAsync(id);
        if (rol == null)
            return NotFound();
        return View(rol);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rol rol)
    {
        if (id != rol.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(rol);

        try
        {
            var existentes = await _unitOfWork.Roles.GetAllAsync();
            if (existentes.Any(r => r.Id != id && r.Nombre.Equals(rol.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Nombre", "Ya existe un rol con ese nombre.");
                return View(rol);
            }

            await _unitOfWork.Roles.UpdateAsync(rol);
            await _unitOfWork.CommitAsync();
            TempData["Success"] = "Rol actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol {RolId}", id);
            ModelState.AddModelError("", "Error al actualizar el rol");
            return View(rol);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            var rol = await _unitOfWork.Roles.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            rol.Activo = !rol.Activo;
            await _unitOfWork.Roles.UpdateAsync(rol);
            await _unitOfWork.CommitAsync();
            TempData["Success"] = rol.Activo ? "Rol activado" : "Rol desactivado";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del rol {RolId}", id);
            TempData["Error"] = "Error al cambiar el estado del rol";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var rol = await _unitOfWork.Roles.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            await _unitOfWork.Roles.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            TempData["Success"] = "Rol eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar rol {RolId}", id);
            TempData["Error"] = "Error al eliminar el rol";
        }
        return RedirectToAction(nameof(Index));
    }
}
