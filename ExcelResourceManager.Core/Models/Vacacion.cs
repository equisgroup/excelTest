using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Core.Models;

public class Vacacion
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoVacacion Estado { get; set; }
    public int DiasHabiles => (int)(FechaFin.Date - FechaInicio.Date).TotalDays + 1;
    public string Observaciones { get; set; } = string.Empty;
}
