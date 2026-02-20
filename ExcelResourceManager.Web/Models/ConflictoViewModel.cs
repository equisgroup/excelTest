using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Web.Models;

public class ConflictoViewModel
{
    public int Id { get; set; }
    public TipoConflicto Tipo { get; set; }
    public NivelConflicto Nivel { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string EmpleadoEmail { get; set; } = string.Empty;
    public DateTime FechaConflicto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Recomendacion { get; set; } = string.Empty;
    public bool Resuelto { get; set; }
}
