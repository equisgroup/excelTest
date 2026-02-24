namespace ExcelResourceManager.Web.Models;

public class RolClienteViewModel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int CantidadRequerida { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
