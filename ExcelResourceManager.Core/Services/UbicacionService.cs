using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class UbicacionService : IUbicacionService
{
    private readonly IUnitOfWork _unitOfWork;

    public UbicacionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Ubicacion>> ObtenerTodasAsync()
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            return ubicaciones.OrderBy(u => u.Pais).ThenBy(u => u.Ciudad).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener ubicaciones");
            throw;
        }
    }

    public async Task<Ubicacion?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Ubicaciones.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener ubicación {Id}", id);
            throw;
        }
    }

    public async Task<int> AgregarUbicacionAsync(Ubicacion ubicacion)
    {
        try
        {
            ubicacion.Activo = true;
            var id = await _unitOfWork.Ubicaciones.InsertAsync(ubicacion);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al agregar ubicación {Ciudad}", ubicacion.Ciudad);
            throw;
        }
    }

    public async Task<bool> ActualizarUbicacionAsync(Ubicacion ubicacion)
    {
        try
        {
            var result = await _unitOfWork.Ubicaciones.UpdateAsync(ubicacion);
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar ubicación {Id}", ubicacion.Id);
            throw;
        }
    }

    public async Task<bool> EliminarUbicacionAsync(int id)
    {
        try
        {
            // Validate no holidays reference this location
            var feriados = await _unitOfWork.Feriados.FindAsync(f => f.UbicacionId == id);
            if (feriados.Any())
                throw new InvalidOperationException("No se puede eliminar la ubicación porque tiene feriados asociados.");

            // Validate no employees reference this location
            var empleados = await _unitOfWork.Empleados.FindAsync(e => e.UbicacionId == id);
            if (empleados.Any())
                throw new InvalidOperationException("No se puede eliminar la ubicación porque tiene empleados asociados.");

            var result = await _unitOfWork.Ubicaciones.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            Log.Error(ex, "Error al eliminar ubicación {Id}", id);
            throw;
        }
    }
}
