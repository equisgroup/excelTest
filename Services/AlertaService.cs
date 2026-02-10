using ExcelDashboardGenerator.Models;

namespace ExcelDashboardGenerator.Services;

public class AlertaService
{
    public List<Alerta> ConsolidarAlertas(List<Alerta> alertas)
    {
        // Consolidar alertas duplicadas y ordenar por prioridad
        return alertas
            .OrderBy(a => a.Nivel == "Alta" ? 1 : a.Nivel == "Media" ? 2 : 3)
            .ThenBy(a => a.FechaConflicto)
            .ToList();
    }
    
    public Dictionary<string, int> ClasificarPorNivel(List<Alerta> alertas)
    {
        return new Dictionary<string, int>
        {
            { "Alta", alertas.Count(a => a.Nivel == "Alta") },
            { "Media", alertas.Count(a => a.Nivel == "Media") },
            { "Baja", alertas.Count(a => a.Nivel == "Baja") }
        };
    }
    
    public List<string> GenerarRecomendaciones(List<Alerta> alertas)
    {
        var recomendaciones = new List<string>();
        
        var alertasAltas = alertas.Where(a => a.Nivel == "Alta").ToList();
        if (alertasAltas.Any())
        {
            recomendaciones.Add($"⚠️ URGENTE: {alertasAltas.Count} alertas de alta prioridad requieren atención inmediata");
            
            var vacacionesViajes = alertasAltas.Count(a => a.Tipo == "VacacionViaje");
            if (vacacionesViajes > 0)
            {
                recomendaciones.Add($"  • {vacacionesViajes} conflictos entre vacaciones y viajes - Reprogramar uno de los dos");
            }
            
            var vacacionesSoporte = alertasAltas.Count(a => a.Tipo == "VacacionSoporte");
            if (vacacionesSoporte > 0)
            {
                recomendaciones.Add($"  • {vacacionesSoporte} conflictos entre vacaciones y turnos de soporte - Reasignar turno");
            }
            
            var asignacionesMultiples = alertasAltas.Count(a => a.Tipo == "AsignacionMultiple");
            if (asignacionesMultiples > 0)
            {
                recomendaciones.Add($"  • {asignacionesMultiples} empleados con asignaciones múltiples - Revisar carga de trabajo");
            }
        }
        
        var alertasMedias = alertas.Where(a => a.Nivel == "Media").ToList();
        if (alertasMedias.Any())
        {
            recomendaciones.Add($"📋 {alertasMedias.Count} alertas de prioridad media requieren revisión");
        }
        
        var alertasBajas = alertas.Where(a => a.Nivel == "Baja").ToList();
        if (alertasBajas.Any())
        {
            recomendaciones.Add($"ℹ️ {alertasBajas.Count} alertas informativas para consideración");
        }
        
        return recomendaciones;
    }
}
