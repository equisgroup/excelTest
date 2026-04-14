namespace ExcelResourceManager.Reports;

public class ImportResult
{
    public string Entidad { get; set; } = string.Empty;
    public int TotalFilas { get; set; }
    public int Importados { get; set; }
    public int Omitidos { get; set; }
    public List<ImportError> Errores { get; set; } = new();
    public string ReturnUrl { get; set; } = string.Empty;
}

public class ImportError
{
    public int Fila { get; set; }
    public string Datos { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}
