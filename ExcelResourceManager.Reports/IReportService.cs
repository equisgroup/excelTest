namespace ExcelResourceManager.Reports;

public interface IReportService
{
    Task<string> GenerarReporteConflictosAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
    Task<string> GenerarDashboardGerencialAsync();
}
