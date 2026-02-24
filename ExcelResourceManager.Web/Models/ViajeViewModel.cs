using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Web.Models;

public class ViajeViewModel
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EmpleadoEmail { get; set; } = string.Empty;
    public int ClienteDestinoId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int UbicacionDestinoId { get; set; }
    public string UbicacionNombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoViaje Estado { get; set; }
    public bool TieneConflictos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}
