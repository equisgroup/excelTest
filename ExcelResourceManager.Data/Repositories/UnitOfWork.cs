using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;

namespace ExcelResourceManager.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LiteDbContext _context;
    private IRepository<Ubicacion>? _ubicaciones;
    private IRepository<Cliente>? _clientes;
    private IRepository<Empleado>? _empleados;
    private IRepository<AsignacionCliente>? _asignacionesCliente;
    private IRepository<Vacacion>? _vacaciones;
    private IRepository<Viaje>? _viajes;
    private IRepository<TurnoSoporte>? _turnosSoporte;
    private IRepository<Feriado>? _feriados;
    private IRepository<Conflicto>? _conflictos;
    
    public UnitOfWork(LiteDbContext context)
    {
        _context = context;
    }
    
    public IRepository<Ubicacion> Ubicaciones => 
        _ubicaciones ??= new Repository<Ubicacion>(_context);
    
    public IRepository<Cliente> Clientes => 
        _clientes ??= new Repository<Cliente>(_context);
    
    public IRepository<Empleado> Empleados => 
        _empleados ??= new Repository<Empleado>(_context);
    
    public IRepository<AsignacionCliente> AsignacionesCliente => 
        _asignacionesCliente ??= new Repository<AsignacionCliente>(_context);
    
    public IRepository<Vacacion> Vacaciones => 
        _vacaciones ??= new Repository<Vacacion>(_context);
    
    public IRepository<Viaje> Viajes => 
        _viajes ??= new Repository<Viaje>(_context);
    
    public IRepository<TurnoSoporte> TurnosSoporte => 
        _turnosSoporte ??= new Repository<TurnoSoporte>(_context);
    
    public IRepository<Feriado> Feriados => 
        _feriados ??= new Repository<Feriado>(_context);
    
    public IRepository<Conflicto> Conflictos => 
        _conflictos ??= new Repository<Conflicto>(_context);
    
    public Task CommitAsync()
    {
        // LiteDB auto-commits, so this is a no-op
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _context?.Dispose();
    }
}
