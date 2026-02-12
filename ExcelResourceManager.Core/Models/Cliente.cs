using LiteDB;

namespace ExcelResourceManager.Core.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int UbicacionId { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    
    [BsonIgnore]
    public Ubicacion? Ubicacion { get; set; }
}
