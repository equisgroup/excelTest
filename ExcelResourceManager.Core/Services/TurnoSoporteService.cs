using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class TurnoSoporteService : ITurnoSoporteService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public TurnoSoporteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<TurnoSoporte?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.TurnosSoporte.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener turno soporte {TurnoId}", id);
            throw;
        }
    }
    
    public async Task<List<TurnoSoporte>> ObtenerTodosAsync()
    {
        try
        {
            var turnos = await _unitOfWork.TurnosSoporte.GetAllAsync();
            return turnos.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener todos los turnos de soporte");
            throw;
        }
    }
    
    public async Task<List<TurnoSoporte>> ObtenerPorEmpleadoAsync(int empleadoId)
    {
        try
        {
            var turnos = await _unitOfWork.TurnosSoporte.FindAsync(t => t.EmpleadoId == empleadoId);
            return turnos.OrderByDescending(t => t.FechaInicio).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener turnos del empleado {EmpleadoId}", empleadoId);
            throw;
        }
    }
    
    public async Task<int> CrearAsync(TurnoSoporte turnoSoporte)
    {
        try
        {
            var id = await _unitOfWork.TurnosSoporte.InsertAsync(turnoSoporte);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear turno soporte para empleado {EmpleadoId}", turnoSoporte.EmpleadoId);
            throw;
        }
    }
    
    public async Task<bool> ActualizarAsync(TurnoSoporte turnoSoporte)
    {
        try
        {
            var resultado = await _unitOfWork.TurnosSoporte.UpdateAsync(turnoSoporte);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar turno soporte {TurnoId}", turnoSoporte.Id);
            throw;
        }
    }
    
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var resultado = await _unitOfWork.TurnosSoporte.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar turno soporte {TurnoId}", id);
            throw;
        }
    }
}
