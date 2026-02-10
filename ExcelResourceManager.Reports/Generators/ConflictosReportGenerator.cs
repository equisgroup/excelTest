using ClosedXML.Excel;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using Serilog;

namespace ExcelResourceManager.Reports.Generators;

public class ConflictosReportGenerator : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;
    private readonly ILogger _logger;

    public ConflictosReportGenerator(IUnitOfWork unitOfWork, IValidationService validationService)
    {
        _unitOfWork = unitOfWork;
        _validationService = validationService;
        _logger = Log.ForContext<ConflictosReportGenerator>();
    }

    public async Task<string> GenerarReporteConflictosAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        try
        {
            _logger.Information("Generando reporte de conflictos desde {FechaInicio} hasta {FechaFin}", 
                fechaInicio?.ToString("dd/MM/yyyy") ?? "inicio", 
                fechaFin?.ToString("dd/MM/yyyy") ?? "fin");

            var conflictos = (await _unitOfWork.Conflictos.GetAllAsync()).ToList();
            
            // Filtrar por fecha si se proporciona
            if (fechaInicio.HasValue)
            {
                conflictos = conflictos.Where(c => c.FechaConflicto >= fechaInicio.Value).ToList();
            }
            if (fechaFin.HasValue)
            {
                conflictos = conflictos.Where(c => c.FechaConflicto <= fechaFin.Value).ToList();
            }

            var empleados = (await _unitOfWork.Empleados.GetAllAsync()).ToList();

            using var workbook = new XLWorkbook();

            // Sheet 1: Resumen
            GenerarHojaResumen(workbook, conflictos);

            // Sheet 2: Lista Detallada
            await GenerarHojaDetalladaAsync(workbook, conflictos, empleados);

            // Sheet 3: Por Empleado
            await GenerarHojaPorEmpleadoAsync(workbook, conflictos, empleados);

            // Guardar archivo
            var reportesDir = Path.Combine(Directory.GetCurrentDirectory(), "Reportes");
            Directory.CreateDirectory(reportesDir);

            var fileName = $"Conflictos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(reportesDir, fileName);

            workbook.SaveAs(filePath);

            _logger.Information("Reporte de conflictos generado exitosamente en {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar reporte de conflictos");
            throw;
        }
    }

    private void GenerarHojaResumen(XLWorkbook workbook, List<Conflicto> conflictos)
    {
        var worksheet = workbook.Worksheets.Add("Resumen");

        // KPIs
        worksheet.Cell(1, 1).Value = "Total Conflictos";
        worksheet.Cell(1, 2).Value = conflictos.Count;
        worksheet.Cell(1, 2).Style.Font.Bold = true;

        worksheet.Cell(2, 1).Value = "Críticos";
        worksheet.Cell(2, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Critico);
        worksheet.Cell(2, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6");
        worksheet.Cell(2, 2).Style.Font.Bold = true;

        worksheet.Cell(3, 1).Value = "Altos";
        worksheet.Cell(3, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Alto);
        worksheet.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE5CC");
        worksheet.Cell(3, 2).Style.Font.Bold = true;

        worksheet.Cell(4, 1).Value = "Medios";
        worksheet.Cell(4, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Medio);
        worksheet.Cell(4, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFCC");
        worksheet.Cell(4, 2).Style.Font.Bold = true;

        worksheet.Cell(5, 1).Value = "Bajos";
        worksheet.Cell(5, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Bajo);
        worksheet.Cell(5, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#CCE5FF");
        worksheet.Cell(5, 2).Style.Font.Bold = true;

        // Tabla resumen por tipo
        int currentRow = 7;
        worksheet.Cell(currentRow, 1).Value = "Tipo de Conflicto";
        worksheet.Cell(currentRow, 2).Value = "Cantidad";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;

        var agrupadoPorTipo = conflictos
            .GroupBy(c => c.Tipo)
            .OrderByDescending(g => g.Count())
            .ToList();

        currentRow++;
        foreach (var grupo in agrupadoPorTipo)
        {
            worksheet.Cell(currentRow, 1).Value = ObtenerNombreTipoConflicto(grupo.Key);
            worksheet.Cell(currentRow, 2).Value = grupo.Count();
            currentRow++;
        }

        // Aplicar formato de tabla
        if (agrupadoPorTipo.Any())
        {
            var tableRange = worksheet.Range(7, 1, currentRow - 1, 2);
            var table = tableRange.CreateTable();
            table.Theme = XLTableTheme.TableStyleMedium2;
        }

        worksheet.Columns().AdjustToContents();
    }

    private async Task GenerarHojaDetalladaAsync(XLWorkbook workbook, List<Conflicto> conflictos, List<Empleado> empleados)
    {
        var worksheet = workbook.Worksheets.Add("Lista Detallada");

        // Encabezados
        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Tipo";
        worksheet.Cell(1, 3).Value = "Nivel";
        worksheet.Cell(1, 4).Value = "EmpleadoId";
        worksheet.Cell(1, 5).Value = "Empleado";
        worksheet.Cell(1, 6).Value = "Fecha Conflicto";
        worksheet.Cell(1, 7).Value = "Descripción";
        worksheet.Cell(1, 8).Value = "Recomendación";
        worksheet.Cell(1, 9).Value = "Resuelto";

        var headerRange = worksheet.Range(1, 1, 1, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Datos
        int currentRow = 2;
        foreach (var conflicto in conflictos.OrderByDescending(c => c.Nivel).ThenByDescending(c => c.FechaConflicto))
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == conflicto.EmpleadoId);

            worksheet.Cell(currentRow, 1).Value = conflicto.Id;
            worksheet.Cell(currentRow, 2).Value = ObtenerNombreTipoConflicto(conflicto.Tipo);
            worksheet.Cell(currentRow, 3).Value = ObtenerNombreNivelConflicto(conflicto.Nivel);
            worksheet.Cell(currentRow, 4).Value = conflicto.EmpleadoId;
            worksheet.Cell(currentRow, 5).Value = empleado?.NombreCompleto ?? "Desconocido";
            worksheet.Cell(currentRow, 6).Value = conflicto.FechaConflicto.ToString("dd/MM/yyyy");
            worksheet.Cell(currentRow, 7).Value = conflicto.Descripcion;
            worksheet.Cell(currentRow, 8).Value = conflicto.Recomendacion;
            worksheet.Cell(currentRow, 9).Value = conflicto.Resuelto ? "Sí" : "No";

            // Formato condicional según nivel
            var rowRange = worksheet.Range(currentRow, 1, currentRow, 9);
            switch (conflicto.Nivel)
            {
                case NivelConflicto.Critico:
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6");
                    break;
                case NivelConflicto.Alto:
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE5CC");
                    break;
                case NivelConflicto.Medio:
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFCC");
                    break;
                case NivelConflicto.Bajo:
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#CCE5FF");
                    break;
                case NivelConflicto.Informativo:
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F0F0");
                    break;
            }

            currentRow++;
        }

        // Auto-filtros
        if (conflictos.Any())
        {
            worksheet.RangeUsed().SetAutoFilter();
        }

        worksheet.Columns().AdjustToContents();
    }

    private async Task GenerarHojaPorEmpleadoAsync(XLWorkbook workbook, List<Conflicto> conflictos, List<Empleado> empleados)
    {
        var worksheet = workbook.Worksheets.Add("Por Empleado");

        int currentRow = 1;

        var conflictosPorEmpleado = conflictos
            .GroupBy(c => c.EmpleadoId)
            .OrderByDescending(g => g.Count())
            .ToList();

        foreach (var grupo in conflictosPorEmpleado)
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == grupo.Key);
            var nombreEmpleado = empleado?.NombreCompleto ?? $"Empleado ID: {grupo.Key}";

            // Encabezado del empleado
            worksheet.Cell(currentRow, 1).Value = nombreEmpleado;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Range(currentRow, 1, currentRow, 7).Merge();
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            currentRow++;

            // Encabezados de columnas
            worksheet.Cell(currentRow, 1).Value = "Id";
            worksheet.Cell(currentRow, 2).Value = "Tipo";
            worksheet.Cell(currentRow, 3).Value = "Nivel";
            worksheet.Cell(currentRow, 4).Value = "Fecha";
            worksheet.Cell(currentRow, 5).Value = "Descripción";
            worksheet.Cell(currentRow, 6).Value = "Recomendación";
            worksheet.Cell(currentRow, 7).Value = "Resuelto";

            var headerRange = worksheet.Range(currentRow, 1, currentRow, 7);
            headerRange.Style.Font.Bold = true;
            currentRow++;

            // Conflictos del empleado
            foreach (var conflicto in grupo.OrderByDescending(c => c.Nivel).ThenByDescending(c => c.FechaConflicto))
            {
                worksheet.Cell(currentRow, 1).Value = conflicto.Id;
                worksheet.Cell(currentRow, 2).Value = ObtenerNombreTipoConflicto(conflicto.Tipo);
                worksheet.Cell(currentRow, 3).Value = ObtenerNombreNivelConflicto(conflicto.Nivel);
                worksheet.Cell(currentRow, 4).Value = conflicto.FechaConflicto.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 5).Value = conflicto.Descripcion;
                worksheet.Cell(currentRow, 6).Value = conflicto.Recomendacion;
                worksheet.Cell(currentRow, 7).Value = conflicto.Resuelto ? "Sí" : "No";
                currentRow++;
            }

            // Subtotal
            worksheet.Cell(currentRow, 1).Value = "Total conflictos:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = grupo.Count();
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            currentRow += 2; // Espacio entre empleados
        }

        worksheet.Columns().AdjustToContents();
    }

    public async Task<string> GenerarDashboardGerencialAsync()
    {
        try
        {
            _logger.Information("Generando dashboard gerencial");

            var empleados = (await _unitOfWork.Empleados.GetAllAsync()).ToList();
            var clientes = (await _unitOfWork.Clientes.GetAllAsync()).ToList();
            var asignaciones = (await _unitOfWork.AsignacionesCliente.GetAllAsync()).ToList();
            var vacaciones = (await _unitOfWork.Vacaciones.GetAllAsync()).ToList();
            var viajes = (await _unitOfWork.Viajes.GetAllAsync()).ToList();
            var conflictos = (await _unitOfWork.Conflictos.GetAllAsync()).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dashboard Gerencial");

            int currentRow = 1;

            // Resumen general
            worksheet.Cell(currentRow, 1).Value = "DASHBOARD GERENCIAL";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
            worksheet.Range(currentRow, 1, currentRow, 4).Merge();
            currentRow += 2;

            // KPIs principales
            worksheet.Cell(currentRow, 1).Value = "Total Empleados";
            worksheet.Cell(currentRow, 2).Value = empleados.Count;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Empleados Activos";
            worksheet.Cell(currentRow, 2).Value = empleados.Count(e => e.Activo);
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Clientes";
            worksheet.Cell(currentRow, 2).Value = clientes.Count;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Asignaciones Activas";
            worksheet.Cell(currentRow, 2).Value = asignaciones.Count(a => a.Activa);
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow += 2;

            // Próximas vacaciones (próximos 30 días)
            var fechaLimite = DateTime.Now.AddDays(30);
            var proximasVacaciones = vacaciones
                .Where(v => v.FechaInicio >= DateTime.Now && v.FechaInicio <= fechaLimite)
                .OrderBy(v => v.FechaInicio)
                .Take(10)
                .ToList();

            worksheet.Cell(currentRow, 1).Value = "PRÓXIMAS VACACIONES (30 días)";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Range(currentRow, 1, currentRow, 5).Merge();
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Empleado";
            worksheet.Cell(currentRow, 2).Value = "Fecha Inicio";
            worksheet.Cell(currentRow, 3).Value = "Fecha Fin";
            worksheet.Cell(currentRow, 4).Value = "Días";
            worksheet.Cell(currentRow, 5).Value = "Estado";
            worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
            currentRow++;

            foreach (var vacacion in proximasVacaciones)
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
                worksheet.Cell(currentRow, 1).Value = empleado?.NombreCompleto ?? "Desconocido";
                worksheet.Cell(currentRow, 2).Value = vacacion.FechaInicio.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 3).Value = vacacion.FechaFin.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 4).Value = vacacion.DiasHabiles;
                worksheet.Cell(currentRow, 5).Value = vacacion.Estado.ToString();
                currentRow++;
            }
            currentRow += 2;

            // Viajes recientes (últimos 30 días)
            var fechaInicio = DateTime.Now.AddDays(-30);
            var viajesRecientes = viajes
                .Where(v => v.FechaInicio >= fechaInicio)
                .OrderByDescending(v => v.FechaInicio)
                .Take(10)
                .ToList();

            worksheet.Cell(currentRow, 1).Value = "VIAJES RECIENTES (últimos 30 días)";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Range(currentRow, 1, currentRow, 4).Merge();
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Empleado";
            worksheet.Cell(currentRow, 2).Value = "Fecha Inicio";
            worksheet.Cell(currentRow, 3).Value = "Fecha Fin";
            worksheet.Cell(currentRow, 4).Value = "Estado";
            worksheet.Range(currentRow, 1, currentRow, 4).Style.Font.Bold = true;
            currentRow++;

            foreach (var viaje in viajesRecientes)
            {
                var empleado = empleados.FirstOrDefault(e => e.Id == viaje.EmpleadoId);
                worksheet.Cell(currentRow, 1).Value = empleado?.NombreCompleto ?? "Desconocido";
                worksheet.Cell(currentRow, 2).Value = viaje.FechaInicio.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 3).Value = viaje.FechaFin.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 4).Value = viaje.Estado.ToString();
                currentRow++;
            }
            currentRow += 2;

            // Resumen de conflictos
            worksheet.Cell(currentRow, 1).Value = "RESUMEN DE CONFLICTOS";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Range(currentRow, 1, currentRow, 3).Merge();
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Conflictos";
            worksheet.Cell(currentRow, 2).Value = conflictos.Count;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Conflictos No Resueltos";
            worksheet.Cell(currentRow, 2).Value = conflictos.Count(c => !c.Resuelto);
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6");
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Críticos";
            worksheet.Cell(currentRow, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Critico);
            worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6");
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Altos";
            worksheet.Cell(currentRow, 2).Value = conflictos.Count(c => c.Nivel == NivelConflicto.Alto);
            worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE5CC");
            currentRow++;

            worksheet.Columns().AdjustToContents();

            // Guardar archivo
            var reportesDir = Path.Combine(Directory.GetCurrentDirectory(), "Reportes");
            Directory.CreateDirectory(reportesDir);

            var fileName = $"DashboardGerencial_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(reportesDir, fileName);

            workbook.SaveAs(filePath);

            _logger.Information("Dashboard gerencial generado exitosamente en {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar dashboard gerencial");
            throw;
        }
    }

    private string ObtenerNombreTipoConflicto(TipoConflicto tipo)
    {
        return tipo switch
        {
            TipoConflicto.VacacionVsViaje => "Vacación vs Viaje",
            TipoConflicto.VacacionVsSoporte => "Vacación vs Soporte",
            TipoConflicto.ViajeVsSoporte => "Viaje vs Soporte",
            TipoConflicto.ViajeEnFeriado => "Viaje en Feriado",
            TipoConflicto.VacacionEnFeriado => "Vacación en Feriado",
            TipoConflicto.Sobreasignacion => "Sobreasignación",
            _ => tipo.ToString()
        };
    }

    private string ObtenerNombreNivelConflicto(NivelConflicto nivel)
    {
        return nivel switch
        {
            NivelConflicto.Critico => "Crítico",
            NivelConflicto.Alto => "Alto",
            NivelConflicto.Medio => "Medio",
            NivelConflicto.Bajo => "Bajo",
            NivelConflicto.Informativo => "Informativo",
            _ => nivel.ToString()
        };
    }
}
