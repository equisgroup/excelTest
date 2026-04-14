using ClosedXML.Excel;
using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using Serilog;

namespace ExcelResourceManager.Reports;

public class ExcelImportExportService : IExcelImportExportService
{
    private readonly ILogger _logger = Log.ForContext<ExcelImportExportService>();

    // ────────────────────────────────────────────────────────────────
    // EXPORT
    // ────────────────────────────────────────────────────────────────

    public byte[] ExportarFeriados(IEnumerable<Feriado> feriados, IEnumerable<Ubicacion> ubicaciones)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Feriados");

        // Headers
        string[] headers = { "Id", "UbicacionId", "Ciudad (ref)", "Pais (ref)", "Fecha (dd/MM/yyyy)", "Nombre", "EsNacional (SI/NO)", "Año" };
        WriteHeaders(ws, headers);

        var ubDict = ubicaciones.ToDictionary(u => u.Id);
        int row = 2;
        foreach (var f in feriados)
        {
            ubDict.TryGetValue(f.UbicacionId, out var ub);
            ws.Cell(row, 1).Value = f.Id;
            ws.Cell(row, 2).Value = f.UbicacionId;
            ws.Cell(row, 3).Value = ub?.Ciudad ?? string.Empty;
            ws.Cell(row, 4).Value = ub?.Pais ?? string.Empty;
            ws.Cell(row, 5).Value = f.Fecha.ToString("dd/MM/yyyy");
            ws.Cell(row, 6).Value = f.Nombre;
            ws.Cell(row, 7).Value = f.EsNacional ? "SI" : "NO";
            ws.Cell(row, 8).Value = f.Año;
            row++;
        }

