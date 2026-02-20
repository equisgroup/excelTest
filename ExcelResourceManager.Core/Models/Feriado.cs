namespace ExcelResourceManager.Core.Models;

public class Feriado
{
    public int Id { get; set; }
    public int UbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EsNacional { get; set; }
    public int Año { get; set; }
}
