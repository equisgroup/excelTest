using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IClienteService
{
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<List<Cliente>> ObtenerTodosAsync();
    Task<List<Cliente>> ObtenerActivosAsync();
    Task<int> CrearAsync(Cliente cliente);
    Task<bool> ActualizarAsync(Cliente cliente);
    Task<bool> EliminarAsync(int id);
}
