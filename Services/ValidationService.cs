using ExcelDashboardGenerator.Models;
using ExcelDashboardGenerator.Data;

namespace ExcelDashboardGenerator.Services;

public class ValidationService
{
    private readonly FeriadoService _feriadoService;
    
    public ValidationService()
    {
        _feriadoService = new FeriadoService();
    }
    
    public List<Alerta> ValidarTodo(DataContainer data, List<Feriado> feriados)
    {
        var alertas = new List<Alerta>();
        
        alertas.AddRange(ValidarVacacionesVsViajes(data));
        alertas.AddRange(ValidarVacacionesVsSoporte(data));
        alertas.AddRange(ValidarViajesVsSoporte(data));
        alertas.AddRange(ValidarViajesEnFeriados(data, feriados));
        alertas.AddRange(ValidarVacacionesEnFeriados(data, feriados));
        alertas.AddRange(ValidarAsignacionesMultiples(data));
        
        // Asignar IDs únicos
        for (int i = 0; i < alertas.Count; i++)
        {
            alertas[i].Id = i + 1;
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarVacacionesVsViajes(DataContainer data)
    {
        var alertas = new List<Alerta>();
        
        foreach (var vacacion in data.Vacaciones)
        {
            var viajesConflicto = data.Viajes.Where(v => 
                v.EmpleadoId == vacacion.EmpleadoId &&
                !(v.FechaFin < vacacion.FechaInicio || v.FechaInicio > vacacion.FechaFin)
            ).ToList();
            
            foreach (var viaje in viajesConflicto)
            {
                var empleado = data.Empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
                var cliente = data.Clientes.FirstOrDefault(c => c.Id == viaje.ClienteId);
                
                alertas.Add(new Alerta
                {
                    Tipo = "VacacionViaje",
                    Nivel = "Alta",
                    EmpleadoId = vacacion.EmpleadoId,
                    EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido",
                    FechaConflicto = vacacion.FechaInicio,
                    Descripcion = "Vacaciones y viaje programados en fechas superpuestas",
                    Detalles = $"Vacaciones: {vacacion.FechaInicio:dd/MM/yyyy} - {vacacion.FechaFin:dd/MM/yyyy}, " +
                              $"Viaje a {viaje.CiudadDestino} ({cliente?.Nombre}): {viaje.FechaInicio:dd/MM/yyyy} - {viaje.FechaFin:dd/MM/yyyy}",
                    Resuelta = false
                });
            }
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarVacacionesVsSoporte(DataContainer data)
    {
        var alertas = new List<Alerta>();
        
        foreach (var vacacion in data.Vacaciones)
        {
            var turnosConflicto = data.TurnosSoporte.Where(t => 
                t.EmpleadoId == vacacion.EmpleadoId &&
                !(t.FechaFin < vacacion.FechaInicio || t.FechaInicio > vacacion.FechaFin)
            ).ToList();
            
            foreach (var turno in turnosConflicto)
            {
                var empleado = data.Empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
                
                alertas.Add(new Alerta
                {
                    Tipo = "VacacionSoporte",
                    Nivel = "Alta",
                    EmpleadoId = vacacion.EmpleadoId,
                    EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido",
                    FechaConflicto = vacacion.FechaInicio,
                    Descripcion = "Vacaciones programadas durante turno de soporte",
                    Detalles = $"Vacaciones: {vacacion.FechaInicio:dd/MM/yyyy} - {vacacion.FechaFin:dd/MM/yyyy}, " +
                              $"Turno soporte semana {turno.NumeroSemana}: {turno.FechaInicio:dd/MM/yyyy} - {turno.FechaFin:dd/MM/yyyy}",
                    Resuelta = false
                });
            }
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarViajesVsSoporte(DataContainer data)
    {
        var alertas = new List<Alerta>();
        
        foreach (var viaje in data.Viajes)
        {
            var turnosConflicto = data.TurnosSoporte.Where(t => 
                t.EmpleadoId == viaje.EmpleadoId &&
                !(t.FechaFin < viaje.FechaInicio || t.FechaInicio > viaje.FechaFin)
            ).ToList();
            
            foreach (var turno in turnosConflicto)
            {
                var empleado = data.Empleados.FirstOrDefault(e => e.Id == viaje.EmpleadoId);
                var cliente = data.Clientes.FirstOrDefault(c => c.Id == viaje.ClienteId);
                
                alertas.Add(new Alerta
                {
                    Tipo = "ViajeSoporte",
                    Nivel = "Media",
                    EmpleadoId = viaje.EmpleadoId,
                    EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido",
                    FechaConflicto = viaje.FechaInicio,
                    Descripcion = "Viaje programado durante turno de soporte (puede gestionar remoto)",
                    Detalles = $"Viaje a {viaje.CiudadDestino} ({cliente?.Nombre}): {viaje.FechaInicio:dd/MM/yyyy} - {viaje.FechaFin:dd/MM/yyyy}, " +
                              $"Turno soporte semana {turno.NumeroSemana}: {turno.FechaInicio:dd/MM/yyyy} - {turno.FechaFin:dd/MM/yyyy}",
                    Resuelta = false
                });
            }
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarViajesEnFeriados(DataContainer data, List<Feriado> feriados)
    {
        var alertas = new List<Alerta>();
        
        foreach (var viaje in data.Viajes)
        {
            var feriadosEnViaje = _feriadoService.ObtenerFeriadosEnRango(
                viaje.FechaInicio, 
                viaje.FechaFin, 
                viaje.PaisDestino, 
                feriados
            );
            
            if (feriadosEnViaje.Any())
            {
                var empleado = data.Empleados.FirstOrDefault(e => e.Id == viaje.EmpleadoId);
                var cliente = data.Clientes.FirstOrDefault(c => c.Id == viaje.ClienteId);
                
                var feriadosDesc = string.Join(", ", feriadosEnViaje.Select(f => $"{f.Nombre} ({f.Fecha:dd/MM/yyyy})"));
                
                alertas.Add(new Alerta
                {
                    Tipo = "ViajeEnFeriado",
                    Nivel = "Baja",
                    EmpleadoId = viaje.EmpleadoId,
                    EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido",
                    FechaConflicto = viaje.FechaInicio,
                    Descripcion = "Viaje programado en fecha de feriado en país destino",
                    Detalles = $"Viaje a {viaje.CiudadDestino}, {viaje.PaisDestino} ({cliente?.Nombre}): {viaje.FechaInicio:dd/MM/yyyy} - {viaje.FechaFin:dd/MM/yyyy}. " +
                              $"Feriados: {feriadosDesc}",
                    Resuelta = false
                });
            }
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarVacacionesEnFeriados(DataContainer data, List<Feriado> feriados)
    {
        var alertas = new List<Alerta>();
        
        foreach (var vacacion in data.Vacaciones)
        {
            var empleado = data.Empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
            if (empleado == null) continue;
            
            var feriadosEnVacacion = _feriadoService.ObtenerFeriadosEnRango(
                vacacion.FechaInicio, 
                vacacion.FechaFin, 
                empleado.Pais, 
                feriados
            );
            
            if (feriadosEnVacacion.Any())
            {
                var feriadosDesc = string.Join(", ", feriadosEnVacacion.Select(f => $"{f.Nombre} ({f.Fecha:dd/MM/yyyy})"));
                
                alertas.Add(new Alerta
                {
                    Tipo = "VacacionConFeriado",
                    Nivel = "Baja",
                    EmpleadoId = vacacion.EmpleadoId,
                    EmpleadoNombre = $"{empleado.Nombre} {empleado.Apellido}",
                    FechaConflicto = vacacion.FechaInicio,
                    Descripcion = "Vacaciones incluyen feriados (pueden extenderse automáticamente)",
                    Detalles = $"Vacaciones: {vacacion.FechaInicio:dd/MM/yyyy} - {vacacion.FechaFin:dd/MM/yyyy}. " +
                              $"Feriados incluidos: {feriadosDesc}",
                    Resuelta = false
                });
            }
        }
        
        return alertas;
    }
    
    public List<Alerta> ValidarAsignacionesMultiples(DataContainer data)
    {
        var alertas = new List<Alerta>();
        
        var asignacionesActivas = data.Asignaciones.Where(a => a.Activa).ToList();
        
        var empleadosConMultiples = asignacionesActivas
            .GroupBy(a => a.EmpleadoId)
            .Where(g => g.Count() > 1)
            .ToList();
        
        foreach (var grupo in empleadosConMultiples)
        {
            var empleado = data.Empleados.FirstOrDefault(e => e.Id == grupo.Key);
            var asignaciones = grupo.ToList();
            
            var clientesDesc = string.Join(", ", asignaciones.Select(a => 
            {
                var cliente = data.Clientes.FirstOrDefault(c => c.Id == a.ClienteId);
                return cliente?.Nombre ?? "Desconocido";
            }));
            
            alertas.Add(new Alerta
            {
                Tipo = "AsignacionMultiple",
                Nivel = "Alta",
                EmpleadoId = grupo.Key,
                EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido",
                FechaConflicto = DateTime.Now,
                Descripcion = "Empleado con múltiples asignaciones activas simultáneas",
                Detalles = $"Asignado a {asignaciones.Count} clientes: {clientesDesc}",
                Resuelta = false
            });
        }
        
        return alertas;
    }
}
