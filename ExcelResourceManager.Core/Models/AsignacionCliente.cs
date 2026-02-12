namespace ExcelResourceManager.Core.Models;

public class AsignacionCliente
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal PorcentajeAsignacion { get; set; }
    public bool Activa { get; set; } = true;
}
