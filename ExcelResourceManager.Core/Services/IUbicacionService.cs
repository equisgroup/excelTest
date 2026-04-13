using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IUbicacionService
{
    Task<List<Ubicacion>> ObtenerTodasAsync();
    Task<Ubicacion?> ObtenerPorIdAsync(int id);
    Task<int> AgregarUbicacionAsync(Ubicacion ubicacion);
    Task<bool> ActualizarUbicacionAsync(Ubicacion ubicacion);
    Task<bool> EliminarUbicacionAsync(int id);
}
