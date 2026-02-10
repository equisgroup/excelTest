using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Data.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class ViajeService : IViajeService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ViajeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Viaje?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Viajes.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener viaje {ViajeId}", id);
            throw;
        }
    }
    
    public async Task<List<Viaje>> ObtenerTodosAsync()
    {
        try
        {
            var viajes = await _unitOfWork.Viajes.GetAllAsync();
            return viajes.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener todos los viajes");
            throw;
        }
    }
    
    public async Task<List<Viaje>> ObtenerActivosAsync()
    {
        try
        {
            var viajes = await _unitOfWork.Viajes.FindAsync(v => 
                v.Estado != Enums.EstadoViaje.Cancelado);
            return viajes.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener viajes activos");
            throw;
        }
    }
    
    public async Task<List<Viaje>> ObtenerPorEmpleadoAsync(int empleadoId)
    {
        try
        {
            var viajes = await _unitOfWork.Viajes.FindAsync(v => v.EmpleadoId == empleadoId);
            return viajes.OrderByDescending(v => v.FechaInicio).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener viajes del empleado {EmpleadoId}", empleadoId);
            throw;
        }
    }
    
    public async Task<int> CrearAsync(Viaje viaje)
    {
        try
        {
            var id = await _unitOfWork.Viajes.InsertAsync(viaje);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear viaje para empleado {EmpleadoId}", viaje.EmpleadoId);
            throw;
        }
    }
    
    public async Task<bool> ActualizarAsync(Viaje viaje)
    {
        try
        {
            var resultado = await _unitOfWork.Viajes.UpdateAsync(viaje);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar viaje {ViajeId}", viaje.Id);
            throw;
        }
    }
    
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var resultado = await _unitOfWork.Viajes.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar viaje {ViajeId}", id);
            throw;
        }
    }
}
