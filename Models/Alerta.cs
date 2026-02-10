namespace ExcelDashboardGenerator.Models;

public class Alerta
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // VacacionViaje, VacacionSoporte, ViajesSoporte, FeriadoViaje, etc.
    public string Nivel { get; set; } = string.Empty; // Alta, Media, Baja
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public DateTime FechaConflicto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Detalles { get; set; } = string.Empty;
    public bool Resuelta { get; set; }
}
