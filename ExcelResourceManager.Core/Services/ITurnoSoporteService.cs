using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface ITurnoSoporteService
{
    Task<TurnoSoporte?> ObtenerPorIdAsync(int id);
    Task<List<TurnoSoporte>> ObtenerTodosAsync();
    Task<List<TurnoSoporte>> ObtenerPorEmpleadoAsync(int empleadoId);
    Task<int> CrearAsync(TurnoSoporte turnoSoporte);
    Task<bool> ActualizarAsync(TurnoSoporte turnoSoporte);
    Task<bool> EliminarAsync(int id);
}
