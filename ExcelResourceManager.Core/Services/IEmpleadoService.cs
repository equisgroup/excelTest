using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IEmpleadoService
{
    Task<Empleado?> ObtenerPorIdAsync(int id);
    Task<List<Empleado>> ObtenerTodosAsync();
    Task<List<Empleado>> ObtenerActivosAsync();
    Task<int> CrearAsync(Empleado empleado);
    Task<bool> ActualizarAsync(Empleado empleado);
    Task<bool> EliminarAsync(int id);
}
