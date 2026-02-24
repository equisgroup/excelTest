namespace ExcelResourceManager.Core.Models
{
    /// <summary>
    /// Define los requerimientos de roles para un cliente en un período de tiempo específico
    /// </summary>
    public class RolCliente
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Rol { get; set; } = string.Empty;  // Ej: "Developer", "QA", "Architect"
        public int CantidadRequerida { get; set; }        // Cantidad de recursos necesarios en este rol
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; } = true;
    }
}
