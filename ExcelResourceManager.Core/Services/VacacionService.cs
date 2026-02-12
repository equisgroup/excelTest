using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class VacacionService : IVacacionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeriadoService _feriadoService;
    
    public VacacionService(IUnitOfWork unitOfWork, IFeriadoService feriadoService)
    {
        _unitOfWork = unitOfWork;
        _feriadoService = feriadoService;
    }
    
    public async Task<Vacacion?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Vacaciones.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener vacación {VacacionId}", id);
            throw;
        }
    }
    
    public async Task<List<Vacacion>> ObtenerTodasAsync()
    {
        try
        {
            var vacaciones = await _unitOfWork.Vacaciones.GetAllAsync();
            return vacaciones.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener todas las vacaciones");
            throw;
        }
    }
    
    public async Task<List<Vacacion>> ObtenerActivasAsync()
    {
        try
        {
            var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => 
                v.Estado != Enums.EstadoVacacion.Cancelada && 
                v.Estado != Enums.EstadoVacacion.Rechazada);
            return vacaciones.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener vacaciones activas");
            throw;
        }
    }
    
    public async Task<List<Vacacion>> ObtenerPorEmpleadoAsync(int empleadoId)
    {
        try
        {
            var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => v.EmpleadoId == empleadoId);
            return vacaciones.OrderByDescending(v => v.FechaInicio).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener vacaciones del empleado {EmpleadoId}", empleadoId);
            throw;
        }
    }
    
    public async Task<int> CrearAsync(Vacacion vacacion)
    {
        try
        {
            var id = await _unitOfWork.Vacaciones.InsertAsync(vacacion);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear vacación para empleado {EmpleadoId}", vacacion.EmpleadoId);
            throw;
        }
    }
    
    public async Task<bool> ActualizarAsync(Vacacion vacacion)
    {
        try
        {
            var resultado = await _unitOfWork.Vacaciones.UpdateAsync(vacacion);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar vacación {VacacionId}", vacacion.Id);
            throw;
        }
    }
    
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var resultado = await _unitOfWork.Vacaciones.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar vacación {VacacionId}", id);
            throw;
        }
    }
    
    public async Task<int> CalcularDiasHabilesAsync(DateTime fechaInicio, DateTime fechaFin, int ubicacionId)
    {
        return await _feriadoService.CalcularDiasHabilesAsync(fechaInicio, fechaFin, ubicacionId);
    }
}
