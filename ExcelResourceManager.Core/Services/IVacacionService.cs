using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IVacacionService
{
    Task<Vacacion?> ObtenerPorIdAsync(int id);
    Task<List<Vacacion>> ObtenerTodasAsync();
    Task<List<Vacacion>> ObtenerActivasAsync();
    Task<List<Vacacion>> ObtenerPorEmpleadoAsync(int empleadoId);
    Task<int> CrearAsync(Vacacion vacacion);
    Task<bool> ActualizarAsync(Vacacion vacacion);
    Task<bool> EliminarAsync(int id);
    Task<int> CalcularDiasHabilesAsync(DateTime fechaInicio, DateTime fechaFin, int ubicacionId);
}
