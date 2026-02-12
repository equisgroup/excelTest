using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Core.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Ubicacion> Ubicaciones { get; }
    IRepository<Cliente> Clientes { get; }
    IRepository<Empleado> Empleados { get; }
    IRepository<AsignacionCliente> AsignacionesCliente { get; }
    IRepository<Vacacion> Vacaciones { get; }
    IRepository<Viaje> Viajes { get; }
    IRepository<TurnoSoporte> TurnosSoporte { get; }
    IRepository<Feriado> Feriados { get; }
    IRepository<Conflicto> Conflictos { get; }
    
    Task CommitAsync();
}
