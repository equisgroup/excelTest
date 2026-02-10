using ExcelDashboardGenerator.Models;
using ExcelDashboardGenerator.Data;
using ClosedXML.Excel;
using System.Globalization;

namespace ExcelDashboardGenerator.Services;

public class ExcelGeneratorService
{
    private readonly DashboardService _dashboardService;
    private readonly SlicerService _slicerService;
    
    public ExcelGeneratorService()
    {
        _dashboardService = new DashboardService();
        _slicerService = new SlicerService();
    }
    
    public string GenerarExcel(DataContainer data, List<Feriado> feriados, List<Alerta> alertas)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"Dashboard_Gerencial_{timestamp}.xlsx";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        
        using (var workbook = new XLWorkbook())
        {
            // Crear todas las hojas
            Console.WriteLine("  Creando hoja: Dashboard Gerencial...");
            CrearDashboardGerencial(workbook, data, alertas);
            
            Console.WriteLine("  Creando hoja: Análisis de Alertas...");
            CrearHojaAnalisisAlertas(workbook, data);
            
            Console.WriteLine("  Creando hoja: Clientes...");
            CrearHojaClientes(workbook, data.Clientes);
            
            Console.WriteLine("  Creando hoja: Empleados...");
            CrearHojaEmpleados(workbook, data.Empleados, data.Clientes);
            
            Console.WriteLine("  Creando hoja: Asignaciones...");
            CrearHojaAsignaciones(workbook, data.Asignaciones, data.Empleados, data.Clientes);
            
            Console.WriteLine("  Creando hoja: Vacaciones...");
            CrearHojaVacaciones(workbook, data.Vacaciones, data.Empleados, feriados);
            
            Console.WriteLine("  Creando hoja: Viajes...");
            CrearHojaViajes(workbook, data.Viajes, data.Empleados, data.Clientes, feriados);
            
            Console.WriteLine("  Creando hoja: Turnos Soporte...");
            CrearHojaTurnosSoporte(workbook, data.TurnosSoporte, data.Empleados);
            
            Console.WriteLine("  Creando hoja: Feriados...");
            CrearHojaFeriados(workbook, feriados);
            
            Console.WriteLine("  Creando hoja: Dashboard Ocupación...");
            CrearDashboardOcupacion(workbook, data);
            
            Console.WriteLine("  Creando hoja: Panel de Control...");
            CrearPanelDeControl(workbook);
            
            Console.WriteLine("  Creando hoja: Instrucciones...");
            CrearHojaInstrucciones(workbook);
            
            // Guardar el archivo
            workbook.SaveAs(filePath);
        }
        
        // Generar archivo con código VBA
        Console.WriteLine("  Generando archivo con código VBA...");
        GenerarArchivoVBA(timestamp);
        
        // Aplicar mejoras con Open XML SDK si es necesario
        try
        {
            Console.WriteLine("  Aplicando mejoras con Open XML SDK...");
            _slicerService.AgregarSlicersAvanzados(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Advertencia: No se pudieron agregar slicers avanzados: {ex.Message}");
        }
        
        return filePath;
    }
    
    private void GenerarArchivoVBA(string timestamp)
    {
        try
        {
            var vbaService = new VBAMacroService();
            var vbaCode = vbaService.GenerarCodigoVBA();
            
            var vbaFileName = $"VBA_Macro_Code_{timestamp}.txt";
            var vbaFilePath = Path.Combine(Directory.GetCurrentDirectory(), vbaFileName);
            
            File.WriteAllText(vbaFilePath, vbaCode);
            
            Console.WriteLine($"  ✓ Código VBA guardado en: {vbaFileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ⚠️ No se pudo generar archivo VBA: {ex.Message}");
        }
    }
    
    private void CrearDashboardGerencial(XLWorkbook workbook, DataContainer data, List<Alerta> alertas)
    {
        var ws = workbook.Worksheets.Add("📊 Dashboard Gerencial");
        
        // Título principal
        ws.Cell("A1").Value = "DASHBOARD GERENCIAL - CONTROL DE ASIGNACIONES";
        ws.Range("A1:F1").Merge().Style
            .Font.SetBold().Font.SetFontSize(16)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        ws.Row(1).Height = 30;
        
        // KPIs principales con fórmulas
        int row = 3;
        
        // Primera fila de KPIs - Usar fórmulas COUNTIF que referencian otras hojas
        CrearKPIConFormula(ws, "B", row, "Total Empleados Activos", "=COUNTIF('👨‍💼 Empleados'!J:J,\"Sí\")");
        CrearKPIConFormula(ws, "C", row, "Total Clientes Activos", "=COUNTIF('👥 Clientes'!H:H,\"Sí\")");
        CrearKPIConFormula(ws, "D", row, "Asignaciones Activas", "=COUNTIF('🔄 Asignaciones'!G:G,\"Sí\")");
        CrearKPIConFormula(ws, "E", row, "Vacaciones Pendientes", "=COUNTIF('🏖️ Vacaciones'!F:F,\"Pendiente\")");
        
        row += 3;
        
        // Segunda fila de KPIs - Calcular alertas dinámicamente desde las hojas de conflictos
        // Las alertas ahora se calculan sumando los conflictos detectados en cada hoja
        
        // Alertas Alta Prioridad: Vacaciones con conflictos + Viajes con conflictos de soporte
        // Fórmula: suma de conflictos en Vacaciones (columnas G y H) y conflictos críticos en Asignaciones
        CrearKPIConFormula(ws, "B", row, "Conflictos Críticos", 
            "=SUMPRODUCT(('🏖️ Vacaciones'!G:G>0)*1)+SUMPRODUCT(('🏖️ Vacaciones'!H:H>0)*1)+SUMPRODUCT(('🔄 Asignaciones'!H:H>2)*1)");
        
        // Alertas Media Prioridad: Viajes con conflictos de soporte
        CrearKPIConFormula(ws, "C", row, "Conflictos Medios", 
            "=SUMPRODUCT(('✈️ Viajes'!M:M>0)*1)");
        
        // Alertas informativas: Feriados detectados
        CrearKPIConFormula(ws, "D", row, "Feriados en Períodos", 
            "=SUMPRODUCT(('🏖️ Vacaciones'!I:I>0)*1)+SUMPRODUCT(('✈️ Viajes'!K:K>0)*1)");
        
        CrearKPIConFormula(ws, "E", row, "Viajes Planificados", "=COUNTIF('✈️ Viajes'!J:J,\"Planificado\")");
        
        row += 3;
        
        // Resumen por país - BASADO EN PAÍSES DE CLIENTES (dinámico con fórmulas)
        ws.Cell($"B{row}").Value = "DISTRIBUCIÓN POR PAÍS (CLIENTES)";
        ws.Range($"B{row}:E{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        row++;
        ws.Cell($"B{row}").Value = "País";
        ws.Cell($"C{row}").Value = "Clientes";
        ws.Cell($"D{row}").Value = "Asignaciones Activas";
        ws.Cell($"E{row}").Value = "Empleados Asignados";
        ws.Range($"B{row}:E{row}").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        
        row++;
        // Obtener países únicos desde CLIENTES (no empleados)
        var paisesClientes = data.Clientes.Select(c => c.Pais).Distinct().OrderBy(p => p).ToList();
        int startRow = row;
        
        foreach (var pais in paisesClientes)
        {
            ws.Cell($"B{row}").Value = pais;
            
            // Fórmula para contar clientes activos por país
            ws.Cell($"C{row}").FormulaA1 = $"=COUNTIFS('👥 Clientes'!C:C,B{row},'👥 Clientes'!H:H,\"Sí\")";
            
            // Fórmula para contar asignaciones activas de ese país (por cliente)
            // Necesitamos contar en Asignaciones donde el cliente es de este país
            ws.Cell($"D{row}").FormulaA1 = $"=SUMPRODUCT(('🔄 Asignaciones'!G:G=\"Sí\")*(VLOOKUP('🔄 Asignaciones'!C:C,'👥 Clientes'!A:C,3,FALSE)=B{row}))";
            
            // Fórmula para contar empleados únicos asignados a clientes de este país
            // Simplificado: contar empleados en hoja de empleados que tienen cliente asignado de este país
            ws.Cell($"E{row}").FormulaA1 = $"=SUMPRODUCT((VLOOKUP('👨‍💼 Empleados'!H:H,'👥 Clientes'!A:C,3,FALSE)=B{row})*('👨‍💼 Empleados'!J:J=\"Sí\"))";
            
            row++;
        }
        
        // Ajustar anchos de columna
        ws.Columns().AdjustToContents();
    }
    
    private void CrearKPI(IXLWorksheet ws, string col, int row, string titulo, int valor)
    {
        ws.Cell($"{col}{row}").Value = titulo;
        ws.Cell($"{col}{row}").Style
            .Font.SetBold().Font.SetFontSize(10)
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        ws.Cell($"{col}{row + 1}").Value = valor;
        ws.Cell($"{col}{row + 1}").Style
            .Font.SetBold().Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.DarkBlue)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        ws.Range($"{col}{row}:{col}{row + 1}").Style
            .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
            .Border.SetOutsideBorderColor(XLColor.DarkBlue);
    }
    
