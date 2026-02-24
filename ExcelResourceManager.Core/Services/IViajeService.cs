using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IViajeService
{
    Task<Viaje?> ObtenerPorIdAsync(int id);
    Task<List<Viaje>> ObtenerTodosAsync();
    Task<List<Viaje>> ObtenerActivosAsync();
    Task<List<Viaje>> ObtenerPorEmpleadoAsync(int empleadoId);
    Task<int> CrearAsync(Viaje viaje);
    Task<bool> ActualizarAsync(Viaje viaje);
    Task<bool> EliminarAsync(int id);
}
