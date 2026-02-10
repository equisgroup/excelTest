namespace ExcelDashboardGenerator.Models;

public class Feriado
{
    public int Id { get; set; }
    public string Pais { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EsNacional { get; set; }
}
