using ExcelDashboardGenerator.Models;

namespace ExcelDashboardGenerator.Data;

public class DataContainer
{
    public List<Cliente> Clientes { get; set; } = new();
    public List<Empleado> Empleados { get; set; } = new();
    public List<Asignacion> Asignaciones { get; set; } = new();
    public List<Vacacion> Vacaciones { get; set; } = new();
    public List<Viaje> Viajes { get; set; } = new();
    public List<TurnoSoporte> TurnosSoporte { get; set; } = new();
    public List<string> Paises { get; set; } = new();
}
