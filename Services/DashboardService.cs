using ExcelDashboardGenerator.Models;
using ExcelDashboardGenerator.Data;

namespace ExcelDashboardGenerator.Services;

public class DashboardService
{
    // Este servicio proporciona funcionalidades adicionales para dashboards
    // que pueden ser utilizadas por ExcelGeneratorService
    
    public Dictionary<string, int> ObtenerEmpleadosPorPais(DataContainer data)
    {
        return data.Empleados
            .Where(e => e.Activo)
            .GroupBy(e => e.Pais)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<string, int> ObtenerClientesPorPais(DataContainer data)
    {
        return data.Clientes
            .Where(c => c.Activo)
            .GroupBy(c => c.Pais)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<string, int> ObtenerAsignacionesPorCliente(DataContainer data)
    {
        return data.Asignaciones
            .Where(a => a.Activa)
            .GroupBy(a => a.ClienteId)
            .ToDictionary(
                g => data.Clientes.FirstOrDefault(c => c.Id == g.Key)?.Nombre ?? "Desconocido",
                g => g.Count()
            );
    }
    
    public Dictionary<string, int> ObtenerAlertasPorTipo(List<Alerta> alertas)
    {
        return alertas
            .GroupBy(a => a.Tipo)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<string, int> ObtenerAlertasPorNivel(List<Alerta> alertas)
    {
        return alertas
            .GroupBy(a => a.Nivel)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
