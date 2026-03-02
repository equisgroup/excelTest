using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Web.Models;

public class ClienteDetailsViewModel
{
    public Cliente Cliente { get; set; } = null!;
    public List<RolClienteViewModel> Roles { get; set; } = new();
    public List<AsignacionClienteViewModel> Asignaciones { get; set; } = new();
}
