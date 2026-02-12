using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Services;

public interface IValidationService
{
    Task<List<Conflicto>> ValidarVacacionAsync(Vacacion vacacion);
    Task<List<Conflicto>> ValidarViajeAsync(Viaje viaje);
    Task<List<Conflicto>> ValidarTurnoSoporteAsync(TurnoSoporte turnoSoporte);
    Task<List<Conflicto>> ValidarTodosAsync();
}
