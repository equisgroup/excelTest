namespace ExcelResourceManager.Web.Models;

public class FeriadoViewModel
{
    public int Id { get; set; }
    public int UbicacionId { get; set; }
    public string UbicacionNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EsNacional { get; set; }
    public int Año { get; set; }
}
