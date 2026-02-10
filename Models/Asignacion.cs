namespace ExcelDashboardGenerator.Models;

public class Asignacion
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public bool Activa { get; set; }
}
