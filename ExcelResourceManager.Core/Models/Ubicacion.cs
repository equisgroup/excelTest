using LiteDB;

namespace ExcelResourceManager.Core.Models;

public class Ubicacion
{
    public int Id { get; set; }
    public string Pais { get; set; } = string.Empty;
    public string CodigoPais { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ZonaHoraria { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
