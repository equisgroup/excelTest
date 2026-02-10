using ExcelDashboardGenerator.Data;
using ExcelDashboardGenerator.Services;

namespace ExcelDashboardGenerator;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  GENERADOR DE EXCEL - DASHBOARD GERENCIAL                 ║");
        Console.WriteLine("║  Sistema de Control de Asignaciones de Empleados          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        try
        {
            // 1. Generar datos de ejemplo
            Console.WriteLine("📊 Paso 1/5: Generando datos de ejemplo...");
            var dataGenerator = new SampleDataGenerator();
            var data = dataGenerator.GenerateData();
            
            Console.WriteLine($"  ✓ {data.Clientes.Count} clientes generados");
            Console.WriteLine($"  ✓ {data.Empleados.Count} empleados generados");
            Console.WriteLine($"  ✓ {data.Asignaciones.Count} asignaciones creadas");
            Console.WriteLine($"  ✓ {data.Vacaciones.Count} vacaciones registradas");
            Console.WriteLine($"  ✓ {data.Viajes.Count} viajes planificados");
            Console.WriteLine($"  ✓ {data.TurnosSoporte.Count} turnos de soporte programados");
            Console.WriteLine();
            
            // 2. Cargar feriados
            Console.WriteLine("📅 Paso 2/5: Cargando feriados de 2026...");
            var feriadoService = new FeriadoService();
            var feriados = feriadoService.ObtenerFeriados2026(data.Paises);
            Console.WriteLine($"  ✓ {feriados.Count} feriados cargados para {data.Paises.Count} países");
            
            foreach (var pais in data.Paises)
            {
                var feriadosPais = feriados.Count(f => f.Pais == pais);
                Console.WriteLine($"    • {pais}: {feriadosPais} feriados");
            }
            Console.WriteLine();
            
            // 3. Ejecutar validaciones
            Console.WriteLine("🔍 Paso 3/5: Ejecutando validaciones...");
            var validationService = new ValidationService();
            var alertas = validationService.ValidarTodo(data, feriados);
            
            Console.WriteLine($"  ✓ {alertas.Count} alertas detectadas:");
            Console.WriteLine($"    • {alertas.Count(a => a.Nivel == "Alta")} alertas de nivel ALTO");
            Console.WriteLine($"    • {alertas.Count(a => a.Nivel == "Media")} alertas de nivel MEDIO");
            Console.WriteLine($"    • {alertas.Count(a => a.Nivel == "Baja")} alertas de nivel BAJO");
            Console.WriteLine();
            
            // 4. Mostrar recomendaciones
            Console.WriteLine("💡 Paso 4/5: Analizando recomendaciones...");
            var alertaService = new AlertaService();
            var recomendaciones = alertaService.GenerarRecomendaciones(alertas);
            
            foreach (var recomendacion in recomendaciones)
            {
                Console.WriteLine($"  {recomendacion}");
            }
            Console.WriteLine();
            
            // 5. Generar Excel
            Console.WriteLine("📁 Paso 5/5: Generando archivo Excel...");
            var excelGenerator = new ExcelGeneratorService();
            var filePath = excelGenerator.GenerarExcel(data, feriados, alertas);
            
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✅ GENERACIÓN COMPLETADA EXITOSAMENTE                     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"📄 Archivo generado: {Path.GetFileName(filePath)}");
            Console.WriteLine($"📂 Ubicación: {Path.GetDirectoryName(filePath)}");
            Console.WriteLine();
            
            // Resumen final
            Console.WriteLine("📋 RESUMEN DEL ARCHIVO GENERADO:");
            Console.WriteLine("  • 11 hojas de trabajo completamente funcionales");
            Console.WriteLine("  • Dashboards interactivos con KPIs dinámicos");
            Console.WriteLine("  • Sistema de alertas COMPLETAMENTE DINÁMICO");
            Console.WriteLine("  • Detección de conflictos con fórmulas que se actualizan automáticamente");
            Console.WriteLine("  • 52 turnos de soporte para todo 2026");
            Console.WriteLine("  • Tablas con filtros y formato condicional");
            Console.WriteLine("  • Control completo de empleados y asignaciones");
            Console.WriteLine();
            
            Console.WriteLine("Presione cualquier tecla para abrir el archivo...");
            Console.ReadKey();
            
            // 6. Abrir archivo
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo abrir el archivo automáticamente: {ex.Message}");
                Console.WriteLine($"Por favor, abra manualmente el archivo: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ❌ ERROR DURANTE LA GENERACIÓN                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
