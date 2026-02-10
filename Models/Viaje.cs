namespace ExcelDashboardGenerator.Models;

public class Viaje
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int ClienteId { get; set; }
    public string PaisDestino { get; set; } = string.Empty;
    public string CiudadDestino { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Planificado"; // Planificado, En Curso, Completado
}
