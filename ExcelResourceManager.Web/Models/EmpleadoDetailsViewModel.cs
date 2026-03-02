using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Web.Models;

public class EmpleadoDetailsViewModel
{
    public Empleado Empleado { get; set; } = null!;
    public List<VacacionViewModel> Vacaciones { get; set; } = new();
    public List<ViajeViewModel> Viajes { get; set; } = new();
}