        AddNoteRow(ws, row + 1, "Para importar: complete columnas UbicacionId, Fecha, Nombre, EsNacional. La columna Id se ignora al importar.");
        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] ExportarUbicaciones(IEnumerable<Ubicacion> ubicaciones)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Ubicaciones");

        string[] headers = { "Id", "Pais", "CodigoPais", "Ciudad", "ZonaHoraria", "Activo (SI/NO)" };
        WriteHeaders(ws, headers);

        int row = 2;
        foreach (var u in ubicaciones)
        {
            ws.Cell(row, 1).Value = u.Id;
            ws.Cell(row, 2).Value = u.Pais;
            ws.Cell(row, 3).Value = u.CodigoPais;
            ws.Cell(row, 4).Value = u.Ciudad;
            ws.Cell(row, 5).Value = u.ZonaHoraria;
            ws.Cell(row, 6).Value = u.Activo ? "SI" : "NO";
            row++;
        }

        AddNoteRow(ws, row + 1, "Para importar: complete Pais, CodigoPais, Ciudad, ZonaHoraria, Activo. La columna Id se ignora al importar.");
        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] ExportarRoles(IEnumerable<Rol> roles)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Roles");

        string[] headers = { "Id", "Nombre", "Activo (SI/NO)" };
        WriteHeaders(ws, headers);

        int row = 2;
        foreach (var r in roles)
        {
            ws.Cell(row, 1).Value = r.Id;
            ws.Cell(row, 2).Value = r.Nombre;
            ws.Cell(row, 3).Value = r.Activo ? "SI" : "NO";
            row++;
        }

        AddNoteRow(ws, row + 1, "Para importar: complete Nombre, Activo. La columna Id se ignora al importar.");
        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] ExportarViajes(IEnumerable<Viaje> viajes, IEnumerable<Empleado> empleados, IEnumerable<Cliente> clientes, IEnumerable<Ubicacion> ubicaciones)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Viajes");

        string[] headers = { "Id", "EmpleadoId", "Empleado (ref)", "ClienteDestinoId", "Cliente (ref)", "UbicacionDestinoId", "Ubicacion (ref)", "FechaInicio (dd/MM/yyyy)", "FechaFin (dd/MM/yyyy)", "Estado", "TieneConflictos", "Observaciones" };
        WriteHeaders(ws, headers);

        var empDict = empleados.ToDictionary(e => e.Id);
        var cliDict = clientes.ToDictionary(c => c.Id);
        var ubDict = ubicaciones.ToDictionary(u => u.Id);

        int row = 2;
        foreach (var v in viajes)
        {
            empDict.TryGetValue(v.EmpleadoId, out var emp);
            cliDict.TryGetValue(v.ClienteDestinoId, out var cli);
            ubDict.TryGetValue(v.UbicacionDestinoId, out var ub);

            ws.Cell(row, 1).Value = v.Id;
            ws.Cell(row, 2).Value = v.EmpleadoId;
            ws.Cell(row, 3).Value = emp?.NombreCompleto ?? string.Empty;
            ws.Cell(row, 4).Value = v.ClienteDestinoId;
            ws.Cell(row, 5).Value = cli?.Nombre ?? string.Empty;
            ws.Cell(row, 6).Value = v.UbicacionDestinoId;
            ws.Cell(row, 7).Value = ub?.Ciudad ?? string.Empty;
            ws.Cell(row, 8).Value = v.FechaInicio.ToString("dd/MM/yyyy");
            ws.Cell(row, 9).Value = v.FechaFin.ToString("dd/MM/yyyy");
            ws.Cell(row, 10).Value = v.Estado.ToString();
            ws.Cell(row, 11).Value = v.TieneConflictos ? "SI" : "NO";
            ws.Cell(row, 12).Value = v.Observaciones;
            row++;
        }

        AddNoteRow(ws, row + 1, "Para importar: complete EmpleadoId, ClienteDestinoId, UbicacionDestinoId, FechaInicio, FechaFin, Estado (Planificado/Confirmado/EnCurso/Completado/Cancelado), Observaciones. Id y columnas (ref) se ignoran.");
        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] ExportarVacaciones(IEnumerable<Vacacion> vacaciones, IEnumerable<Empleado> empleados)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Vacaciones");

        string[] headers = { "Id", "EmpleadoId", "Empleado (ref)", "FechaInicio (dd/MM/yyyy)", "FechaFin (dd/MM/yyyy)", "Estado", "DiasHabiles", "TieneConflictos", "Observaciones" };
        WriteHeaders(ws, headers);

        var empDict = empleados.ToDictionary(e => e.Id);
        int row = 2;
        foreach (var v in vacaciones)
        {
            empDict.TryGetValue(v.EmpleadoId, out var emp);
            ws.Cell(row, 1).Value = v.Id;
            ws.Cell(row, 2).Value = v.EmpleadoId;
            ws.Cell(row, 3).Value = emp?.NombreCompleto ?? string.Empty;
            ws.Cell(row, 4).Value = v.FechaInicio.ToString("dd/MM/yyyy");
            ws.Cell(row, 5).Value = v.FechaFin.ToString("dd/MM/yyyy");
            ws.Cell(row, 6).Value = v.Estado.ToString();
            ws.Cell(row, 7).Value = v.DiasHabiles;
            ws.Cell(row, 8).Value = v.TieneConflictos ? "SI" : "NO";
            ws.Cell(row, 9).Value = v.Observaciones;
            row++;
        }

        AddNoteRow(ws, row + 1, "Para importar: complete EmpleadoId, FechaInicio, FechaFin, Estado (Solicitada/Aprobada/Rechazada/Cancelada), Observaciones. Id y columnas (ref) se ignoran.");
        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    // ────────────────────────────────────────────────────────────────
    // IMPORT
    // ────────────────────────────────────────────────────────────────

    public async Task<ImportResult> ImportarFeriadosAsync(Stream stream, IEnumerable<Ubicacion> ubicaciones, IEnumerable<Feriado> existentes, Func<Feriado, Task> insertar)
    {
        var result = new ImportResult { Entidad = "Feriados" };
        var ubList = ubicaciones.ToList();
        var existentesList = existentes.ToList();

        try
        {
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var cellUbId = ws.Cell(row, 2).GetString().Trim();
                var cellFecha = ws.Cell(row, 5).GetString().Trim();
                var cellNombre = ws.Cell(row, 6).GetString().Trim();
                var cellNacional = ws.Cell(row, 7).GetString().Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(cellNombre) && string.IsNullOrWhiteSpace(cellFecha))
                    continue; // skip empty rows (e.g. note row)

                result.TotalFilas++;
                string datos = $"Fila {row}: UbicacionId={cellUbId}, Fecha={cellFecha}, Nombre={cellNombre}";

                // Parse UbicacionId
                if (!int.TryParse(cellUbId, out int ubicacionId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "UbicacionId inválido o vacío" });
                    result.Omitidos++;
                    continue;
                }

                // Validate ubicacion exists
                if (!ubList.Any(u => u.Id == ubicacionId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"No existe la ubicación con Id {ubicacionId}" });
                    result.Omitidos++;
                    continue;
                }

                // Parse fecha
                if (!DateTime.TryParseExact(cellFecha, new[] { "dd/MM/yyyy", "M/d/yyyy", "yyyy-MM-dd" }, null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "Fecha inválida (use formato dd/MM/yyyy)" });
                    result.Omitidos++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(cellNombre))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "El nombre es obligatorio" });
                    result.Omitidos++;
                    continue;
                }

                // Check for duplicate
                if (existentesList.Any(f => f.UbicacionId == ubicacionId && f.Fecha.Date == fecha.Date && f.Nombre.Equals(cellNombre, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "Ya existe un feriado con el mismo nombre, fecha y ubicación" });
                    result.Omitidos++;
                    continue;
                }

                bool esNacional = cellNacional == "SI" || cellNacional == "SÍ" || cellNacional == "1" || cellNacional == "TRUE";

                var feriado = new Feriado
                {
                    UbicacionId = ubicacionId,
                    Fecha = fecha,
                    Nombre = cellNombre,
                    EsNacional = esNacional,
                    Año = fecha.Year
                };

                try
                {
                    await insertar(feriado);
                    existentesList.Add(feriado); // prevent duplicates within same import
                    result.Importados++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al insertar feriado fila {Row}", row);
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"Error al guardar: {ex.Message}" });
                    result.Omitidos++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al procesar archivo de feriados");
            result.Errores.Add(new ImportError { Fila = 0, Datos = string.Empty, Motivo = $"Error al leer el archivo: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResult> ImportarUbicacionesAsync(Stream stream, IEnumerable<Ubicacion> existentes, Func<Ubicacion, Task> insertar)
    {
        var result = new ImportResult { Entidad = "Ubicaciones" };
        var existentesList = existentes.ToList();

        try
        {
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var pais = ws.Cell(row, 2).GetString().Trim();
                var codigoPais = ws.Cell(row, 3).GetString().Trim();
                var ciudad = ws.Cell(row, 4).GetString().Trim();
                var zona = ws.Cell(row, 5).GetString().Trim();
                var activoStr = ws.Cell(row, 6).GetString().Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(pais) && string.IsNullOrWhiteSpace(ciudad))
                    continue;

                result.TotalFilas++;
                string datos = $"Fila {row}: Pais={pais}, Ciudad={ciudad}";

                if (string.IsNullOrWhiteSpace(pais))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "El campo Pais es obligatorio" });
                    result.Omitidos++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "El campo Ciudad es obligatorio" });
                    result.Omitidos++;
                    continue;
                }

                // Duplicate check
                if (existentesList.Any(u => u.Pais.Equals(pais, StringComparison.OrdinalIgnoreCase) && u.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "Ya existe una ubicación con el mismo País y Ciudad" });
                    result.Omitidos++;
                    continue;
                }

                bool activo = string.IsNullOrWhiteSpace(activoStr) || activoStr == "SI" || activoStr == "SÍ" || activoStr == "1" || activoStr == "TRUE";

                var ubicacion = new Ubicacion
                {
                    Pais = pais,
                    CodigoPais = codigoPais,
                    Ciudad = ciudad,
                    ZonaHoraria = zona,
                    Activo = activo
                };

                try
                {
                    await insertar(ubicacion);
                    existentesList.Add(ubicacion);
                    result.Importados++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al insertar ubicación fila {Row}", row);
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"Error al guardar: {ex.Message}" });
                    result.Omitidos++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al procesar archivo de ubicaciones");
            result.Errores.Add(new ImportError { Fila = 0, Datos = string.Empty, Motivo = $"Error al leer el archivo: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResult> ImportarRolesAsync(Stream stream, IEnumerable<Rol> existentes, Func<Rol, Task> insertar)
    {
        var result = new ImportResult { Entidad = "Roles" };
        var existentesList = existentes.ToList();

        try
        {
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var nombre = ws.Cell(row, 2).GetString().Trim();
                var activoStr = ws.Cell(row, 3).GetString().Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(nombre))
                    continue;

                result.TotalFilas++;
                string datos = $"Fila {row}: Nombre={nombre}";

                // Duplicate check
                if (existentesList.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "Ya existe un rol con ese nombre" });
                    result.Omitidos++;
                    continue;
                }

                bool activo = string.IsNullOrWhiteSpace(activoStr) || activoStr == "SI" || activoStr == "SÍ" || activoStr == "1" || activoStr == "TRUE";

                var rol = new Rol { Nombre = nombre, Activo = activo };

                try
                {
                    await insertar(rol);
                    existentesList.Add(rol);
                    result.Importados++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al insertar rol fila {Row}", row);
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"Error al guardar: {ex.Message}" });
                    result.Omitidos++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al procesar archivo de roles");
            result.Errores.Add(new ImportError { Fila = 0, Datos = string.Empty, Motivo = $"Error al leer el archivo: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResult> ImportarViajesAsync(Stream stream, IEnumerable<Empleado> empleados, IEnumerable<Cliente> clientes, IEnumerable<Ubicacion> ubicaciones, Func<Viaje, Task> insertar)
    {
        var result = new ImportResult { Entidad = "Viajes" };
        var empList = empleados.ToList();
        var cliList = clientes.ToList();
        var ubList = ubicaciones.ToList();

        try
        {
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var cellEmpId = ws.Cell(row, 2).GetString().Trim();
                var cellCliId = ws.Cell(row, 4).GetString().Trim();
                var cellUbId = ws.Cell(row, 6).GetString().Trim();
                var cellFechaInicio = ws.Cell(row, 8).GetString().Trim();
                var cellFechaFin = ws.Cell(row, 9).GetString().Trim();
                var cellEstado = ws.Cell(row, 10).GetString().Trim();
                var cellObs = ws.Cell(row, 12).GetString().Trim();

                if (string.IsNullOrWhiteSpace(cellEmpId) && string.IsNullOrWhiteSpace(cellFechaInicio))
                    continue;

                result.TotalFilas++;
                string datos = $"Fila {row}: EmpleadoId={cellEmpId}, FechaInicio={cellFechaInicio}, FechaFin={cellFechaFin}";

                if (!int.TryParse(cellEmpId, out int empId) || !empList.Any(e => e.Id == empId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"EmpleadoId '{cellEmpId}' no existe o es inválido" });
                    result.Omitidos++;
                    continue;
                }

                if (!int.TryParse(cellCliId, out int cliId) || !cliList.Any(c => c.Id == cliId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"ClienteDestinoId '{cellCliId}' no existe o es inválido" });
                    result.Omitidos++;
                    continue;
                }

                if (!int.TryParse(cellUbId, out int ubId) || !ubList.Any(u => u.Id == ubId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"UbicacionDestinoId '{cellUbId}' no existe o es inválido" });
                    result.Omitidos++;
                    continue;
                }

                if (!DateTime.TryParseExact(cellFechaInicio, new[] { "dd/MM/yyyy", "M/d/yyyy", "yyyy-MM-dd" }, null, System.Globalization.DateTimeStyles.None, out DateTime fechaInicio))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaInicio inválida (use dd/MM/yyyy)" });
                    result.Omitidos++;
                    continue;
                }

                if (!DateTime.TryParseExact(cellFechaFin, new[] { "dd/MM/yyyy", "M/d/yyyy", "yyyy-MM-dd" }, null, System.Globalization.DateTimeStyles.None, out DateTime fechaFin))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaFin inválida (use dd/MM/yyyy)" });
                    result.Omitidos++;
                    continue;
                }

                if (fechaFin < fechaInicio)
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaFin debe ser mayor o igual a FechaInicio" });
                    result.Omitidos++;
                    continue;
                }

                if (!Enum.TryParse<EstadoViaje>(cellEstado, true, out var estado))
                    estado = EstadoViaje.Planificado;

                var viaje = new Viaje
                {
                    EmpleadoId = empId,
                    ClienteDestinoId = cliId,
                    UbicacionDestinoId = ubId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Estado = estado,
                    Observaciones = cellObs
                };

                try
                {
                    await insertar(viaje);
                    result.Importados++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al insertar viaje fila {Row}", row);
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"Error al guardar: {ex.Message}" });
                    result.Omitidos++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al procesar archivo de viajes");
            result.Errores.Add(new ImportError { Fila = 0, Datos = string.Empty, Motivo = $"Error al leer el archivo: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResult> ImportarVacacionesAsync(Stream stream, IEnumerable<Empleado> empleados, IEnumerable<Vacacion> existentes, Func<Vacacion, Task> insertar)
    {
        var result = new ImportResult { Entidad = "Vacaciones" };
        var empList = empleados.ToList();
        var existentesList = existentes.ToList();

        try
        {
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheets.First();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var cellEmpId = ws.Cell(row, 2).GetString().Trim();
                var cellFechaInicio = ws.Cell(row, 4).GetString().Trim();
                var cellFechaFin = ws.Cell(row, 5).GetString().Trim();
                var cellEstado = ws.Cell(row, 6).GetString().Trim();
                var cellObs = ws.Cell(row, 9).GetString().Trim();

                if (string.IsNullOrWhiteSpace(cellEmpId) && string.IsNullOrWhiteSpace(cellFechaInicio))
                    continue;

                result.TotalFilas++;
                string datos = $"Fila {row}: EmpleadoId={cellEmpId}, FechaInicio={cellFechaInicio}, FechaFin={cellFechaFin}";

                if (!int.TryParse(cellEmpId, out int empId) || !empList.Any(e => e.Id == empId))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"EmpleadoId '{cellEmpId}' no existe o es inválido" });
                    result.Omitidos++;
                    continue;
                }

                if (!DateTime.TryParseExact(cellFechaInicio, new[] { "dd/MM/yyyy", "M/d/yyyy", "yyyy-MM-dd" }, null, System.Globalization.DateTimeStyles.None, out DateTime fechaInicio))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaInicio inválida (use dd/MM/yyyy)" });
                    result.Omitidos++;
                    continue;
                }

                if (!DateTime.TryParseExact(cellFechaFin, new[] { "dd/MM/yyyy", "M/d/yyyy", "yyyy-MM-dd" }, null, System.Globalization.DateTimeStyles.None, out DateTime fechaFin))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaFin inválida (use dd/MM/yyyy)" });
                    result.Omitidos++;
                    continue;
                }

                if (fechaFin < fechaInicio)
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "FechaFin debe ser mayor o igual a FechaInicio" });
                    result.Omitidos++;
                    continue;
                }

                // Overlap check: same employee with overlapping dates
                if (existentesList.Any(v => v.EmpleadoId == empId && v.FechaInicio.Date <= fechaFin.Date && v.FechaFin.Date >= fechaInicio.Date))
                {
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = "El empleado ya tiene vacaciones en ese período" });
                    result.Omitidos++;
                    continue;
                }

                if (!Enum.TryParse<EstadoVacacion>(cellEstado, true, out var estado))
                    estado = EstadoVacacion.Solicitada;

                var vacacion = new Vacacion
                {
                    EmpleadoId = empId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Estado = estado,
                    Observaciones = cellObs
                };

                try
                {
                    await insertar(vacacion);
                    existentesList.Add(vacacion);
                    result.Importados++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al insertar vacación fila {Row}", row);
                    result.Errores.Add(new ImportError { Fila = row, Datos = datos, Motivo = $"Error al guardar: {ex.Message}" });
                    result.Omitidos++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al procesar archivo de vacaciones");
            result.Errores.Add(new ImportError { Fila = 0, Datos = string.Empty, Motivo = $"Error al leer el archivo: {ex.Message}" });
        }

        return result;
    }

    // ────────────────────────────────────────────────────────────────
    // EXPORT ERRORS
    // ────────────────────────────────────────────────────────────────

    public byte[] ExportarErroresImportacion(IEnumerable<ImportError> errores, string entidad)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Errores");

        string[] headers = { "Fila", "Datos", "Motivo" };
        WriteHeaders(ws, headers);

        ws.Cell(1, 1).WorksheetRow().Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE6E6");

        int row = 2;
        foreach (var e in errores)
        {
            ws.Cell(row, 1).Value = e.Fila;
            ws.Cell(row, 2).Value = e.Datos;
            ws.Cell(row, 3).Value = e.Motivo;
            row++;
        }

        if (errores.Any())
            ws.RangeUsed()?.SetAutoFilter();

        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    // ────────────────────────────────────────────────────────────────
    // HELPERS
    // ────────────────────────────────────────────────────────────────

    private static void WriteHeaders(IXLWorksheet ws, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        var headerRange = ws.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
    }

    private static void AddNoteRow(IXLWorksheet ws, int row, string note)
    {
        ws.Cell(row, 1).Value = note;
        ws.Cell(row, 1).Style.Font.Italic = true;
        ws.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
    }

    private static byte[] ToBytes(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
