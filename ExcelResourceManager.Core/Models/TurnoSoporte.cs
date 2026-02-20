namespace ExcelResourceManager.Core.Models;

public class TurnoSoporte
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int NumeroSemana { get; set; }
    public int Año { get; set; }
}
