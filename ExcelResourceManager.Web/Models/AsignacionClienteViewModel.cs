namespace ExcelResourceManager.Web.Models;

public class AsignacionClienteViewModel
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal PorcentajeAsignacion { get; set; }
    public bool Activa { get; set; }
}
