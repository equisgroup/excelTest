using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IFeriadoService
{
    Task CargarFeriadosAñoAsync(int año);
    Task<bool> EsFeriadoAsync(int ubicacionId, DateTime fecha);
    Task<int> CalcularDiasHabilesAsync(DateTime fechaInicio, DateTime fechaFin, int ubicacionId);
    Task<List<Feriado>> ObtenerFeriadosPorUbicacionAsync(int ubicacionId, int año);
    Task<List<Feriado>> ObtenerTodosAsync(int? ubicacionId = null, int? año = null);
    Task<Feriado?> ObtenerPorIdAsync(int id);
    Task<int> AgregarFeriadoAsync(Feriado feriado);
    Task<bool> ActualizarFeriadoAsync(Feriado feriado);
    Task<bool> EliminarFeriadoAsync(int id);
}
