using ExcelDashboardGenerator.Models;
using System.Globalization;

namespace ExcelDashboardGenerator.Data;

public class SampleDataGenerator
{
    private readonly Random _random = new();
    
    public DataContainer GenerateData()
    {
        var data = new DataContainer();
        
        // Generar clientes
        data.Clientes = GenerarClientes();
        data.Paises = data.Clientes.Select(c => c.Pais).Distinct().ToList();
        
        // Generar empleados
        data.Empleados = GenerarEmpleados(data.Clientes);
        
        // Generar asignaciones
        data.Asignaciones = GenerarAsignaciones(data.Empleados, data.Clientes);
        
        // Generar vacaciones (con algunos conflictos intencionales)
        data.Vacaciones = GenerarVacaciones(data.Empleados);
        
        // Generar viajes
        data.Viajes = GenerarViajes(data.Empleados, data.Clientes);
        
        // Generar turnos de soporte
        data.TurnosSoporte = GenerarTurnosSoporte(data.Empleados);
        
        return data;
    }
    
    private List<Cliente> GenerarClientes()
    {
        var clientes = new List<Cliente>
        {
            new Cliente { Id = 1, Nombre = "Quito Tech Solutions", Pais = "EC", Ciudad = "Quito", Email = "contacto@quitotech.ec", Telefono = "+593-2-2501234", FechaRegistro = new DateTime(2024, 1, 15), Activo = true },
            new Cliente { Id = 2, Nombre = "Guayaquil Innovation Hub", Pais = "EC", Ciudad = "Guayaquil", Email = "info@guayaquilhub.ec", Telefono = "+593-4-2301234", FechaRegistro = new DateTime(2024, 3, 20), Activo = true },
            new Cliente { Id = 3, Nombre = "Asunción Digital", Pais = "PY", Ciudad = "Asunción", Email = "contacto@asunciondigital.py", Telefono = "+595-21-123456", FechaRegistro = new DateTime(2024, 2, 10), Activo = true },
        };
        
        return clientes;
    }
    
    private List<Empleado> GenerarEmpleados(List<Cliente> clientes)
    {
        var empleados = new List<Empleado>
        {
            new Empleado { Id = 1, Nombre = "Carlos", Apellido = "Morales", Email = "carlos.morales@empresa.com", Telefono = "+593-2-2501001", Pais = "EC", Ciudad = "Quito", ClienteAsignadoId = 1, FechaIngreso = new DateTime(2023, 1, 10), Activo = true },
            new Empleado { Id = 2, Nombre = "María", Apellido = "Jiménez", Email = "maria.jimenez@empresa.com", Telefono = "+593-2-2501002", Pais = "EC", Ciudad = "Quito", ClienteAsignadoId = 2, FechaIngreso = new DateTime(2023, 3, 15), Activo = true },
            new Empleado { Id = 3, Nombre = "Diego", Apellido = "Santana", Email = "diego.santana@empresa.com", Telefono = "+593-4-2301001", Pais = "EC", Ciudad = "Guayaquil", ClienteAsignadoId = 3, FechaIngreso = new DateTime(2023, 6, 20), Activo = true },
        };
        
        return empleados;
    }
    
