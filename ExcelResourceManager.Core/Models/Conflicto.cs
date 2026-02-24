using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Core.Models;

public class Conflicto
{
    public int Id { get; set; }
    public TipoConflicto Tipo { get; set; }
    public NivelConflicto Nivel { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaConflicto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Recomendacion { get; set; } = string.Empty;
    public bool Resuelto { get; set; }
    public int? VacacionId { get; set; }
    public int? ViajeId { get; set; }
    public int? TurnoSoporteId { get; set; }
}
