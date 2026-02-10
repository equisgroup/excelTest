using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class ValidationService : IValidationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeriadoService _feriadoService;
    
    public ValidationService(IUnitOfWork unitOfWork, IFeriadoService feriadoService)
    {
        _unitOfWork = unitOfWork;
        _feriadoService = feriadoService;
    }
    
    public async Task<List<Conflicto>> ValidarVacacionAsync(Vacacion vacacion)
    {
        var conflictos = new List<Conflicto>();
        
        try
        {
            var empleado = await _unitOfWork.Empleados.GetByIdAsync(vacacion.EmpleadoId);
            if (empleado == null) return conflictos;
            
            // 1. Verificar conflicto con viajes del mismo empleado
            var viajes = await _unitOfWork.Viajes.FindAsync(v => 
                v.EmpleadoId == vacacion.EmpleadoId && 
                v.Estado != EstadoViaje.Cancelado);
            
            foreach (var viaje in viajes.Where(v => v.Id != vacacion.Id))
            {
                if (HaySolapamiento(vacacion.FechaInicio, vacacion.FechaFin, viaje.FechaInicio, viaje.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.VacacionVsViaje,
                        Nivel = NivelConflicto.Critico,
                        EmpleadoId = vacacion.EmpleadoId,
                        FechaConflicto = vacacion.FechaInicio,
                        Descripcion = $"Vacación se solapa con viaje programado del {viaje.FechaInicio:dd/MM/yyyy} al {viaje.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Cancelar o reprogramar el viaje o la vacación",
                        VacacionId = vacacion.Id,
                        ViajeId = viaje.Id
                    });
                }
            }
            
            // 2. Verificar conflicto con turnos de soporte
            var turnos = await _unitOfWork.TurnosSoporte.FindAsync(t => t.EmpleadoId == vacacion.EmpleadoId);
            
            foreach (var turno in turnos)
            {
                if (HaySolapamiento(vacacion.FechaInicio, vacacion.FechaFin, turno.FechaInicio, turno.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.VacacionVsSoporte,
                        Nivel = NivelConflicto.Critico,
                        EmpleadoId = vacacion.EmpleadoId,
                        FechaConflicto = vacacion.FechaInicio,
                        Descripcion = $"Vacación se solapa con turno de soporte del {turno.FechaInicio:dd/MM/yyyy} al {turno.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Reasignar el turno de soporte a otro empleado",
                        VacacionId = vacacion.Id,
                        TurnoSoporteId = turno.Id
                    });
                }
            }
            
            // 3. Verificar si cae en feriado (informativo)
            if (empleado.UbicacionId > 0)
            {
                var fechaActual = vacacion.FechaInicio.Date;
                while (fechaActual <= vacacion.FechaFin.Date)
                {
                    if (await _feriadoService.EsFeriadoAsync(empleado.UbicacionId, fechaActual))
                    {
                        conflictos.Add(new Conflicto
                        {
                            Tipo = TipoConflicto.VacacionEnFeriado,
                            Nivel = NivelConflicto.Bajo,
                            EmpleadoId = vacacion.EmpleadoId,
                            FechaConflicto = fechaActual,
                            Descripcion = $"La vacación incluye el feriado {fechaActual:dd/MM/yyyy}",
                            Recomendacion = "Considerar ajustar las fechas para optimizar días de vacación",
                            VacacionId = vacacion.Id
                        });
                        break; // Solo reportar uno
                    }
                    fechaActual = fechaActual.AddDays(1);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al validar vacación {VacacionId}", vacacion.Id);
        }
        
        return conflictos;
    }
    
    public async Task<List<Conflicto>> ValidarViajeAsync(Viaje viaje)
    {
        var conflictos = new List<Conflicto>();
        
        try
        {
            var empleado = await _unitOfWork.Empleados.GetByIdAsync(viaje.EmpleadoId);
            if (empleado == null) return conflictos;
            
            // 1. Verificar conflicto con vacaciones del mismo empleado
            var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => 
                v.EmpleadoId == viaje.EmpleadoId && 
                v.Estado != EstadoVacacion.Rechazada && 
                v.Estado != EstadoVacacion.Cancelada);
            
            foreach (var vacacion in vacaciones.Where(v => v.Id != viaje.Id))
            {
                if (HaySolapamiento(viaje.FechaInicio, viaje.FechaFin, vacacion.FechaInicio, vacacion.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.VacacionVsViaje,
                        Nivel = NivelConflicto.Critico,
                        EmpleadoId = viaje.EmpleadoId,
                        FechaConflicto = viaje.FechaInicio,
                        Descripcion = $"Viaje se solapa con vacación programada del {vacacion.FechaInicio:dd/MM/yyyy} al {vacacion.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Cancelar o reprogramar el viaje o la vacación",
                        ViajeId = viaje.Id,
                        VacacionId = vacacion.Id
                    });
                }
            }
            
            // 2. Verificar conflicto con turnos de soporte
            var turnos = await _unitOfWork.TurnosSoporte.FindAsync(t => t.EmpleadoId == viaje.EmpleadoId);
            
            foreach (var turno in turnos)
            {
                if (HaySolapamiento(viaje.FechaInicio, viaje.FechaFin, turno.FechaInicio, turno.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.ViajeVsSoporte,
                        Nivel = NivelConflicto.Medio,
                        EmpleadoId = viaje.EmpleadoId,
                        FechaConflicto = viaje.FechaInicio,
                        Descripcion = $"Viaje se solapa con turno de soporte del {turno.FechaInicio:dd/MM/yyyy} al {turno.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Considerar soporte remoto durante el viaje o reasignar el turno",
                        ViajeId = viaje.Id,
                        TurnoSoporteId = turno.Id
                    });
                }
            }
            
            // 3. Verificar si viaje es en feriado destino
            if (viaje.UbicacionDestinoId > 0)
            {
                var fechaActual = viaje.FechaInicio.Date;
                while (fechaActual <= viaje.FechaFin.Date)
                {
                    if (await _feriadoService.EsFeriadoAsync(viaje.UbicacionDestinoId, fechaActual))
                    {
                        conflictos.Add(new Conflicto
                        {
                            Tipo = TipoConflicto.ViajeEnFeriado,
                            Nivel = NivelConflicto.Bajo,
                            EmpleadoId = viaje.EmpleadoId,
                            FechaConflicto = fechaActual,
                            Descripcion = $"El viaje incluye el feriado {fechaActual:dd/MM/yyyy} en destino",
                            Recomendacion = "Verificar disponibilidad del cliente durante el feriado",
                            ViajeId = viaje.Id
                        });
                        break;
                    }
                    fechaActual = fechaActual.AddDays(1);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al validar viaje {ViajeId}", viaje.Id);
        }
        
        return conflictos;
    }
    
    public async Task<List<Conflicto>> ValidarTurnoSoporteAsync(TurnoSoporte turnoSoporte)
    {
        var conflictos = new List<Conflicto>();
        
        try
        {
            // 1. Verificar conflicto con vacaciones
            var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => 
                v.EmpleadoId == turnoSoporte.EmpleadoId && 
                v.Estado != EstadoVacacion.Rechazada && 
                v.Estado != EstadoVacacion.Cancelada);
            
            foreach (var vacacion in vacaciones)
            {
                if (HaySolapamiento(turnoSoporte.FechaInicio, turnoSoporte.FechaFin, vacacion.FechaInicio, vacacion.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.VacacionVsSoporte,
                        Nivel = NivelConflicto.Critico,
                        EmpleadoId = turnoSoporte.EmpleadoId,
                        FechaConflicto = turnoSoporte.FechaInicio,
                        Descripcion = $"Turno de soporte se solapa con vacación del {vacacion.FechaInicio:dd/MM/yyyy} al {vacacion.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Reasignar el turno a otro empleado",
                        TurnoSoporteId = turnoSoporte.Id,
                        VacacionId = vacacion.Id
                    });
                }
            }
            
            // 2. Verificar conflicto con viajes
            var viajes = await _unitOfWork.Viajes.FindAsync(v => 
                v.EmpleadoId == turnoSoporte.EmpleadoId && 
                v.Estado != EstadoViaje.Cancelado);
            
            foreach (var viaje in viajes)
            {
                if (HaySolapamiento(turnoSoporte.FechaInicio, turnoSoporte.FechaFin, viaje.FechaInicio, viaje.FechaFin))
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.ViajeVsSoporte,
                        Nivel = NivelConflicto.Medio,
                        EmpleadoId = turnoSoporte.EmpleadoId,
                        FechaConflicto = turnoSoporte.FechaInicio,
                        Descripcion = $"Turno de soporte se solapa con viaje del {viaje.FechaInicio:dd/MM/yyyy} al {viaje.FechaFin:dd/MM/yyyy}",
                        Recomendacion = "Considerar soporte remoto durante el viaje",
                        TurnoSoporteId = turnoSoporte.Id,
                        ViajeId = viaje.Id
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al validar turno soporte {TurnoId}", turnoSoporte.Id);
        }
        
        return conflictos;
    }
    
    public async Task<List<Conflicto>> ValidarTodosAsync()
    {
        var conflictos = new List<Conflicto>();
        
        try
        {
            // Validar sobreasignación
            var empleados = await _unitOfWork.Empleados.GetAllAsync();
            foreach (var empleado in empleados.Where(e => e.Activo))
            {
                var asignaciones = await _unitOfWork.AsignacionesCliente.FindAsync(a => 
                    a.EmpleadoId == empleado.Id && a.Activa);
                
                var porcentajeTotal = asignaciones.Sum(a => a.PorcentajeAsignacion);
                
                if (porcentajeTotal > 100)
                {
                    conflictos.Add(new Conflicto
                    {
                        Tipo = TipoConflicto.Sobreasignacion,
                        Nivel = NivelConflicto.Alto,
                        EmpleadoId = empleado.Id,
                        FechaConflicto = DateTime.Now,
                        Descripcion = $"Empleado {empleado.NombreCompleto} está sobreasignado con {porcentajeTotal}%",
                        Recomendacion = "Reducir asignaciones o redistribuir carga de trabajo",
                        Resuelto = false
                    });
                }
            }
            
            // Validar todas las vacaciones
            var vacaciones = await _unitOfWork.Vacaciones.GetAllAsync();
            foreach (var vacacion in vacaciones.Where(v => v.Estado != EstadoVacacion.Cancelada && v.Estado != EstadoVacacion.Rechazada))
            {
                var conflictosVacacion = await ValidarVacacionAsync(vacacion);
                conflictos.AddRange(conflictosVacacion);
            }
            
            // Validar todos los viajes
            var viajes = await _unitOfWork.Viajes.GetAllAsync();
            foreach (var viaje in viajes.Where(v => v.Estado != EstadoViaje.Cancelado))
            {
                var conflictosViaje = await ValidarViajeAsync(viaje);
                conflictos.AddRange(conflictosViaje);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al validar todos los conflictos");
        }
        
        return conflictos.DistinctBy(c => $"{c.Tipo}-{c.EmpleadoId}-{c.VacacionId}-{c.ViajeId}-{c.TurnoSoporteId}").ToList();
    }
    
    private bool HaySolapamiento(DateTime inicio1, DateTime fin1, DateTime inicio2, DateTime fin2)
    {
        return inicio1.Date <= fin2.Date && fin1.Date >= inicio2.Date;
    }
}
