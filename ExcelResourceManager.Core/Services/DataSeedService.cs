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
                return;
            }

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
                return;
            }

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
                Activo = true
            },
            new()
            {
                Id = 2,
                Nombre = "Cliente Quito",
                UbicacionId = 2,
                CodigoInterno = "CLI-UIO-001",
                Email = "contacto@clientequito.com",
                Activo = true
            },
            new()
            {
                Id = 3,
                Nombre = "Cliente Asunción",
                UbicacionId = 3,
                CodigoInterno = "CLI-ASU-001",
                Email = "contacto@clienteasuncion.com",
                Activo = true
            }
        };

        foreach (var cliente in clientes)
        {
            await _unitOfWork.Clientes.InsertAsync(cliente);
        }
    }

    private async Task SeedEmpleadosAsync()
    {
        var empleados = new List<Empleado>
        {
            // Guayaquil - 12 empleados
            new() { Id = 1, Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 1, 15), Activo = true },
            new() { Id = 2, Nombre = "María", Apellido = "González", Email = "maria.gonzalez@empresa.com", UbicacionId = 1, Rol = "QA", FechaIngreso = new DateTime(2024, 2, 1), Activo = true },
            new() { Id = 3, Nombre = "Carlos", Apellido = "Rodríguez", Email = "carlos.rodriguez@empresa.com", UbicacionId = 1, Rol = "Arquitecto", FechaIngreso = new DateTime(2023, 5, 10), Activo = true },
            new() { Id = 4, Nombre = "Ana", Apellido = "Martínez", Email = "ana.martinez@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 3, 20), Activo = true },
            new() { Id = 5, Nombre = "Luis", Apellido = "López", Email = "luis.lopez@empresa.com", UbicacionId = 1, Rol = "Gerente", FechaIngreso = new DateTime(2022, 8, 1), Activo = true },
            new() { Id = 6, Nombre = "Carmen", Apellido = "Sánchez", Email = "carmen.sanchez@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 1, 10), Activo = true },
            new() { Id = 7, Nombre = "Pedro", Apellido = "Ramírez", Email = "pedro.ramirez@empresa.com", UbicacionId = 1, Rol = "QA", FechaIngreso = new DateTime(2023, 11, 5), Activo = true },
            new() { Id = 8, Nombre = "Laura", Apellido = "Torres", Email = "laura.torres@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 4, 1), Activo = true },
            new() { Id = 9, Nombre = "Miguel", Apellido = "Flores", Email = "miguel.flores@empresa.com", UbicacionId = 1, Rol = "Analista", FechaIngreso = new DateTime(2023, 9, 15), Activo = true },
            new() { Id = 10, Nombre = "Isabel", Apellido = "Vargas", Email = "isabel.vargas@empresa.com", UbicacionId = 1, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 2, 20), Activo = true },
            new() { Id = 11, Nombre = "Roberto", Apellido = "Castro", Email = "roberto.castro@empresa.com", UbicacionId = 1, Rol = "DevOps", FechaIngreso = new DateTime(2023, 7, 1), Activo = true },
            new() { Id = 12, Nombre = "Patricia", Apellido = "Méndez", Email = "patricia.mendez@empresa.com", UbicacionId = 1, Rol = "Scrum Master", FechaIngreso = new DateTime(2023, 10, 10), Activo = true },
            
            // Quito - 8 empleados
            new() { Id = 13, Nombre = "Diego", Apellido = "Morales", Email = "diego.morales@empresa.com", UbicacionId = 2, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 1, 5), Activo = true },
            new() { Id = 14, Nombre = "Gabriela", Apellido = "Herrera", Email = "gabriela.herrera@empresa.com", UbicacionId = 2, Rol = "QA", FechaIngreso = new DateTime(2023, 12, 1), Activo = true },
            new() { Id = 15, Nombre = "Fernando", Apellido = "Ortiz", Email = "fernando.ortiz@empresa.com", UbicacionId = 2, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 3, 15), Activo = true },
            new() { Id = 16, Nombre = "Sofía", Apellido = "Ruiz", Email = "sofia.ruiz@empresa.com", UbicacionId = 2, Rol = "Arquitecto", FechaIngreso = new DateTime(2023, 6, 20), Activo = true },
            new() { Id = 17, Nombre = "Andrés", Apellido = "Jiménez", Email = "andres.jimenez@empresa.com", UbicacionId = 2, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 2, 10), Activo = true },
            new() { Id = 18, Nombre = "Valentina", Apellido = "Navarro", Email = "valentina.navarro@empresa.com", UbicacionId = 2, Rol = "Gerente", FechaIngreso = new DateTime(2022, 9, 1), Activo = true },
            new() { Id = 19, Nombre = "Javier", Apellido = "Reyes", Email = "javier.reyes@empresa.com", UbicacionId = 2, Rol = "Desarrollador", FechaIngreso = new DateTime(2024, 4, 5), Activo = true },
            new() { Id = 20, Nombre = "Daniela", Apellido = "Romero", Email = "daniela.romero@empresa.com", UbicacionId = 2, Rol = "Analista", FechaIngreso = new DateTime(2023, 11, 20), Activo = true }
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
            // Guayaquil
            new() { Id = 1, EmpleadoId = 1, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 2, EmpleadoId = 2, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 75m, Activa = true },
            new() { Id = 3, EmpleadoId = 3, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 50m, Activa = true },
            new() { Id = 4, EmpleadoId = 4, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 10), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 5, EmpleadoId = 6, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 15), PorcentajeAsignacion = 80m, Activa = true },
            new() { Id = 6, EmpleadoId = 7, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 20), PorcentajeAsignacion = 60m, Activa = true },
            new() { Id = 7, EmpleadoId = 8, ClienteId = 1, FechaInicio = new DateTime(2026, 2, 1), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 8, EmpleadoId = 9, ClienteId = 1, FechaInicio = new DateTime(2026, 2, 5), PorcentajeAsignacion = 50m, Activa = true },
            new() { Id = 9, EmpleadoId = 10, ClienteId = 1, FechaInicio = new DateTime(2026, 2, 10), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 10, EmpleadoId = 11, ClienteId = 1, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 40m, Activa = true },
            
            // Quito
            new() { Id = 11, EmpleadoId = 13, ClienteId = 2, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 12, EmpleadoId = 14, ClienteId = 2, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 80m, Activa = true },
            new() { Id = 13, EmpleadoId = 15, ClienteId = 2, FechaInicio = new DateTime(2026, 1, 10), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 14, EmpleadoId = 16, ClienteId = 2, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 50m, Activa = true },
            new() { Id = 15, EmpleadoId = 17, ClienteId = 2, FechaInicio = new DateTime(2026, 1, 15), PorcentajeAsignacion = 100m, Activa = true },
            new() { Id = 16, EmpleadoId = 19, ClienteId = 2, FechaInicio = new DateTime(2026, 2, 1), PorcentajeAsignacion = 90m, Activa = true },
            new() { Id = 17, EmpleadoId = 20, ClienteId = 2, FechaInicio = new DateTime(2026, 2, 5), PorcentajeAsignacion = 60m, Activa = true },
            
            // Asunción
            new() { Id = 18, EmpleadoId = 5, ClienteId = 3, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 30m, Activa = true },
            new() { Id = 19, EmpleadoId = 12, ClienteId = 3, FechaInicio = new DateTime(2026, 1, 10), PorcentajeAsignacion = 40m, Activa = true },
            new() { Id = 20, EmpleadoId = 18, ClienteId = 3, FechaInicio = new DateTime(2026, 1, 5), PorcentajeAsignacion = 50m, Activa = true }
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
            
            // Ana (id=4) - conflicto con turno soporte
            new() { Id = 4, EmpleadoId = 4, FechaInicio = new DateTime(2026, 5, 1), FechaFin = new DateTime(2026, 5, 8), Estado = EstadoVacacion.Solicitada, DiasHabiles = 6, TieneConflictos = true, Observaciones = "Conflicto con turno de soporte del 4-10 de mayo" },
            
            // Luis (id=5) - vacaciones durante feriado
            new() { Id = 5, EmpleadoId = 5, FechaInicio = new DateTime(2026, 12, 20), FechaFin = new DateTime(2026, 12, 31), Estado = EstadoVacacion.Aprobada, DiasHabiles = 8, TieneConflictos = false, Observaciones = "Vacaciones de fin de año" },
            
            // Carmen (id=6) - sin conflictos
            new() { Id = 6, EmpleadoId = 6, FechaInicio = new DateTime(2026, 6, 15), FechaFin = new DateTime(2026, 6, 20), Estado = EstadoVacacion.Aprobada, DiasHabiles = 5, TieneConflictos = false, Observaciones = "" },
            
            // Pedro (id=7) - conflicto con vacaciones de Laura
            new() { Id = 7, EmpleadoId = 7, FechaInicio = new DateTime(2026, 7, 10), FechaFin = new DateTime(2026, 7, 17), Estado = EstadoVacacion.Aprobada, DiasHabiles = 6, TieneConflictos = true, Observaciones = "Conflicto: múltiples empleados de vacaciones mismo período" },
            
            // Laura (id=8) - conflicto con vacaciones de Pedro
            new() { Id = 8, EmpleadoId = 8, FechaInicio = new DateTime(2026, 7, 14), FechaFin = new DateTime(2026, 7, 21), Estado = EstadoVacacion.Aprobada, DiasHabiles = 6, TieneConflictos = true, Observaciones = "Conflicto: múltiples empleados de vacaciones mismo período" },
            
            // Miguel (id=9) - sin conflictos
            new() { Id = 9, EmpleadoId = 9, FechaInicio = new DateTime(2026, 8, 5), FechaFin = new DateTime(2026, 8, 12), Estado = EstadoVacacion.Aprobada, DiasHabiles = 6, TieneConflictos = false, Observaciones = "" },
            
            // Isabel (id=10) - conflicto con viaje
            new() { Id = 10, EmpleadoId = 10, FechaInicio = new DateTime(2026, 9, 1), FechaFin = new DateTime(2026, 9, 10), Estado = EstadoVacacion.Aprobada, DiasHabiles = 8, TieneConflictos = true, Observaciones = "Conflicto con viaje del 8-12 de septiembre" },
            
            // Diego (id=13) - conflicto con turno soporte
            new() { Id = 11, EmpleadoId = 13, FechaInicio = new DateTime(2026, 4, 20), FechaFin = new DateTime(2026, 4, 25), Estado = EstadoVacacion.Solicitada, DiasHabiles = 5, TieneConflictos = true, Observaciones = "Conflicto con turno de soporte del 20-26 de abril" },
            
            // Gabriela (id=14) - sin conflictos
            new() { Id = 12, EmpleadoId = 14, FechaInicio = new DateTime(2026, 10, 5), FechaFin = new DateTime(2026, 10, 12), Estado = EstadoVacacion.Aprobada, DiasHabiles = 6, TieneConflictos = false, Observaciones = "" },
            
            // Fernando (id=15) - conflicto con viaje
            new() { Id = 13, EmpleadoId = 15, FechaInicio = new DateTime(2026, 11, 2), FechaFin = new DateTime(2026, 11, 8), Estado = EstadoVacacion.Aprobada, DiasHabiles = 5, TieneConflictos = true, Observaciones = "Conflicto con viaje del 5-10 de noviembre" },
            
            // Andrés (id=17) - vacaciones durante feriado
            new() { Id = 14, EmpleadoId = 17, FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 1, 5), Estado = EstadoVacacion.Aprobada, DiasHabiles = 2, TieneConflictos = false, Observaciones = "Vacaciones de año nuevo" },
            
            // Javier (id=19) - conflicto con turno soporte
            new() { Id = 15, EmpleadoId = 19, FechaInicio = new DateTime(2026, 6, 1), FechaFin = new DateTime(2026, 6, 7), Estado = EstadoVacacion.Solicitada, DiasHabiles = 5, TieneConflictos = true, Observaciones = "Conflicto con turno de soporte del 1-7 de junio" }
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
            
            // Ana (id=4) - viaje durante feriado
            new() { Id = 3, EmpleadoId = 4, ClienteDestinoId = 3, UbicacionDestinoId = 3, FechaInicio = new DateTime(2026, 12, 24), FechaFin = new DateTime(2026, 12, 28), Estado = EstadoViaje.Planificado, TieneConflictos = false, Observaciones = "Viaje a Asunción durante Navidad" },
            
            // Carmen (id=6) - sin conflictos
            new() { Id = 4, EmpleadoId = 6, ClienteDestinoId = 2, UbicacionDestinoId = 2, FechaInicio = new DateTime(2026, 5, 10), FechaFin = new DateTime(2026, 5, 15), Estado = EstadoViaje.Confirmado, TieneConflictos = false, Observaciones = "Viaje a Quito" },
            
            // Miguel (id=9) - sin conflictos
            new() { Id = 5, EmpleadoId = 9, ClienteDestinoId = 3, UbicacionDestinoId = 3, FechaInicio = new DateTime(2026, 6, 20), FechaFin = new DateTime(2026, 6, 25), Estado = EstadoViaje.Planificado, TieneConflictos = false, Observaciones = "Viaje a Asunción" },
            
            // Isabel (id=10) - conflicto con vacaciones
            new() { Id = 6, EmpleadoId = 10, ClienteDestinoId = 2, UbicacionDestinoId = 2, FechaInicio = new DateTime(2026, 9, 8), FechaFin = new DateTime(2026, 9, 12), Estado = EstadoViaje.Confirmado, TieneConflictos = true, Observaciones = "Viaje a Quito - Conflicto con vacaciones" },
            
            // Diego (id=13) - sin conflictos
            new() { Id = 7, EmpleadoId = 13, ClienteDestinoId = 1, UbicacionDestinoId = 1, FechaInicio = new DateTime(2026, 7, 5), FechaFin = new DateTime(2026, 7, 10), Estado = EstadoViaje.Planificado, TieneConflictos = false, Observaciones = "Viaje a Guayaquil" },
            
            // Fernando (id=15) - conflicto con vacaciones
            new() { Id = 8, EmpleadoId = 15, ClienteDestinoId = 3, UbicacionDestinoId = 3, FechaInicio = new DateTime(2026, 11, 5), FechaFin = new DateTime(2026, 11, 10), Estado = EstadoViaje.Confirmado, TieneConflictos = true, Observaciones = "Viaje a Asunción - Conflicto con vacaciones" },
            
            // Andrés (id=17) - sin conflictos
            new() { Id = 9, EmpleadoId = 17, ClienteDestinoId = 1, UbicacionDestinoId = 1, FechaInicio = new DateTime(2026, 8, 15), FechaFin = new DateTime(2026, 8, 20), Estado = EstadoViaje.Planificado, TieneConflictos = false, Observaciones = "Viaje a Guayaquil" },
            
            // Roberto (id=11) - viaje durante feriado
            new() { Id = 10, EmpleadoId = 11, ClienteDestinoId = 3, UbicacionDestinoId = 3, FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 1, 5), Estado = EstadoViaje.Planificado, TieneConflictos = false, Observaciones = "Viaje a Asunción durante año nuevo" }
        };

        foreach (var viaje in viajes)
        {
            await _unitOfWork.Viajes.InsertAsync(viaje);
        }
    }

    private async Task SeedTurnosSoporteAsync()
    {
        var turnosSoporte = new List<TurnoSoporte>
        {
            // María (id=2) - conflicto con vacaciones
            new() { Id = 1, EmpleadoId = 2, FechaInicio = new DateTime(2026, 3, 17), FechaFin = new DateTime(2026, 3, 23), NumeroSemana = 12, Año = 2026 },
            
            // Ana (id=4) - conflicto con vacaciones
            new() { Id = 2, EmpleadoId = 4, FechaInicio = new DateTime(2026, 5, 4), FechaFin = new DateTime(2026, 5, 10), NumeroSemana = 19, Año = 2026 },
            
            // Pedro (id=7) - sin conflictos
            new() { Id = 3, EmpleadoId = 7, FechaInicio = new DateTime(2026, 2, 2), FechaFin = new DateTime(2026, 2, 8), NumeroSemana = 6, Año = 2026 },
            
            // Roberto (id=11) - sin conflictos
            new() { Id = 4, EmpleadoId = 11, FechaInicio = new DateTime(2026, 2, 9), FechaFin = new DateTime(2026, 2, 15), NumeroSemana = 7, Año = 2026 },
            
            // Diego (id=13) - conflicto con vacaciones
            new() { Id = 5, EmpleadoId = 13, FechaInicio = new DateTime(2026, 4, 20), FechaFin = new DateTime(2026, 4, 26), NumeroSemana = 17, Año = 2026 },
            
            // Gabriela (id=14) - sin conflictos
            new() { Id = 6, EmpleadoId = 14, FechaInicio = new DateTime(2026, 2, 16), FechaFin = new DateTime(2026, 2, 22), NumeroSemana = 8, Año = 2026 },
            
            // Javier (id=19) - conflicto con vacaciones
            new() { Id = 7, EmpleadoId = 19, FechaInicio = new DateTime(2026, 6, 1), FechaFin = new DateTime(2026, 6, 7), NumeroSemana = 23, Año = 2026 },
            
            // Carmen (id=6) - sin conflictos
            new() { Id = 8, EmpleadoId = 6, FechaInicio = new DateTime(2026, 2, 23), FechaFin = new DateTime(2026, 3, 1), NumeroSemana = 9, Año = 2026 },
            
            // Miguel (id=9) - sin conflictos
            new() { Id = 9, EmpleadoId = 9, FechaInicio = new DateTime(2026, 3, 2), FechaFin = new DateTime(2026, 3, 8), NumeroSemana = 10, Año = 2026 },
            
            // Fernando (id=15) - sin conflictos
            new() { Id = 10, EmpleadoId = 15, FechaInicio = new DateTime(2026, 3, 9), FechaFin = new DateTime(2026, 3, 15), NumeroSemana = 11, Año = 2026 }
        };

        foreach (var turno in turnosSoporte)
        {
            await _unitOfWork.TurnosSoporte.InsertAsync(turno);
        }
    }
}
