namespace ExcelDashboardGenerator.Models;

public class Vacacion
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobada, Rechazada
    public string Observaciones { get; set; } = string.Empty;
}
