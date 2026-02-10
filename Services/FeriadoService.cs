using ExcelDashboardGenerator.Models;
using Nager.Date;

namespace ExcelDashboardGenerator.Services;

public class FeriadoService
{
    public List<Feriado> ObtenerFeriados2026(List<string> paises)
    {
        var feriados = new List<Feriado>();
        int id = 1;
        
        foreach (var paisCodigo in paises)
        {
            try
            {
                // Convertir código de país a CountryCode
                if (Enum.TryParse<CountryCode>(paisCodigo, out var countryCode))
                {
                    var publicHolidays = DateSystem.GetPublicHolidays(2026, countryCode);
                    
                    foreach (var holiday in publicHolidays)
                    {
                        feriados.Add(new Feriado
                        {
                            Id = id++,
                            Pais = paisCodigo,
                            Ciudad = "", // Nager.Date no proporciona información de ciudad
                            Fecha = holiday.Date,
                            Nombre = holiday.LocalName ?? holiday.Name,
                            EsNacional = holiday.Global
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Advertencia: No se pudieron cargar feriados para {paisCodigo}: {ex.Message}");
            }
        }
        
        return feriados;
    }
    
    public bool EsFeriado(DateTime fecha, string pais, List<Feriado> feriados)
    {
        return feriados.Any(f => 
            f.Fecha.Date == fecha.Date && 
            f.Pais == pais);
    }
    
    public List<Feriado> ObtenerFeriadosEnRango(DateTime inicio, DateTime fin, string pais, List<Feriado> feriados)
    {
        return feriados.Where(f => 
            f.Pais == pais && 
            f.Fecha.Date >= inicio.Date && 
            f.Fecha.Date <= fin.Date).ToList();
    }
}