    private List<Asignacion> GenerarAsignaciones(List<Empleado> empleados, List<Cliente> clientes)
    {
        var asignaciones = new List<Asignacion>();
        int id = 1;
        
        // Asignaciones activas para empleados con cliente asignado
        foreach (var emp in empleados.Where(e => e.ClienteAsignadoId.HasValue))
        {
            asignaciones.Add(new Asignacion
            {
                Id = id++,
                EmpleadoId = emp.Id,
                ClienteId = emp.ClienteAsignadoId.Value,
                FechaInicio = emp.FechaIngreso.AddMonths(1),
                FechaFin = null,
                Activa = true,
                Observaciones = "Asignación actual"
            });
        }
        
        // Algunas asignaciones históricas
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 1, ClienteId = 2, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2024, 6, 30), Activa = false, Observaciones = "Proyecto completado" });
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 2, ClienteId = 3, FechaInicio = new DateTime(2024, 2, 1), FechaFin = new DateTime(2024, 8, 31), Activa = false, Observaciones = "Finalizado" });
        
        return asignaciones;
    }
    
    private List<Vacacion> GenerarVacaciones(List<Empleado> empleados)
    {
        var vacaciones = new List<Vacacion>
        {
            // Vacaciones para empleados
            new Vacacion { Id = 1, EmpleadoId = 1, FechaInicio = new DateTime(2026, 3, 10), FechaFin = new DateTime(2026, 3, 20), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 2, EmpleadoId = 2, FechaInicio = new DateTime(2026, 4, 15), FechaFin = new DateTime(2026, 4, 25), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 3, EmpleadoId = 3, FechaInicio = new DateTime(2026, 5, 1), FechaFin = new DateTime(2026, 5, 10), Estado = "Pendiente", Observaciones = "" },
            
            // Conflicto intencional: vacaciones durante turno de soporte (empleado 1)
            new Vacacion { Id = 4, EmpleadoId = 1, FechaInicio = new DateTime(2026, 2, 9), FechaFin = new DateTime(2026, 2, 16), Estado = "Pendiente", Observaciones = "Conflicto con soporte" },
            
            // Vacaciones que incluyen feriados
            new Vacacion { Id = 5, EmpleadoId = 2, FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 1, 10), Estado = "Aprobada", Observaciones = "Incluye feriados" },
        };
        
        return vacaciones;
    }
    
    private List<Viaje> GenerarViajes(List<Empleado> empleados, List<Cliente> clientes)
    {
        var viajes = new List<Viaje>
        {
            new Viaje { Id = 1, EmpleadoId = 1, ClienteId = 1, PaisDestino = "EC", CiudadDestino = "Quito", FechaInicio = new DateTime(2026, 2, 10), FechaFin = new DateTime(2026, 2, 15), Motivo = "Reunión con cliente", Estado = "Planificado" },
            new Viaje { Id = 2, EmpleadoId = 2, ClienteId = 2, PaisDestino = "EC", CiudadDestino = "Guayaquil", FechaInicio = new DateTime(2026, 3, 5), FechaFin = new DateTime(2026, 3, 10), Motivo = "Implementación de proyecto", Estado = "Planificado" },
            new Viaje { Id = 3, EmpleadoId = 3, ClienteId = 3, PaisDestino = "PY", CiudadDestino = "Asunción", FechaInicio = new DateTime(2026, 4, 20), FechaFin = new DateTime(2026, 4, 25), Motivo = "Capacitación", Estado = "Planificado" },
            
            // Viaje durante turno de soporte (empleado 1)
            new Viaje { Id = 4, EmpleadoId = 1, ClienteId = 1, PaisDestino = "EC", CiudadDestino = "Guayaquil", FechaInicio = new DateTime(2026, 2, 11), FechaFin = new DateTime(2026, 2, 14), Motivo = "Visita cliente", Estado = "Planificado" },
            
            // Viaje en feriado (1 de mayo - Día del Trabajo)
            new Viaje { Id = 5, EmpleadoId = 2, ClienteId = 2, PaisDestino = "EC", CiudadDestino = "Guayaquil", FechaInicio = new DateTime(2026, 5, 1), FechaFin = new DateTime(2026, 5, 5), Motivo = "Reunión estratégica", Estado = "Planificado" },
        };
        
        return viajes;
    }
    
    private List<TurnoSoporte> GenerarTurnosSoporte(List<Empleado> empleados)
    {
        var turnos = new List<TurnoSoporte>();
        int id = 1;
        
        // Generar turnos semanales para 2026 (primera mitad del año)
        var fechaInicio = new DateTime(2026, 1, 5); // Primera semana de 2026
        var empleadosRotacion = empleados.ToList(); // Usar todos los empleados para rotación
        
        for (int semana = 1; semana <= 26; semana++)
        {
            var empleadoIndex = (semana - 1) % empleadosRotacion.Count;
            var empleado = empleadosRotacion[empleadoIndex];
            
            var inicioSemana = fechaInicio.AddDays((semana - 1) * 7);
            var finSemana = inicioSemana.AddDays(6);
            
            turnos.Add(new TurnoSoporte
            {
                Id = id++,
                EmpleadoId = empleado.Id,
                FechaInicio = inicioSemana,
                FechaFin = finSemana,
                NumeroSemana = semana,
                Año = 2026,
                Observaciones = $"Turno rotativo semana {semana}"
            });
        }
        
        return turnos;
    }
}
