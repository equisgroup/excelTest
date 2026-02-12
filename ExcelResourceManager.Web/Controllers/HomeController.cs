using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExcelResourceManager.Web.Models;

namespace ExcelResourceManager.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ToggleMode()
    {
        var currentMode = HttpContext.Session.GetString("Mode") ?? "Prueba";
        var newMode = currentMode == "Prueba" ? "Producción" : "Prueba";
        
        HttpContext.Session.SetString("Mode", newMode);
        
        _logger.LogInformation($"Mode toggled from {currentMode} to {newMode}");
        
        TempData["Success"] = $"Modo cambiado a {newMode}. Por favor, recargue la página para ver los cambios.";
        
        return RedirectToAction("Index", "Dashboard");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
