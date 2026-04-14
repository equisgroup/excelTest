using ExcelResourceManager.Core.Models;

namespace ExcelResourceManager.Reports;

public interface IExcelImportExportService
{
    // Export
    byte[] ExportarFeriados(IEnumerable<Feriado> feriados, IEnumerable<Ubicacion> ubicaciones);
    byte[] ExportarUbicaciones(IEnumerable<Ubicacion> ubicaciones);
    byte[] ExportarRoles(IEnumerable<Rol> roles);
    byte[] ExportarViajes(IEnumerable<Viaje> viajes, IEnumerable<Empleado> empleados, IEnumerable<Cliente> clientes, IEnumerable<Ubicacion> ubicaciones);
    byte[] ExportarVacaciones(IEnumerable<Vacacion> vacaciones, IEnumerable<Empleado> empleados);

    // Import
    Task<ImportResult> ImportarFeriadosAsync(Stream stream, IEnumerable<Ubicacion> ubicaciones, IEnumerable<Feriado> existentes, Func<Feriado, Task> insertar);
    Task<ImportResult> ImportarUbicacionesAsync(Stream stream, IEnumerable<Ubicacion> existentes, Func<Ubicacion, Task> insertar);
    Task<ImportResult> ImportarRolesAsync(Stream stream, IEnumerable<Rol> existentes, Func<Rol, Task> insertar);
    Task<ImportResult> ImportarViajesAsync(Stream stream, IEnumerable<Empleado> empleados, IEnumerable<Cliente> clientes, IEnumerable<Ubicacion> ubicaciones, Func<Viaje, Task> insertar);
    Task<ImportResult> ImportarVacacionesAsync(Stream stream, IEnumerable<Empleado> empleados, IEnumerable<Vacacion> existentes, Func<Vacacion, Task> insertar);

    // Export errors
    byte[] ExportarErroresImportacion(IEnumerable<ImportError> errores, string entidad);
}
