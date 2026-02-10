using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Data.Repositories;
using Serilog;

namespace ExcelResourceManager.Core.Services;

public class FeriadoService : IFeriadoService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public FeriadoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task CargarFeriadosAñoAsync(int año)
    {
        try
        {
            if (año == 2026)
            {
                await CargarFeriados2026Async();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al cargar feriados del año {Año}", año);
            throw;
        }
    }
    
    private async Task CargarFeriados2026Async()
    {
        var feriados = new List<Feriado>();
        
        // Ecuador - Ubicación 1 (Guayaquil), 2 (Quito)
        var feriadosEcuador = new List<(DateTime Fecha, string Nombre, bool EsNacional, int? UbicacionLocal)>
        {
            (new DateTime(2026, 1, 1), "Año Nuevo", true, null),
            (new DateTime(2026, 2, 12), "Carnaval - Lunes", true, null),
            (new DateTime(2026, 2, 13), "Carnaval - Martes", true, null),
            (new DateTime(2026, 4, 18), "Viernes Santo", true, null),
            (new DateTime(2026, 5, 1), "Día del Trabajo", true, null),
            (new DateTime(2026, 5, 24), "Batalla de Pichincha", true, null),
            (new DateTime(2026, 10, 9), "Independencia de Guayaquil", false, 1),
            (new DateTime(2026, 11, 2), "Día de los Difuntos", true, null),
            (new DateTime(2026, 12, 6), "Fundación de Quito", false, 2),
            (new DateTime(2026, 12, 25), "Navidad", true, null)
        };
        
        foreach (var (fecha, nombre, esNacional, ubicacionLocal) in feriadosEcuador)
        {
            if (ubicacionLocal.HasValue)
            {
                feriados.Add(new Feriado
                {
                    UbicacionId = ubicacionLocal.Value,
                    Fecha = fecha,
                    Nombre = nombre,
                    EsNacional = esNacional,
                    Año = 2026
                });
            }
            else
            {
                // Agregar para todas las ubicaciones de Ecuador (1, 2)
                feriados.Add(new Feriado { UbicacionId = 1, Fecha = fecha, Nombre = nombre, EsNacional = esNacional, Año = 2026 });
                feriados.Add(new Feriado { UbicacionId = 2, Fecha = fecha, Nombre = nombre, EsNacional = esNacional, Año = 2026 });
            }
        }
        
        // Paraguay - Ubicación 3 (Asunción)
        var feriadosParaguay = new List<(DateTime Fecha, string Nombre)>
        {
            (new DateTime(2026, 1, 1), "Año Nuevo"),
            (new DateTime(2026, 3, 1), "Día de los Héroes"),
            (new DateTime(2026, 4, 17), "Jueves Santo"),
            (new DateTime(2026, 4, 18), "Viernes Santo"),
            (new DateTime(2026, 5, 1), "Día del Trabajo"),
            (new DateTime(2026, 5, 15), "Independencia Nacional"),
            (new DateTime(2026, 6, 12), "Paz del Chaco"),
            (new DateTime(2026, 8, 15), "Fundación de Asunción"),
            (new DateTime(2026, 12, 8), "Virgen de Caacupé"),
            (new DateTime(2026, 12, 25), "Navidad")
        };
        
        foreach (var (fecha, nombre) in feriadosParaguay)
        {
            feriados.Add(new Feriado
            {
                UbicacionId = 3,
                Fecha = fecha,
                Nombre = nombre,
                EsNacional = true,
                Año = 2026
            });
        }
        
        // Insertar todos los feriados
        foreach (var feriado in feriados)
        {
            await _unitOfWork.Feriados.InsertAsync(feriado);
        }
        
        await _unitOfWork.CommitAsync();
    }
    
    public async Task<bool> EsFeriadoAsync(int ubicacionId, DateTime fecha)
    {
        try
        {
            var feriados = await _unitOfWork.Feriados.FindAsync(f => 
                f.UbicacionId == ubicacionId && 
                f.Fecha.Date == fecha.Date);
            
            return feriados.Any();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al verificar si {Fecha} es feriado para ubicación {UbicacionId}", fecha, ubicacionId);
            throw;
        }
    }
    
    public async Task<int> CalcularDiasHabilesAsync(DateTime fechaInicio, DateTime fechaFin, int ubicacionId)
    {
        try
        {
            var diasHabiles = 0;
            var fechaActual = fechaInicio.Date;
            
            var feriados = await _unitOfWork.Feriados.FindAsync(f => 
                f.UbicacionId == ubicacionId && 
                f.Fecha >= fechaInicio.Date && 
                f.Fecha <= fechaFin.Date);
            
            var fechasFeriados = feriados.Select(f => f.Fecha.Date).ToHashSet();
            
            while (fechaActual <= fechaFin.Date)
            {
                // Excluir sábados (6) y domingos (0)
                if (fechaActual.DayOfWeek != DayOfWeek.Saturday && 
                    fechaActual.DayOfWeek != DayOfWeek.Sunday &&
                    !fechasFeriados.Contains(fechaActual))
                {
                    diasHabiles++;
                }
                
                fechaActual = fechaActual.AddDays(1);
            }
            
            return diasHabiles;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al calcular días hábiles desde {FechaInicio} hasta {FechaFin}", fechaInicio, fechaFin);
            throw;
        }
    }
    
    public async Task<List<Feriado>> ObtenerFeriadosPorUbicacionAsync(int ubicacionId, int año)
    {
        try
        {
            var feriados = await _unitOfWork.Feriados.FindAsync(f => 
                f.UbicacionId == ubicacionId && f.Año == año);
            
            return feriados.OrderBy(f => f.Fecha).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener feriados para ubicación {UbicacionId} y año {Año}", ubicacionId, año);
            throw;
        }
    }
}
