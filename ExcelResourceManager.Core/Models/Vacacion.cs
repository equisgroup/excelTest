using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Core.Models;

public class Vacacion
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoVacacion Estado { get; set; }
    public int DiasHabiles { get; set; }
    public bool TieneConflictos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}
