using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Web.Models;

public class VacacionViewModel
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EmpleadoEmail { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoVacacion Estado { get; set; }
    public int DiasHabiles { get; set; }
    public bool TieneConflictos { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}
