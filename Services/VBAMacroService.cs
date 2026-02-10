using System.Text;

namespace ExcelDashboardGenerator.Services;

/// <summary>
/// Servicio para generar código VBA que se embebe en el archivo Excel
/// </summary>
public class VBAMacroService
{
    /// <summary>
    /// Genera el código VBA completo para actualizar el dashboard y detectar conflictos
    /// </summary>
    public string GenerarCodigoVBA()
    {
        var sb = new StringBuilder();
        
        // Módulo principal con todas las funciones
        sb.AppendLine("Option Explicit");
        sb.AppendLine();
        sb.AppendLine("' ========================================");
        sb.AppendLine("' MÓDULO PRINCIPAL - ACTUALIZACIÓN DE DASHBOARD");
        sb.AppendLine("' ========================================");
        sb.AppendLine();
        
        // Función principal que actualiza todo
        sb.AppendLine("Sub ActualizarDashboardYConflictos()");
        sb.AppendLine("    On Error GoTo ErrorHandler");
        sb.AppendLine("    ");
        sb.AppendLine("    Application.ScreenUpdating = False");
        sb.AppendLine("    Application.Calculation = xlCalculationManual");
        sb.AppendLine("    ");
        sb.AppendLine("    ' Mostrar progreso");
        sb.AppendLine("    Application.StatusBar = \"Actualizando columnas de conflictos...\"");
        sb.AppendLine("    ");
        sb.AppendLine("    ' 1. Actualizar columnas de conflictos en Vacaciones");
        sb.AppendLine("    Call ActualizarConflictosVacaciones");
        sb.AppendLine("    ");
        sb.AppendLine("    ' 2. Actualizar columnas de conflictos en Viajes");
        sb.AppendLine("    Application.StatusBar = \"Actualizando conflictos de viajes...\"");
        sb.AppendLine("    Call ActualizarConflictosViajes");
        sb.AppendLine("    ");
        sb.AppendLine("    ' 3. Actualizar columnas de conflictos en Asignaciones");
        sb.AppendLine("    Application.StatusBar = \"Actualizando conflictos de asignaciones...\"");
        sb.AppendLine("    Call ActualizarConflictosAsignaciones");
        sb.AppendLine("    ");
        sb.AppendLine("    ' 4. Recalcular todas las fórmulas");
        sb.AppendLine("    Application.StatusBar = \"Recalculando fórmulas...\"");
        sb.AppendLine("    Application.Calculation = xlCalculationAutomatic");
        sb.AppendLine("    Application.Calculate");
        sb.AppendLine("    ");
        sb.AppendLine("    ' 5. Actualizar hoja de Alertas y Conflictos");
        sb.AppendLine("    Application.StatusBar = \"Actualizando hoja de alertas...\"");
        sb.AppendLine("    Call ActualizarHojaAlertas");
        sb.AppendLine("    ");
        sb.AppendLine("    Application.ScreenUpdating = True");
        sb.AppendLine("    Application.StatusBar = False");
        sb.AppendLine("    ");
        sb.AppendLine("    MsgBox \"✅ Actualización completada exitosamente!\" & vbCrLf & vbCrLf & _");
        sb.AppendLine("           \"- Conflictos detectados y actualizados\" & vbCrLf & _");
        sb.AppendLine("           \"- Dashboard recalculado\" & vbCrLf & _");
        sb.AppendLine("           \"- Alertas actualizadas\", vbInformation, \"Actualización Completa\"");
        sb.AppendLine("    ");
        sb.AppendLine("    Exit Sub");
        sb.AppendLine("    ");
        sb.AppendLine("ErrorHandler:");
        sb.AppendLine("    Application.ScreenUpdating = True");
        sb.AppendLine("    Application.Calculation = xlCalculationAutomatic");
        sb.AppendLine("    Application.StatusBar = False");
        sb.AppendLine("    MsgBox \"❌ Error al actualizar: \" & Err.Description, vbCritical, \"Error\"");
        sb.AppendLine("End Sub");
        sb.AppendLine();
        
        // Función para actualizar conflictos en Vacaciones
        sb.AppendLine("Private Sub ActualizarConflictosVacaciones()");
        sb.AppendLine("    Dim ws As Worksheet");
        sb.AppendLine("    Dim lastRow As Long");
        sb.AppendLine("    Dim i As Long");
        sb.AppendLine("    ");
        sb.AppendLine("    On Error Resume Next");
        sb.AppendLine("    Set ws = ThisWorkbook.Worksheets(\"🏖️ Vacaciones\")");
        sb.AppendLine("    If ws Is Nothing Then Exit Sub");
        sb.AppendLine("    On Error GoTo 0");
        sb.AppendLine("    ");
        sb.AppendLine("    lastRow = ws.Cells(ws.Rows.Count, \"A\").End(xlUp).Row");
        sb.AppendLine("    ");
        sb.AppendLine("    ' Asegurar que las columnas de conflictos existen y tienen fórmulas");
        sb.AppendLine("    For i = 2 To lastRow");
        sb.AppendLine("        ' Columna G: Conflictos Viajes");
        sb.AppendLine("        If Len(ws.Cells(i, 7).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 7).Formula = \"=SUMPRODUCT(('✈️ Viajes'!$B:$B=B\" & i & \")*('✈️ Viajes'!$F:$F<=D\" & i & \")*('✈️ Viajes'!$G:$G>=C\" & i & \"))\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna H: Conflictos Soporte");
        sb.AppendLine("        If Len(ws.Cells(i, 8).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 8).Formula = \"=SUMPRODUCT(('🛠️ Turnos Soporte'!$B:$B=B\" & i & \")*('🛠️ Turnos Soporte'!$C:$C<=D\" & i & \")*('🛠️ Turnos Soporte'!$D:$D>=C\" & i & \"))\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna I: Feriados Empleado");
        sb.AppendLine("        If Len(ws.Cells(i, 9).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 9).Formula = \"=COUNTIFS('📅 Feriados'!$D:$D,\"\">=\"\"&C\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&D\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna J: Feriados Cliente");
        sb.AppendLine("        If Len(ws.Cells(i, 10).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 10).Formula = \"=COUNTIFS('📅 Feriados'!$D:$D,\"\">=\"\"&C\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&D\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("    Next i");
        sb.AppendLine("End Sub");
        sb.AppendLine();
        
        // Función para actualizar conflictos en Viajes
        sb.AppendLine("Private Sub ActualizarConflictosViajes()");
        sb.AppendLine("    Dim ws As Worksheet");
        sb.AppendLine("    Dim lastRow As Long");
        sb.AppendLine("    Dim i As Long");
        sb.AppendLine("    ");
        sb.AppendLine("    On Error Resume Next");
        sb.AppendLine("    Set ws = ThisWorkbook.Worksheets(\"✈️ Viajes\")");
        sb.AppendLine("    If ws Is Nothing Then Exit Sub");
        sb.AppendLine("    On Error GoTo 0");
        sb.AppendLine("    ");
        sb.AppendLine("    lastRow = ws.Cells(ws.Rows.Count, \"A\").End(xlUp).Row");
        sb.AppendLine("    ");
        sb.AppendLine("    For i = 2 To lastRow");
        sb.AppendLine("        ' Columna K: Feriados Destino");
        sb.AppendLine("        If Len(ws.Cells(i, 11).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 11).Formula = \"=COUNTIFS('📅 Feriados'!$B:$B,D\" & i & \",'📅 Feriados'!$D:$D,\"\">=\"\"&F\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&G\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna L: Feriados Empleado");
        sb.AppendLine("        If Len(ws.Cells(i, 12).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 12).Formula = \"=COUNTIFS('📅 Feriados'!$D:$D,\"\">=\"\"&F\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&G\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna M: Conflictos Soporte");
        sb.AppendLine("        If Len(ws.Cells(i, 13).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 13).Formula = \"=SUMPRODUCT(('🛠️ Turnos Soporte'!$B:$B=B\" & i & \")*('🛠️ Turnos Soporte'!$C:$C<=G\" & i & \")*('🛠️ Turnos Soporte'!$D:$D>=F\" & i & \"))\"");
        sb.AppendLine("        End If");
        sb.AppendLine("    Next i");
        sb.AppendLine("End Sub");
        sb.AppendLine();
        
        // Función para actualizar conflictos en Asignaciones
        sb.AppendLine("Private Sub ActualizarConflictosAsignaciones()");
        sb.AppendLine("    Dim ws As Worksheet");
        sb.AppendLine("    Dim lastRow As Long");
        sb.AppendLine("    Dim i As Long");
        sb.AppendLine("    ");
        sb.AppendLine("    On Error Resume Next");
        sb.AppendLine("    Set ws = ThisWorkbook.Worksheets(\"🔄 Asignaciones\")");
        sb.AppendLine("    If ws Is Nothing Then Exit Sub");
        sb.AppendLine("    On Error GoTo 0");
        sb.AppendLine("    ");
        sb.AppendLine("    lastRow = ws.Cells(ws.Rows.Count, \"A\").End(xlUp).Row");
        sb.AppendLine("    ");
        sb.AppendLine("    For i = 2 To lastRow");
        sb.AppendLine("        ' Columna H: Conflictos Vacaciones");
        sb.AppendLine("        If Len(ws.Cells(i, 8).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 8).Formula = \"=SUMPRODUCT(('🏖️ Vacaciones'!$B:$B=B\" & i & \")*('🏖️ Vacaciones'!$C:$C<=E\" & i & \")*('🏖️ Vacaciones'!$D:$D>=D\" & i & \"))\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna I: Conflictos Viajes");
        sb.AppendLine("        If Len(ws.Cells(i, 9).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 9).Formula = \"=SUMPRODUCT(('✈️ Viajes'!$B:$B=B\" & i & \")*('✈️ Viajes'!$F:$F<=E\" & i & \")*('✈️ Viajes'!$G:$G>=D\" & i & \"))\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna J: Feriados Empleado");
        sb.AppendLine("        If Len(ws.Cells(i, 10).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 10).Formula = \"=COUNTIFS('📅 Feriados'!$D:$D,\"\">=\"\"&D\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&E\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("        ");
        sb.AppendLine("        ' Columna K: Feriados Cliente");
        sb.AppendLine("        If Len(ws.Cells(i, 11).Formula) = 0 Then");
        sb.AppendLine("            ws.Cells(i, 11).Formula = \"=COUNTIFS('📅 Feriados'!$D:$D,\"\">=\"\"&D\" & i & \",'📅 Feriados'!$D:$D,\"\"<=\"\"&E\" & i & \")\"");
        sb.AppendLine("        End If");
        sb.AppendLine("    Next i");
        sb.AppendLine("End Sub");
        sb.AppendLine();
        
        // Función para actualizar la hoja de alertas (recalcular fórmulas)
        sb.AppendLine("Private Sub ActualizarHojaAlertas()");
        sb.AppendLine("    Dim ws As Worksheet");
        sb.AppendLine("    ");
        sb.AppendLine("    On Error Resume Next");
        sb.AppendLine("    Set ws = ThisWorkbook.Worksheets(\"🚨 Alertas y Conflictos\")");
        sb.AppendLine("    If ws Is Nothing Then Exit Sub");
        sb.AppendLine("    On Error GoTo 0");
        sb.AppendLine("    ");
        sb.AppendLine("    ' Simplemente recalcular la hoja");
        sb.AppendLine("    ws.Calculate");
        sb.AppendLine("End Sub");
        sb.AppendLine();
        
        return sb.ToString();
    }
}
