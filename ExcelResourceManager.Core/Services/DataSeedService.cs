using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class DataSeedService : IDataSeedService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeriadoService _feriadoService;

    public DataSeedService(IUnitOfWork unitOfWork, IFeriadoService feriadoService)
    {
        _unitOfWork = unitOfWork;
        _feriadoService = feriadoService;
    }

    public async Task SeedTestDataAsync()
    {
        try
        {
            Log.Information("Iniciando carga de datos de prueba...");

            var ubicacionesExistentes = await _unitOfWork.Ubicaciones.GetAllAsync();
            if (ubicacionesExistentes.Any())
            {
                Log.Information("Los datos de prueba ya existen. Omitiendo la carga.");
                await EnsureRolesSeedAsync();
                return;
            }

            await SeedRolesAsync();
            await SeedUbicacionesAsync();
            await SeedClientesAsync();
            await SeedEmpleadosAsync();
            await SeedAsignacionesClienteAsync();
            await SeedVacacionesAsync();
            await SeedViajesAsync();
            await SeedTurnosSoporteAsync();
            
            await _feriadoService.CargarFeriadosAñoAsync(2026);

            await _unitOfWork.CommitAsync();
            
            Log.Information("Datos de prueba cargados exitosamente.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar datos de prueba");
            throw;
        }
    }

    public async Task SeedProdDataAsync()
    {
        try
        {
            Log.Information("Iniciando carga de datos de producción...");

            var ubicacionesExistentes = await _unitOfWork.Ubicaciones.GetAllAsync();
            if (ubicacionesExistentes.Any())
            {
                Log.Information("Los datos de producción ya existen. Omitiendo la carga.");
                await EnsureRolesSeedAsync();
                return;
            }

            await SeedRolesAsync();
            await SeedUbicacionesAsync();
            await _feriadoService.CargarFeriadosAñoAsync(2026);

            await _unitOfWork.CommitAsync();
            
            Log.Information("Datos de producción cargados exitosamente.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar datos de producción");
            throw;
        }
    }

    private async Task SeedUbicacionesAsync()
    {
        var ubicaciones = new List<Ubicacion>
        {
            new()
            {
                Id = 1,
                Pais = "Ecuador",
                CodigoPais = "EC",
                Ciudad = "Guayaquil",
                ZonaHoraria = "America/Guayaquil",
                Activo = true
            },
            new()
            {
                Id = 2,
                Pais = "Ecuador",
                CodigoPais = "EC",
                Ciudad = "Quito",
                ZonaHoraria = "America/Guayaquil",
                Activo = true
            },
            new()
            {
                Id = 3,
                Pais = "Paraguay",
                CodigoPais = "PY",
                Ciudad = "Asunción",
                ZonaHoraria = "America/Asuncion",
                Activo = true
            }
        };

        foreach (var ubicacion in ubicaciones)
        {
            await _unitOfWork.Ubicaciones.InsertAsync(ubicacion);
        }
    }

    private async Task SeedClientesAsync()
    {
        var clientes = new List<Cliente>
        {
            new()
            {
                Id = 1,
                Nombre = "Cliente Guayaquil",
                UbicacionId = 1,
                CodigoInterno = "CLI-GYE-001",
                Email = "contacto@clienteguayaquil.com",
                FechaContratoInicio = new DateTime(2024, 1, 1),
                FechaContratoFin = new DateTime(2025, 12, 31),
                Activo = true
            },
            new()
            {
                Id = 2,
                Nombre = "Cliente Quito",
                UbicacionId = 2,
                CodigoInterno = "CLI-UIO-001",
                Email = "contacto@clientequito.com",
                FechaContratoInicio = new DateTime(2024, 3, 1),
                FechaContratoFin = new DateTime(2026, 2, 28),
                Activo = true
            },
            new()
            {
                Id = 3,
                Nombre = "Cliente Asunción",
                UbicacionId = 3,
                CodigoInterno = "CLI-ASU-001",
                Email = "contacto@clienteasuncion.com",
                FechaContratoInicio = new DateTime(2024, 6, 1),
                FechaContratoFin = new DateTime(2026, 5, 31),
                Activo = true
            }
        };

        foreach (var cliente in clientes)
        {
            await _unitOfWork.Clientes.InsertAsync(cliente);
        }
        
        // Seed Roles Cliente
        await SeedRolesClienteAsync();
    }
    
    private async Task SeedRolesClienteAsync()
    {
        var rolesCliente = new List<RolCliente>
        {
            new() { Id = 1, ClienteId = 1, Rol = "Desarrollador Backend", CantidadRequerida = 3, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2025, 12, 31) },
            new() { Id = 2, ClienteId = 1, Rol = "Desarrollador Frontend", CantidadRequerida = 2, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2025, 12, 31) },
            new() { Id = 3, ClienteId = 1, Rol = "QA", CantidadRequerida = 1, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2025, 12, 31) },
            new() { Id = 4, ClienteId = 2, Rol = "Arquitecto", CantidadRequerida = 1, FechaInicio = new DateTime(2024, 3, 1), FechaFin = new DateTime(2026, 2, 28) },
            new() { Id = 5, ClienteId = 2, Rol = "Desarrollador", CantidadRequerida = 4, FechaInicio = new DateTime(2024, 3, 1), FechaFin = new DateTime(2026, 2, 28) },
            new() { Id = 6, ClienteId = 3, Rol = "Desarrollador", CantidadRequerida = 2, FechaInicio = new DateTime(2024, 6, 1), FechaFin = new DateTime(2026, 5, 31) },
            new() { Id = 7, ClienteId = 3, Rol = "DevOps", CantidadRequerida = 1, FechaInicio = new DateTime(2024, 6, 1), FechaFin = new DateTime(2026, 5, 31) }
        };

        foreach (var rolCliente in rolesCliente)
        {
            await _unitOfWork.RolesCliente.InsertAsync(rolCliente);
        }
    }

    private async Task SeedEmpleadosAsync()
    {
        var empleados = new List<Empleado>
        {
            new() { Id = 1, Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 1, 15), Activo = true },
            
            // Quito - 8 empleados
            new() { Id = 2, Nombre = "Diego", Apellido = "Morales", Email = "diego.morales@empresa.com", UbicacionId = 2, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 1, 5), Activo = true },
            new() { Id = 3, Nombre = "Gabriela", Apellido = "Herrera", Email = "gabriela.herrera@empresa.com", UbicacionId = 2, Rol = "QA", FechaIngreso = new DateTime(2023, 12, 1), Activo = true },
        };

        foreach (var empleado in empleados)
        {
            await _unitOfWork.Empleados.InsertAsync(empleado);
        }
    }

    private async Task SeedAsignacionesClienteAsync()
    {
        var asignaciones = new List<AsignacionCliente>
        {
            // Guayaquil - Cliente 1
            new() { Id = 1, EmpleadoId = 1, ClienteId = 1, Rol = "Desarrollador Backend", FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 2, EmpleadoId = 2, ClienteId = 2, Rol = "QA", FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 75m, Activa = true },
            new() { Id = 3, EmpleadoId = 3, ClienteId = 3, Rol = "Arquitecto", FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 50m, Activa = true },
        };

        foreach (var asignacion in asignaciones)
        {
            await _unitOfWork.AsignacionesCliente.InsertAsync(asignacion);
        }
    }

    private async Task SeedVacacionesAsync()
    {
        var vacaciones = new List<Vacacion>
        {
            // Juan (id=1) - conflicto con viaje
            new() { Id = 1, EmpleadoId = 1, FechaInicio = new DateTime(2026, 3, 15), FechaFin = new DateTime(2026, 3, 20), Estado = EstadoVacacion.Aprobada, DiasHabiles = 4, TieneConflictos = true, Observaciones = "Conflicto con viaje del 18-20 de marzo" },
            
            // María (id=2) - conflicto con turno soporte
            new() { Id = 2, EmpleadoId = 2, FechaInicio = new DateTime(2026, 3, 20), FechaFin = new DateTime(2026, 3, 25), Estado = EstadoVacacion.Aprobada, DiasHabiles = 4, TieneConflictos = true, Observaciones = "Conflicto con turno de soporte del 17-23 de marzo" },
            
            // Carlos (id=3) - conflicto con viaje a Quito
            new() { Id = 3, EmpleadoId = 3, FechaInicio = new DateTime(2026, 4, 10), FechaFin = new DateTime(2026, 4, 15), Estado = EstadoVacacion.Aprobada, DiasHabiles = 4, TieneConflictos = true, Observaciones = "Conflicto con viaje a Quito del 12-16 de abril" },
            
            // Carlos (id=3) - conflicto con viaje a Quito
            new() { Id = 4, EmpleadoId = 3, FechaInicio = new DateTime(2026, 4, 10), FechaFin = new DateTime(2026, 4, 20), Estado = EstadoVacacion.Aprobada, DiasHabiles = 4, TieneConflictos = true, Observaciones = "Conflicto con viaje a Quito del 12-16 de abril" },
        };

        foreach (var vacacion in vacaciones)
        {
            await _unitOfWork.Vacaciones.InsertAsync(vacacion);
        }
    }

    private async Task SeedViajesAsync()
    {
        var viajes = new List<Viaje>
        {
            // Juan (id=1) - conflicto con vacaciones
            new() { Id = 1, EmpleadoId = 1, ClienteDestinoId = 2, UbicacionDestinoId = 2, FechaInicio = new DateTime(2026, 3, 18), FechaFin = new DateTime(2026, 3, 20), Estado = EstadoViaje.Confirmado, TieneConflictos = true, Observaciones = "Viaje a Quito - Conflicto con vacaciones" },
            
            // Carlos (id=3) - conflicto con vacaciones
            new() { Id = 2, EmpleadoId = 3, ClienteDestinoId = 2, UbicacionDestinoId = 2, FechaInicio = new DateTime(2026, 4, 12), FechaFin = new DateTime(2026, 4, 16), Estado = EstadoViaje.Planificado, TieneConflictos = true, Observaciones = "Viaje a Quito - Conflicto con vacaciones" },
            
        };

        foreach (var viaje in viajes)
        {
            await _unitOfWork.Viajes.InsertAsync(viaje);
        }
    }

    public async Task EnsureRolesSeedAsync()
    {
        var existentes = await _unitOfWork.Roles.GetAllAsync();
        if (!existentes.Any())
        {
            Log.Information("Seeding roles iniciales...");
            await SeedRolesAsync();
            await _unitOfWork.CommitAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<Rol>
        {
            new() { Nombre = "Analista", Activo = true },
            new() { Nombre = "Arquitecto", Activo = true },
            new() { Nombre = "Consultor", Activo = true },
            new() { Nombre = "DevOps", Activo = true },
            new() { Nombre = "Desarrollador", Activo = true },
            new() { Nombre = "Desarrollador Backend", Activo = true },
            new() { Nombre = "Desarrollador Frontend", Activo = true },
            new() { Nombre = "Desarrollador Full Stack", Activo = true },
            new() { Nombre = "Gerente de Proyecto", Activo = true },
            new() { Nombre = "Líder Técnico", Activo = true },
            new() { Nombre = "QA", Activo = true },
            new() { Nombre = "Scrum Master", Activo = true },
            new() { Nombre = "UX/UI Designer", Activo = true },
        };

        foreach (var rol in roles)
        {
            await _unitOfWork.Roles.InsertAsync(rol);
        }
    }

    private async Task SeedTurnosSoporteAsync()
    {
        var turnosSoporte = new List<TurnoSoporte>
        {
            new() { Id = 1, EmpleadoId = 2, FechaInicio = new DateTime(2026, 3, 17), FechaFin = new DateTime(2026, 3, 23), NumeroSemana = 12, Año = 2026 },
            
        };

        foreach (var turno in turnosSoporte)
        {
            await _unitOfWork.TurnosSoporte.InsertAsync(turno);
        }
    }
}
