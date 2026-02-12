using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelResourceManager.Web.Controllers;

public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return View(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar clientes");
            return View(new List<Cliente>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        try
        {
            if (ModelState.IsValid)
            {
                cliente.Activo = true;
                await _clienteService.CrearAsync(cliente);
                TempData["Success"] = "Cliente creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente");
            ModelState.AddModelError("", "Error al crear el cliente");
        }
        return View(cliente);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound();
            return View(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar cliente {id}");
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                await _clienteService.ActualizarAsync(cliente);
                TempData["Success"] = "Cliente actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar cliente {id}");
            ModelState.AddModelError("", "Error al actualizar el cliente");
        }
        return View(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _clienteService.EliminarAsync(id);
            TempData["Success"] = "Cliente eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar cliente {id}");
            TempData["Error"] = "Error al eliminar el cliente";
        }
        return RedirectToAction(nameof(Index));
    }
}