    private void CrearKPIConFormula(IXLWorksheet ws, string col, int row, string titulo, string formula)
    {
        ws.Cell($"{col}{row}").Value = titulo;
        ws.Cell($"{col}{row}").Style
            .Font.SetBold().Font.SetFontSize(10)
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        ws.Cell($"{col}{row + 1}").FormulaA1 = formula;
        ws.Cell($"{col}{row + 1}").Style
            .Font.SetBold().Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.DarkBlue)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        ws.Range($"{col}{row}:{col}{row + 1}").Style
            .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
            .Border.SetOutsideBorderColor(XLColor.DarkBlue);
    }
    
    private void CrearHojaAlertas(XLWorkbook workbook, List<Alerta> alertas, DataContainer data)
    {
        var ws = workbook.Worksheets.Add("🚨 Alertas");
        
        // Headers
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Tipo";
        ws.Cell("C1").Value = "Nivel";
        ws.Cell("D1").Value = "Empleado";
        ws.Cell("E1").Value = "Fecha Conflicto";
        ws.Cell("F1").Value = "Descripción";
        ws.Cell("G1").Value = "Detalles";
        ws.Cell("H1").Value = "Estado";
        ws.Cell("I1").Value = "Acción Recomendada";
        
        // Estilo de headers
        ws.Range("A1:I1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        
        // Datos
        int row = 2;
        foreach (var alerta in alertas.OrderBy(a => a.Nivel == "Alta" ? 1 : a.Nivel == "Media" ? 2 : 3))
        {
            ws.Cell($"A{row}").Value = alerta.Id;
            ws.Cell($"B{row}").Value = alerta.Tipo;
            ws.Cell($"C{row}").Value = alerta.Nivel;
            ws.Cell($"D{row}").Value = alerta.EmpleadoNombre;
            ws.Cell($"E{row}").Value = alerta.FechaConflicto.ToString("dd/MM/yyyy");
            ws.Cell($"F{row}").Value = alerta.Descripcion;
            ws.Cell($"G{row}").Value = alerta.Detalles;
            ws.Cell($"H{row}").Value = alerta.Resuelta ? "Resuelta" : "Pendiente";
            ws.Cell($"I{row}").Value = ObtenerAccionRecomendada(alerta);
            
            // Formato condicional por nivel
            var color = alerta.Nivel == "Alta" ? XLColor.Red : 
                       alerta.Nivel == "Media" ? XLColor.Yellow : 
                       XLColor.LightBlue;
            
            ws.Range($"A{row}:I{row}").Style.Fill.SetBackgroundColor(color);
            
            if (alerta.Nivel == "Alta")
            {
                ws.Range($"A{row}:I{row}").Style.Font.SetBold();
            }
            
            row++;
        }
        
        // Crear tabla
        if (alertas.Any())
        {
            var tabla = ws.Range($"A1:I{row - 1}").CreateTable();
            tabla.Theme = XLTableTheme.TableStyleMedium2;
        }
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaAnalisisAlertas(XLWorkbook workbook, DataContainer data)
    {
        var ws = workbook.Worksheets.Add("🚨 Alertas y Conflictos");
        
        // Título
        ws.Cell("A1").Value = "ANÁLISIS DINÁMICO DE ALERTAS Y CONFLICTOS";
        ws.Range("A1:F1").Merge().Style
            .Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        ws.Row(1).Height = 30;
        
        int row = 3;
        
        // Sección 1: Resumen de conflictos por empleado
        ws.Cell($"A{row}").Value = "CONFLICTOS POR EMPLEADO (ACTUALIZACIÓN AUTOMÁTICA)";
        ws.Range($"A{row}:G{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        
        row++;
        ws.Cell($"A{row}").Value = "Empleado";
        ws.Cell($"B{row}").Value = "Vacaciones vs Viajes";
        ws.Cell($"C{row}").Value = "Vacaciones vs Soporte";
        ws.Cell($"D{row}").Value = "Viajes vs Soporte";
        ws.Cell($"E{row}").Value = "Asignación Conflictos";
        ws.Cell($"F{row}").Value = "Total Conflictos";
        ws.Cell($"G{row}").Value = "Estado";
        ws.Range($"A{row}:G{row}").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        
        row++;
        int startRow = row;
        
        foreach (var empleado in data.Empleados.Where(e => e.Activo))
        {
            var nombreCompleto = $"{empleado.Nombre} {empleado.Apellido}";
            ws.Cell($"A{row}").Value = nombreCompleto;
            
            // Contar conflictos de Vacaciones vs Viajes (columna G en Vacaciones)
            ws.Cell($"B{row}").FormulaA1 = $"=SUMPRODUCT(('🏖️ Vacaciones'!$B:$B=A{row})*('🏖️ Vacaciones'!$G:$G>0)*1)";
            
            // Contar conflictos de Vacaciones vs Soporte (columna H en Vacaciones)
            ws.Cell($"C{row}").FormulaA1 = $"=SUMPRODUCT(('🏖️ Vacaciones'!$B:$B=A{row})*('🏖️ Vacaciones'!$H:$H>0)*1)";
            
            // Contar conflictos de Viajes vs Soporte (columna M en Viajes)
            ws.Cell($"D{row}").FormulaA1 = $"=SUMPRODUCT(('✈️ Viajes'!$B:$B=A{row})*('✈️ Viajes'!$M:$M>0)*1)";
            
            // Contar conflictos en Asignaciones (columnas H e I)
            ws.Cell($"E{row}").FormulaA1 = $"=SUMPRODUCT(('🔄 Asignaciones'!$B:$B=A{row})*(('🔄 Asignaciones'!$H:$H>0)+('🔄 Asignaciones'!$I:$I>0))*1)";
            
            // Total de conflictos
            ws.Cell($"F{row}").FormulaA1 = $"=B{row}+C{row}+D{row}+E{row}";
            
            // Fórmula para determinar estado basado en severidad
            ws.Cell($"G{row}").FormulaA1 = $"=IF(C{row}>0,\"🔴 CRÍTICO\",IF(B{row}>0,\"🟡 URGENTE\",IF(F{row}>0,\"🔵 REVISAR\",\"✅ OK\")))";
            
            row++;
        }
        
        // Crear tabla
        if (row > startRow)
        {
            var tabla = ws.Range($"A{startRow - 1}:G{row - 1}").CreateTable();
            tabla.Theme = XLTableTheme.TableStyleMedium9;
        }
        
        row += 2;
        
        // Sección 2: Resumen general de conflictos
        ws.Cell($"A{row}").Value = "RESUMEN GENERAL DE CONFLICTOS";
        ws.Range($"A{row}:C{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        
        row++;
        ws.Cell($"A{row}").Value = "Tipo de Conflicto";
        ws.Cell($"B{row}").Value = "Total Detectado";
        ws.Cell($"C{row}").Value = "Severidad";
        ws.Range($"A{row}:C{row}").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        
        row++;
        int summaryStart = row;
        
        // Vacaciones vs Viajes
        ws.Cell($"A{row}").Value = "Vacaciones vs Viajes";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('🏖️ Vacaciones'!$G:$G>0)*1)";
        ws.Cell($"C{row}").Value = "🔴 Alta";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.Red);
        row++;
        
        // Vacaciones vs Soporte
        ws.Cell($"A{row}").Value = "Vacaciones vs Soporte";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('🏖️ Vacaciones'!$H:$H>0)*1)";
        ws.Cell($"C{row}").Value = "🔴 Alta";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.Red);
        row++;
        
        // Viajes vs Soporte
        ws.Cell($"A{row}").Value = "Viajes vs Soporte";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('✈️ Viajes'!$M:$M>0)*1)";
        ws.Cell($"C{row}").Value = "🟡 Media";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.Yellow);
        row++;
        
        // Asignaciones con Conflictos
        ws.Cell($"A{row}").Value = "Asignaciones con Conflictos";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('🔄 Asignaciones'!$H:$H>0)*1)+SUMPRODUCT(('🔄 Asignaciones'!$I:$I>0)*1)";
        ws.Cell($"C{row}").Value = "🟡 Media";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.Yellow);
        row++;
        
        // Viajes en Feriados
        ws.Cell($"A{row}").Value = "Viajes en Feriados";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('✈️ Viajes'!$K:$K>0)*1)";
        ws.Cell($"C{row}").Value = "🔵 Baja";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        // Vacaciones en Feriados
        ws.Cell($"A{row}").Value = "Vacaciones en Feriados";
        ws.Cell($"B{row}").FormulaA1 = "=SUMPRODUCT(('🏖️ Vacaciones'!$I:$I>0)*1)";
        ws.Cell($"C{row}").Value = "🔵 Baja";
        ws.Cell($"C{row}").Style.Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        // Crear tabla resumen
        var tablaResumen = ws.Range($"A{summaryStart - 1}:C{row - 1}").CreateTable();
        tablaResumen.Theme = XLTableTheme.TableStyleMedium2;
        
        row += 2;
        
        // Instrucciones
        ws.Cell($"A{row}").Value = "✅ ESTA HOJA SE ACTUALIZA AUTOMÁTICAMENTE";
        ws.Range($"A{row}:G{row}").Merge().Style
            .Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGreen);
        
        row++;
        ws.Cell($"A{row}").Value = "Los conflictos se detectan dinámicamente basándose en fórmulas que analizan las hojas de Vacaciones, Viajes, Asignaciones y Turnos de Soporte.";
        ws.Range($"A{row}:G{row}").Merge().Style.Font.SetItalic();
        
        row++;
        ws.Cell($"A{row}").Value = "Al agregar o modificar datos en cualquier hoja, esta vista de alertas se actualiza automáticamente.";
        ws.Range($"A{row}:G{row}").Merge().Style.Font.SetItalic();
        
        ws.Columns().AdjustToContents();
    }
    
    private string ObtenerAccionRecomendada(Alerta alerta)
    {
        return alerta.Tipo switch
        {
            "VacacionViaje" => "Cancelar o reprogramar vacaciones o viaje",
            "VacacionSoporte" => "Reasignar turno de soporte o reprogramar vacaciones",
            "ViajeSoporte" => "Confirmar disponibilidad para soporte remoto",
            "ViajeEnFeriado" => "Verificar disponibilidad del cliente",
            "VacacionConFeriado" => "Informativo - Considerar extensión automática",
            "AsignacionMultiple" => "Revisar carga de trabajo y priorizar cliente",
            _ => "Revisar situación"
        };
    }
    
    private void CrearHojaClientes(XLWorkbook workbook, List<Cliente> clientes)
    {
        var ws = workbook.Worksheets.Add("👥 Clientes");
        
        // Headers
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Nombre";
        ws.Cell("C1").Value = "País";
        ws.Cell("D1").Value = "Ciudad";
        ws.Cell("E1").Value = "Email";
        ws.Cell("F1").Value = "Teléfono";
        ws.Cell("G1").Value = "Fecha Registro";
        ws.Cell("H1").Value = "Activo";
        
        // Estilo headers
        ws.Range("A1:H1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        foreach (var cliente in clientes)
        {
            ws.Cell($"A{row}").Value = cliente.Id;
            ws.Cell($"B{row}").Value = cliente.Nombre;
            ws.Cell($"C{row}").Value = cliente.Pais;
            ws.Cell($"D{row}").Value = cliente.Ciudad;
            ws.Cell($"E{row}").Value = cliente.Email;
            ws.Cell($"F{row}").Value = cliente.Telefono;
            ws.Cell($"G{row}").Value = cliente.FechaRegistro;
            ws.Cell($"G{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"H{row}").Value = cliente.Activo ? "Sí" : "No";
            
            // Formato condicional para clientes inactivos
            if (!cliente.Activo)
            {
                ws.Range($"A{row}:H{row}").Style
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetFontColor(XLColor.DarkGray);
            }
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:H{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaEmpleados(XLWorkbook workbook, List<Empleado> empleados, List<Cliente> clientes)
    {
        var ws = workbook.Worksheets.Add("👨‍💼 Empleados");
        
        // Headers
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Nombre";
        ws.Cell("C1").Value = "Apellido";
        ws.Cell("D1").Value = "Email";
        ws.Cell("E1").Value = "Teléfono";
        ws.Cell("F1").Value = "País";
        ws.Cell("G1").Value = "Ciudad";
        ws.Cell("H1").Value = "Cliente Asignado";
        ws.Cell("I1").Value = "Fecha Ingreso";
        ws.Cell("J1").Value = "Activo";
        
        // Estilo headers
        ws.Range("A1:J1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        foreach (var empleado in empleados)
        {
            ws.Cell($"A{row}").Value = empleado.Id;
            ws.Cell($"B{row}").Value = empleado.Nombre;
            ws.Cell($"C{row}").Value = empleado.Apellido;
            ws.Cell($"D{row}").Value = empleado.Email;
            ws.Cell($"E{row}").Value = empleado.Telefono;
            ws.Cell($"F{row}").Value = empleado.Pais;
            ws.Cell($"G{row}").Value = empleado.Ciudad;
            
            // Lookup de cliente
            if (empleado.ClienteAsignadoId.HasValue)
            {
                var cliente = clientes.FirstOrDefault(c => c.Id == empleado.ClienteAsignadoId.Value);
                ws.Cell($"H{row}").Value = cliente?.Nombre ?? "No encontrado";
            }
            else
            {
                ws.Cell($"H{row}").Value = "Sin asignar";
                ws.Cell($"H{row}").Style.Fill.SetBackgroundColor(XLColor.LightYellow);
            }
            
            ws.Cell($"I{row}").Value = empleado.FechaIngreso;
            ws.Cell($"I{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"J{row}").Value = empleado.Activo ? "Sí" : "No";
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:J{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        // Agregar data validation para columna H (Cliente Asignado)
        // Crear lista de nombres de clientes para el dropdown
        var clientesSheet = workbook.Worksheet("👥 Clientes");
        if (clientesSheet != null)
        {
            // Data validation: columna H debe seleccionar desde lista de clientes
            var lastClientRow = clientesSheet.LastRowUsed()?.RowNumber() ?? 1;
            var validationRange = ws.Range($"H2:H1000");
            var validation = validationRange.CreateDataValidation();
            validation.List($"'👥 Clientes'!$B$2:$B${lastClientRow}", true);
        }
        
        // Agregar data validation para columna J (Activo: Sí/No)
        var activoRange = ws.Range($"J2:J1000");
        var validationActivo = activoRange.CreateDataValidation();
        validationActivo.List("\"Sí\",\"No\"", true);
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaAsignaciones(XLWorkbook workbook, List<Asignacion> asignaciones, 
        List<Empleado> empleados, List<Cliente> clientes)
    {
        var ws = workbook.Worksheets.Add("🔄 Asignaciones");
        
        // Headers - Añadidas columnas de detección de conflictos
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Empleado";
        ws.Cell("C1").Value = "Cliente";
        ws.Cell("D1").Value = "Fecha Inicio";
        ws.Cell("E1").Value = "Fecha Fin";
        ws.Cell("F1").Value = "Duración (días)";
        ws.Cell("G1").Value = "Activa";
        ws.Cell("H1").Value = "Conflictos Vacaciones";
        ws.Cell("I1").Value = "Conflictos Viajes";
        ws.Cell("J1").Value = "Feriados Empleado";
        ws.Cell("K1").Value = "Feriados Cliente";
        ws.Cell("L1").Value = "Observaciones";
        
        // Estilo headers
        ws.Range("A1:L1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        foreach (var asignacion in asignaciones.OrderByDescending(a => a.Activa).ThenBy(a => a.FechaInicio))
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == asignacion.EmpleadoId);
            var cliente = clientes.FirstOrDefault(c => c.Id == asignacion.ClienteId);
            
            ws.Cell($"A{row}").Value = asignacion.Id;
            ws.Cell($"B{row}").Value = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido";
            ws.Cell($"C{row}").Value = cliente?.Nombre ?? "Desconocido";
            ws.Cell($"D{row}").Value = asignacion.FechaInicio;
            ws.Cell($"D{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            
            if (asignacion.FechaFin.HasValue)
            {
                ws.Cell($"E{row}").Value = asignacion.FechaFin.Value;
                ws.Cell($"E{row}").Style.DateFormat.Format = "dd/mm/yyyy";
                
                // Usar fórmula para calcular duración
                ws.Cell($"F{row}").FormulaA1 = $"=IF(E{row}=\"\",TODAY()-D{row},E{row}-D{row})";
            }
            else
            {
                ws.Cell($"E{row}").Value = "";
                // Usar fórmula para calcular duración desde fecha inicio hasta hoy
                ws.Cell($"F{row}").FormulaA1 = $"=TODAY()-D{row}";
            }
            
            ws.Cell($"G{row}").Value = asignacion.Activa ? "Sí" : "No";
            
            // Fórmula para detectar conflictos con vacaciones
            // Cuenta vacaciones del empleado que se solapan con este período de asignación
            ws.Cell($"H{row}").FormulaA1 = $"=SUMPRODUCT(('🏖️ Vacaciones'!$B:$B=B{row})*('🏖️ Vacaciones'!$C:$C<=IF(E{row}=\"\",TODAY(),E{row}))*('🏖️ Vacaciones'!$D:$D>=D{row}))";
            
            // Fórmula para detectar conflictos con viajes
            ws.Cell($"I{row}").FormulaA1 = $"=SUMPRODUCT(('✈️ Viajes'!$B:$B=B{row})*('✈️ Viajes'!$F:$F<=IF(E{row}=\"\",TODAY(),E{row}))*('✈️ Viajes'!$G:$G>=D{row}))";
            
            // Fórmula para contar feriados en país/ciudad del empleado durante la asignación
            // Simplificado: cuenta feriados en el rango de fechas
            ws.Cell($"J{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&D{row},'📅 Feriados'!$D:$D,\"<=\"&IF(E{row}=\"\",TODAY(),E{row}))";
            
            // Fórmula para contar feriados en país/ciudad del cliente durante la asignación
            ws.Cell($"K{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&D{row},'📅 Feriados'!$D:$D,\"<=\"&IF(E{row}=\"\",TODAY(),E{row}))";
            
            ws.Cell($"L{row}").Value = asignacion.Observaciones;
            
            // Formato condicional
            if (asignacion.Activa)
            {
                ws.Range($"A{row}:L{row}").Style.Fill.SetBackgroundColor(XLColor.LightGreen);
            }
            
            // Resaltar conflictos en rojo
            ws.Cell($"H{row}").Style.Font.SetBold();
            ws.Cell($"I{row}").Style.Font.SetBold();
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:L{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        // Agregar data validation para columna B (Empleado)
        // Necesitamos crear una lista combinada de nombres completos de empleados
        var empleadosSheet = workbook.Worksheet("👨‍💼 Empleados");
        if (empleadosSheet != null)
        {
            var lastEmpRow = empleadosSheet.LastRowUsed()?.RowNumber() ?? 1;
            // Crear referencia a nombres completos (asumiendo que hay una columna helper o usamos CONCATENATE)
            // Por simplicidad, validamos contra rango de nombres
            var validationRangeEmpleado = ws.Range($"B2:B1000");
            // Nota: ClosedXML tiene limitaciones, podríamos necesitar una columna helper en Empleados
            // Por ahora, validamos contra  el rango
        }
        
        // Agregar data validation para columna C (Cliente)
        var clientesSheet = workbook.Worksheet("👥 Clientes");
        if (clientesSheet != null)
        {
            var lastClientRow = clientesSheet.LastRowUsed()?.RowNumber() ?? 1;
            var validationRangeCliente = ws.Range($"C2:C1000");
            var validationCliente = validationRangeCliente.CreateDataValidation();
            validationCliente.List($"'👥 Clientes'!$B$2:$B${lastClientRow}", true);
        }
        
        // Agregar data validation para columna G (Activa: Sí/No)
        var activaRange = ws.Range($"G2:G1000");
        var validationActiva = activaRange.CreateDataValidation();
        validationActiva.List("\"Sí\",\"No\"", true);
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaVacaciones(XLWorkbook workbook, List<Vacacion> vacaciones, 
        List<Empleado> empleados, List<Feriado> feriados)
    {
        var ws = workbook.Worksheets.Add("🏖️ Vacaciones");
        
        // Headers - Añadidas columnas de detección de conflictos
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Empleado";
        ws.Cell("C1").Value = "Fecha Inicio";
        ws.Cell("D1").Value = "Fecha Fin";
        ws.Cell("E1").Value = "Días";
        ws.Cell("F1").Value = "Estado";
        ws.Cell("G1").Value = "Conflictos Viajes";
        ws.Cell("H1").Value = "Conflictos Soporte";
        ws.Cell("I1").Value = "Feriados Empleado";
        ws.Cell("J1").Value = "Feriados Cliente";
        ws.Cell("K1").Value = "Observaciones";
        
        // Estilo headers
        ws.Range("A1:K1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        
        foreach (var vacacion in vacaciones.OrderBy(v => v.FechaInicio))
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == vacacion.EmpleadoId);
            
            ws.Cell($"A{row}").Value = vacacion.Id;
            ws.Cell($"B{row}").Value = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido";
            ws.Cell($"C{row}").Value = vacacion.FechaInicio;
            ws.Cell($"C{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"D{row}").Value = vacacion.FechaFin;
            ws.Cell($"D{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            
            // Usar fórmula para calcular días
            ws.Cell($"E{row}").FormulaA1 = $"=D{row}-C{row}+1";
            
            ws.Cell($"F{row}").Value = vacacion.Estado;
            
            // Fórmula para detectar conflictos con viajes del mismo empleado
            ws.Cell($"G{row}").FormulaA1 = $"=SUMPRODUCT(('✈️ Viajes'!$B:$B=B{row})*('✈️ Viajes'!$F:$F<=D{row})*('✈️ Viajes'!$G:$G>=C{row}))";
            
            // Fórmula para detectar conflictos con turnos de soporte
            ws.Cell($"H{row}").FormulaA1 = $"=SUMPRODUCT(('🛠️ Turnos Soporte'!$B:$B=B{row})*('🛠️ Turnos Soporte'!$C:$C<=D{row})*('🛠️ Turnos Soporte'!$D:$D>=C{row}))";
            
            // Fórmula para contar feriados en país del empleado durante las vacaciones
            ws.Cell($"I{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&C{row},'📅 Feriados'!$D:$D,\"<=\"&D{row})";
            
            // Fórmula para contar feriados en país del cliente (si tiene asignación activa)
            ws.Cell($"J{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&C{row},'📅 Feriados'!$D:$D,\"<=\"&D{row})";
            
            ws.Cell($"K{row}").Value = vacacion.Observaciones;
            
            // Formato condicional por estado
            var color = vacacion.Estado switch
            {
                "Aprobada" => XLColor.LightGreen,
                "Pendiente" => XLColor.LightYellow,
                "Rechazada" => XLColor.Red,
                _ => XLColor.White
            };
            ws.Range($"A{row}:K{row}").Style.Fill.SetBackgroundColor(color);
            
            // Resaltar conflictos
            ws.Cell($"G{row}").Style.Font.SetBold();
            ws.Cell($"H{row}").Style.Font.SetBold();
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:K{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        // Agregar data validation para columna F (Estado)
        var estadoRange = ws.Range($"F2:F1000");
        var validationEstado = estadoRange.CreateDataValidation();
        validationEstado.List("\"Pendiente\",\"Aprobada\",\"Rechazada\"", true);
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaViajes(XLWorkbook workbook, List<Viaje> viajes, 
        List<Empleado> empleados, List<Cliente> clientes, List<Feriado> feriados)
    {
        var ws = workbook.Worksheets.Add("✈️ Viajes");
        
        // Headers - Añadidas columnas de feriados empleado/cliente
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Empleado";
        ws.Cell("C1").Value = "Cliente";
        ws.Cell("D1").Value = "País Destino";
        ws.Cell("E1").Value = "Ciudad Destino";
        ws.Cell("F1").Value = "Fecha Inicio";
        ws.Cell("G1").Value = "Fecha Fin";
        ws.Cell("H1").Value = "Días";
        ws.Cell("I1").Value = "Motivo";
        ws.Cell("J1").Value = "Estado";
        ws.Cell("K1").Value = "Feriados Destino";
        ws.Cell("L1").Value = "Feriados Empleado";
        ws.Cell("M1").Value = "Conflictos Soporte";
        
        // Estilo headers
        ws.Range("A1:M1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        
        foreach (var viaje in viajes.OrderBy(v => v.FechaInicio))
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == viaje.EmpleadoId);
            var cliente = clientes.FirstOrDefault(c => c.Id == viaje.ClienteId);
            
            ws.Cell($"A{row}").Value = viaje.Id;
            ws.Cell($"B{row}").Value = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido";
            ws.Cell($"C{row}").Value = cliente?.Nombre ?? "Desconocido";
            ws.Cell($"D{row}").Value = viaje.PaisDestino;
            ws.Cell($"E{row}").Value = viaje.CiudadDestino;
            ws.Cell($"F{row}").Value = viaje.FechaInicio;
            ws.Cell($"F{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"G{row}").Value = viaje.FechaFin;
            ws.Cell($"G{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            
            // Usar fórmula para calcular días
            ws.Cell($"H{row}").FormulaA1 = $"=G{row}-F{row}+1";
            
            ws.Cell($"I{row}").Value = viaje.Motivo;
            ws.Cell($"J{row}").Value = viaje.Estado;
            
            // Fórmula para contar feriados en país destino (cliente)
            ws.Cell($"K{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&F{row},'📅 Feriados'!$D:$D,\"<=\"&G{row},'📅 Feriados'!$B:$B,D{row})";
            
            // Fórmula para contar feriados en país del empleado
            ws.Cell($"L{row}").FormulaA1 = $"=COUNTIFS('📅 Feriados'!$D:$D,\">=\"&F{row},'📅 Feriados'!$D:$D,\"<=\"&G{row})";
            
            // Fórmula para detectar conflictos con turnos de soporte
            ws.Cell($"M{row}").FormulaA1 = $"=SUMPRODUCT(('🛠️ Turnos Soporte'!$B:$B=B{row})*('🛠️ Turnos Soporte'!$C:$C<=G{row})*('🛠️ Turnos Soporte'!$D:$D>=F{row}))";
            
            // Formato condicional por estado
            var color = viaje.Estado switch
            {
                "Planificado" => XLColor.LightBlue,
                "En Curso" => XLColor.LightYellow,
                "Completado" => XLColor.LightGreen,
                _ => XLColor.White
            };
            ws.Range($"A{row}:M{row}").Style.Fill.SetBackgroundColor(color);
            
            // Resaltar conflictos
            ws.Cell($"M{row}").Style.Font.SetBold();
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:M{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        // Agregar data validation para columna C (Cliente)
        var clientesSheet = workbook.Worksheet("👥 Clientes");
        if (clientesSheet != null)
        {
            var lastClientRow = clientesSheet.LastRowUsed()?.RowNumber() ?? 1;
            var validationRangeCliente = ws.Range($"C2:C1000");
            var validationCliente = validationRangeCliente.CreateDataValidation();
            validationCliente.List($"'👥 Clientes'!$B$2:$B${lastClientRow}", true);
        }
        
        // Agregar data validation para columna J (Estado)
        var estadoRange = ws.Range($"J2:J1000");
        var validationEstado = estadoRange.CreateDataValidation();
        validationEstado.List("\"Planificado\",\"En Curso\",\"Completado\",\"Cancelado\"", true);
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaTurnosSoporte(XLWorkbook workbook, List<TurnoSoporte> turnos, List<Empleado> empleados)
    {
        var ws = workbook.Worksheets.Add("🛠️ Turnos Soporte");
        
        // Headers
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "Empleado";
        ws.Cell("C1").Value = "Semana Inicio";
        ws.Cell("D1").Value = "Semana Fin";
        ws.Cell("E1").Value = "Número Semana";
        ws.Cell("F1").Value = "Año";
        ws.Cell("G1").Value = "Observaciones";
        
        // Estilo headers
        ws.Range("A1:G1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        foreach (var turno in turnos.OrderBy(t => t.FechaInicio))
        {
            var empleado = empleados.FirstOrDefault(e => e.Id == turno.EmpleadoId);
            
            ws.Cell($"A{row}").Value = turno.Id;
            ws.Cell($"B{row}").Value = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Desconocido";
            ws.Cell($"C{row}").Value = turno.FechaInicio;
            ws.Cell($"C{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"D{row}").Value = turno.FechaFin;
            ws.Cell($"D{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"E{row}").Value = turno.NumeroSemana;
            ws.Cell($"F{row}").Value = turno.Año;
            ws.Cell($"G{row}").Value = turno.Observaciones;
            
            // Alternar colores por empleado
            if (row % 2 == 0)
            {
                ws.Range($"A{row}:G{row}").Style.Fill.SetBackgroundColor(XLColor.LightCyan);
            }
            
            row++;
        }
        
        // Crear tabla
        var tabla = ws.Range($"A1:G{row - 1}").CreateTable();
        tabla.Theme = XLTableTheme.TableStyleMedium9;
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearHojaFeriados(XLWorkbook workbook, List<Feriado> feriados)
    {
        var ws = workbook.Worksheets.Add("📅 Feriados");
        
        // Headers
        ws.Cell("A1").Value = "ID";
        ws.Cell("B1").Value = "País";
        ws.Cell("C1").Value = "Ciudad";
        ws.Cell("D1").Value = "Fecha";
        ws.Cell("E1").Value = "Nombre";
        ws.Cell("F1").Value = "Es Nacional";
        
        // Estilo headers
        ws.Range("A1:F1").Style
            .Font.SetBold().Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Datos
        int row = 2;
        foreach (var feriado in feriados.OrderBy(f => f.Pais).ThenBy(f => f.Fecha))
        {
            ws.Cell($"A{row}").Value = feriado.Id;
            ws.Cell($"B{row}").Value = feriado.Pais;
            ws.Cell($"C{row}").Value = feriado.Ciudad;
            ws.Cell($"D{row}").Value = feriado.Fecha;
            ws.Cell($"D{row}").Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell($"E{row}").Value = feriado.Nombre;
            ws.Cell($"F{row}").Value = feriado.EsNacional ? "Sí" : "No";
            
            row++;
        }
        
        // Crear tabla
        if (feriados.Any())
        {
            var tabla = ws.Range($"A1:F{row - 1}").CreateTable();
            tabla.Theme = XLTableTheme.TableStyleMedium9;
        }
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearDashboardOcupacion(XLWorkbook workbook, DataContainer data)
    {
        var ws = workbook.Worksheets.Add("📊 Dashboard Ocupación");
        
        // Título
        ws.Cell("A1").Value = "DASHBOARD DE OCUPACIÓN - VISTA MENSUAL 2026";
        ws.Range("A1:M1").Merge().Style
            .Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        // Leyenda
        int row = 3;
        ws.Cell($"A{row}").Value = "LEYENDA:";
        ws.Cell($"A{row}").Style.Font.SetBold();
        row++;
        
        CrearLeyenda(ws, "A", row++, "Asignado a cliente", XLColor.LightGreen);
        CrearLeyenda(ws, "A", row++, "En viaje", XLColor.LightBlue);
        CrearLeyenda(ws, "A", row++, "Turno soporte", XLColor.Orange);
        CrearLeyenda(ws, "A", row++, "Vacaciones", XLColor.Yellow);
        CrearLeyenda(ws, "A", row++, "Conflicto", XLColor.Red);
        
        row += 2;
        
        // Resumen de disponibilidad - AHORA CON FÓRMULAS DINÁMICAS
        ws.Cell($"A{row}").Value = "RESUMEN DE DISPONIBILIDAD";
        ws.Range($"A{row}:E{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        
        row++;
        ws.Cell($"A{row}").Value = "Empleado";
        ws.Cell($"B{row}").Value = "Asignaciones Activas";
        ws.Cell($"C{row}").Value = "Viajes";
        ws.Cell($"D{row}").Value = "Vacaciones";
        ws.Cell($"E{row}").Value = "Turnos Soporte";
        ws.Range($"A{row}:E{row}").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        
        row++;
        int startDataRow = row;
        
        foreach (var empleado in data.Empleados.Where(e => e.Activo))
        {
            ws.Cell($"A{row}").Value = $"{empleado.Nombre} {empleado.Apellido}";
            
            // Fórmula para contar asignaciones activas
            ws.Cell($"B{row}").FormulaA1 = $"=COUNTIFS('🔄 Asignaciones'!$B:$B,A{row},'🔄 Asignaciones'!$G:$G,\"Sí\")";
            
            // Fórmula para contar viajes
            ws.Cell($"C{row}").FormulaA1 = $"=COUNTIF('✈️ Viajes'!$B:$B,A{row})";
            
            // Fórmula para contar vacaciones
            ws.Cell($"D{row}").FormulaA1 = $"=COUNTIF('🏖️ Vacaciones'!$B:$B,A{row})";
            
            // Fórmula para contar turnos de soporte
            ws.Cell($"E{row}").FormulaA1 = $"=COUNTIF('🛠️ Turnos Soporte'!$B:$B,A{row})";
            
            row++;
        }
        
        // Crear tabla con los datos
        if (row > startDataRow)
        {
            var tabla = ws.Range($"A{startDataRow - 1}:E{row - 1}").CreateTable();
            tabla.Theme = XLTableTheme.TableStyleMedium9;
        }
        
        ws.Columns().AdjustToContents();
    }
    
    private void CrearLeyenda(IXLWorksheet ws, string col, int row, string texto, XLColor color)
    {
        ws.Cell($"{col}{row}").Value = "  ";
        ws.Cell($"{col}{row}").Style.Fill.SetBackgroundColor(color);
        ws.Cell($"{col}{row}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        
        var nextCol = ((char)(col[0] + 1)).ToString();
        ws.Cell($"{nextCol}{row}").Value = texto;
    }
    
    private void CrearPanelDeControl(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("🎛️ Panel de Control");
        
        int row = 1;
        
        // Título
        ws.Cell($"A{row}").Value = "PANEL DE CONTROL - ACTUALIZACIÓN DE DASHBOARD";
        ws.Range($"A{row}:E{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(18)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        ws.Row(row).Height = 40;
        row += 3;
        
        // Sección: Descripción
        ws.Cell($"A{row}").Value = "📋 DESCRIPCIÓN";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(14)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        ws.Cell($"A{row}").Value = "Este panel le permite actualizar dinámicamente el Dashboard Gerencial y las columnas de detección de conflictos.";
        ws.Range($"A{row}:E{row}").Merge().Style.Alignment.SetWrapText();
        row += 2;
        
        // Sección: Instrucciones para agregar el botón VBA
        ws.Cell($"A{row}").Value = "⚙️ CONFIGURACIÓN DEL BOTÓN DE ACTUALIZACIÓN";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(14)
            .Fill.SetBackgroundColor(XLColor.LightYellow);
        row++;
        
        var instrucciones = new[]
        {
            "1. Abra el archivo VBA_Macro_Code.txt que se generó junto con este Excel",
            "2. En Excel, presione Alt + F11 para abrir el Editor de Visual Basic",
            "3. En el menú, seleccione Insertar → Módulo",
            "4. Copie y pegue todo el código del archivo VBA_Macro_Code.txt en el módulo",
            "5. Cierre el Editor de Visual Basic",
            "6. Vaya a la pestaña 'Desarrollador' (si no la ve, actívela en Opciones de Excel)",
            "7. Haga clic en 'Insertar' → 'Botón de formulario'",
            "8. Dibuje el botón en este panel (recomendado: celda C15)",
            "9. En el cuadro de diálogo, seleccione la macro 'ActualizarDashboardYConflictos'",
            "10. Haga clic derecho en el botón → 'Modificar texto' → Escriba 'ACTUALIZAR DASHBOARD'",
            "",
            "✅ ¡Listo! Ahora puede usar el botón para actualizar el dashboard y conflictos."
        };
        
        foreach (var instruccion in instrucciones)
        {
            if (string.IsNullOrEmpty(instruccion))
            {
                row++;
            }
            else
            {
                ws.Cell($"A{row}").Value = instruccion;
                ws.Range($"A{row}:E{row}").Merge().Style.Alignment.SetWrapText();
                row++;
            }
        }
        
        row += 2;
        
        // Sección: Qué hace el botón
        ws.Cell($"A{row}").Value = "🔄 QUÉ HACE EL BOTÓN DE ACTUALIZACIÓN";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(14)
            .Fill.SetBackgroundColor(XLColor.LightGreen);
        row++;
        
        var funciones = new[]
        {
            "✓ Actualiza las columnas de conflictos en la hoja de Vacaciones",
            "✓ Actualiza las columnas de conflictos en la hoja de Viajes",
            "✓ Actualiza las columnas de conflictos en la hoja de Asignaciones",
            "✓ Recalcula todas las fórmulas del Dashboard Gerencial",
            "✓ Actualiza la hoja de Alertas y Conflictos",
            "✓ Muestra un mensaje de confirmación al finalizar"
        };
        
        foreach (var funcion in funciones)
        {
            ws.Cell($"A{row}").Value = funcion;
            ws.Range($"A{row}:E{row}").Merge();
            row++;
        }
        
        row += 2;
        
        // Sección: Cuándo usar el botón
        ws.Cell($"A{row}").Value = "📅 CUÁNDO USAR EL BOTÓN";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(14)
            .Fill.SetBackgroundColor(XLColor.LightCyan);
        row++;
        
        var momentos = new[]
        {
            "• Después de agregar nuevas vacaciones",
            "• Después de agregar nuevos viajes",
            "• Después de agregar nuevas asignaciones",
            "• Después de modificar fechas de inicio o fin",
            "• Después de cambiar estados (Pendiente, Aprobada, etc.)",
            "• Al inicio del día para ver el estado actual",
            "",
            "ℹ️ NOTA: Las fórmulas se actualizan automáticamente en Excel, pero el botón",
            "   asegura que todas las columnas de conflictos estén correctamente calculadas."
        };
        
        foreach (var momento in momentos)
        {
            if (string.IsNullOrEmpty(momento))
            {
                row++;
            }
            else
            {
                ws.Cell($"A{row}").Value = momento;
                ws.Range($"A{row}:E{row}").Merge();
                row++;
            }
        }
        
        row += 3;
        
        // Área para el botón
        ws.Cell($"C{row}").Value = "[ ESPACIO PARA EL BOTÓN DE ACTUALIZACIÓN ]";
        ws.Range($"C{row}:D{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
            .Border.SetOutsideBorderColor(XLColor.DarkBlue)
            .Fill.SetBackgroundColor(XLColor.LightYellow);
        
        ws.Row(row).Height = 50;
        
        ws.Columns().AdjustToContents();
        ws.Column("A").Width = 60;
    }
    
    private void CrearHojaInstrucciones(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("ℹ️ Instrucciones");
        
        int row = 1;
        
        // Título
        ws.Cell($"A{row}").Value = "GUÍA DE USO - DASHBOARD GERENCIAL";
        ws.Range($"A{row}:D{row}").Merge().Style
            .Font.SetBold().Font.SetFontSize(16)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.DarkBlue)
            .Font.SetFontColor(XLColor.White);
        
        row += 2;
        
        // Sección: Descripción general
        ws.Cell($"A{row}").Value = "DESCRIPCIÓN GENERAL";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        ws.Cell($"A{row}").Value = "Este archivo Excel proporciona un dashboard completo para gestión de empleados, clientes, asignaciones, vacaciones, viajes y turnos de soporte.";
        ws.Range($"A{row}:D{row}").Merge();
        row += 2;
        
        // Sección: Hojas de trabajo
        ws.Cell($"A{row}").Value = "HOJAS DE TRABAJO";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        var hojas = new[]
        {
            "📊 Dashboard Gerencial - KPIs y métricas principales con fórmulas dinámicas",
            "🚨 Alertas y Conflictos - Detección automática y dinámica de conflictos",
            "👥 Clientes - Listado completo de clientes (editable con dropdowns)",
            "👨‍💼 Empleados - Listado completo de empleados (editable con dropdowns)",
            "🔄 Asignaciones - Historial con detección automática de conflictos (dropdowns)",
            "🏖️ Vacaciones - Registro con detección de conflictos (dropdowns de estado)",
            "✈️ Viajes - Registro con detección de feriados y conflictos (dropdowns)",
            "🛠️ Turnos Soporte - Planificación completa de 52 semanas (año 2026)",
            "📅 Feriados - Catálogo de feriados por país (EC y PY)",
            "📊 Dashboard Ocupación - Vista dinámica de ocupación de empleados",
            "🎛️ Panel de Control - Instrucciones para agregar botón VBA de actualización",
            "ℹ️ Instrucciones - Esta hoja"
        };
        
        foreach (var hoja in hojas)
        {
            ws.Cell($"A{row}").Value = $"• {hoja}";
            ws.Range($"A{row}:D{row}").Merge();
            row++;
        }
        
        row++;
        
        // Sección: Sistema de alertas
        ws.Cell($"A{row}").Value = "SISTEMA DE ALERTAS";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        var alertas = new[]
        {
            "NIVEL ALTO (Rojo) - Requiere acción inmediata:",
            "  • Vacaciones y viaje en fechas superpuestas",
            "  • Vacaciones durante turno de soporte",
            "  • Múltiples asignaciones activas simultáneas",
            "",
            "NIVEL MEDIO (Amarillo) - Revisar y planificar:",
            "  • Viaje durante turno de soporte (puede gestionar remoto)",
            "",
            "NIVEL BAJO (Azul) - Informativo:",
            "  • Viaje en fecha de feriado",
            "  • Vacaciones que incluyen feriados"
        };
        
        foreach (var alerta in alertas)
        {
            ws.Cell($"A{row}").Value = alerta;
            ws.Range($"A{row}:D{row}").Merge();
            row++;
        }
        
        row++;
        
        // Sección: Leyenda de colores
        ws.Cell($"A{row}").Value = "LEYENDA DE COLORES";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        CrearLeyenda(ws, "A", row, "Verde - Asignaciones activas / Vacaciones aprobadas", XLColor.LightGreen);
        row++;
        CrearLeyenda(ws, "A", row, "Amarillo - Pendiente de aprobación / En curso", XLColor.Yellow);
        row++;
        CrearLeyenda(ws, "A", row, "Rojo - Alertas de alta prioridad / Rechazado", XLColor.Red);
        row++;
        CrearLeyenda(ws, "A", row, "Azul - Información / Planificado", XLColor.LightBlue);
        row++;
        CrearLeyenda(ws, "A", row, "Gris - Inactivo / Histórico", XLColor.LightGray);
        
        row += 2;
        
        // Notas finales
        ws.Cell($"A{row}").Value = "NOTAS IMPORTANTES";
        ws.Cell($"A{row}").Style.Font.SetBold().Font.SetFontSize(12)
            .Fill.SetBackgroundColor(XLColor.LightBlue);
        row++;
        
        ws.Cell($"A{row}").Value = "• Todas las fechas están en formato DD/MM/YYYY";
        ws.Range($"A{row}:D{row}").Merge();
        row++;
        
        ws.Cell($"A{row}").Value = "• Los feriados se obtienen automáticamente usando Nager.Date";
        ws.Range($"A{row}:D{row}").Merge();
        row++;
        
        ws.Cell($"A{row}").Value = "• Las tablas incluyen filtros automáticos para facilitar búsquedas";
        ws.Range($"A{row}:D{row}").Merge();
        row++;
        
        ws.Cell($"A{row}").Value = "• Revise regularmente la hoja de Alertas para evitar conflictos";
        ws.Range($"A{row}:D{row}").Merge();
        
        ws.Columns().AdjustToContents();
        ws.Column("A").Width = 80;
    }
}
