namespace ExcelDashboardGenerator.Models;

public class Empleado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int? ClienteAsignadoId { get; set; }
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; }
}
