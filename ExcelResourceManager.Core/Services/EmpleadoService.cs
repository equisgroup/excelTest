using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public EmpleadoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Empleados.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener empleado {EmpleadoId}", id);
            throw;
        }
    }
    
    public async Task<List<Empleado>> ObtenerTodosAsync()
    {
        try
        {
            var empleados = await _unitOfWork.Empleados.GetAllAsync();
            return empleados.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener todos los empleados");
            throw;
        }
    }
    
    public async Task<List<Empleado>> ObtenerActivosAsync()
    {
        try
        {
            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.Activo);
            return empleados.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener empleados activos");
            throw;
        }
    }
    
    public async Task<int> CrearAsync(Empleado empleado)
    {
        try
        {
            var id = await _unitOfWork.Empleados.InsertAsync(empleado);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear empleado {Nombre}", empleado.NombreCompleto);
            throw;
        }
    }
    
    public async Task<bool> ActualizarAsync(Empleado empleado)
    {
        try
        {
            var resultado = await _unitOfWork.Empleados.UpdateAsync(empleado);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar empleado {EmpleadoId}", empleado.Id);
            throw;
        }
    }
    
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var resultado = await _unitOfWork.Empleados.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar empleado {EmpleadoId}", id);
            throw;
        }
    }
}
