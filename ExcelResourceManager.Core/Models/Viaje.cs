using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Core.Models;

public class Viaje
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int ClienteDestinoId { get; set; }
    public int UbicacionDestinoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoViaje Estado { get; set; }
    public bool TieneConflictos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}
