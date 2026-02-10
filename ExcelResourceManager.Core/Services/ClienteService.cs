using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Data.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Clientes.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener cliente {ClienteId}", id);
            throw;
        }
    }
    
    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        try
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            return clientes.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener todos los clientes");
            throw;
        }
    }
    
    public async Task<List<Cliente>> ObtenerActivosAsync()
    {
        try
        {
            var clientes = await _unitOfWork.Clientes.FindAsync(c => c.Activo);
            return clientes.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener clientes activos");
            throw;
        }
    }
    
    public async Task<int> CrearAsync(Cliente cliente)
    {
        try
        {
            var id = await _unitOfWork.Clientes.InsertAsync(cliente);
            await _unitOfWork.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear cliente {Nombre}", cliente.Nombre);
            throw;
        }
    }
    
    public async Task<bool> ActualizarAsync(Cliente cliente)
    {
        try
        {
            var resultado = await _unitOfWork.Clientes.UpdateAsync(cliente);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al actualizar cliente {ClienteId}", cliente.Id);
            throw;
        }
    }
    
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var resultado = await _unitOfWork.Clientes.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return resultado;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar cliente {ClienteId}", id);
            throw;
        }
    }
}
