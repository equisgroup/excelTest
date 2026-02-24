using LiteDB;

namespace ExcelResourceManager.Core.Models;

public class Empleado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int UbicacionId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; } = true;
    
    [BsonIgnore]
    public string NombreCompleto => $"{Nombre} {Apellido}";
    
    [BsonIgnore]
    public Ubicacion? Ubicacion { get; set; }
}
