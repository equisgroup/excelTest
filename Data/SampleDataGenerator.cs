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
            new Cliente { Id = 1, Nombre = "TechCorp USA", Pais = "US", Ciudad = "New York", Email = "contact@techcorp.com", Telefono = "+1-555-0101", FechaRegistro = new DateTime(2024, 1, 15), Activo = true },
            new Cliente { Id = 2, Nombre = "Innovación México", Pais = "MX", Ciudad = "Ciudad de México", Email = "info@innovacion.mx", Telefono = "+52-55-1234", FechaRegistro = new DateTime(2024, 3, 20), Activo = true },
            new Cliente { Id = 3, Nombre = "España Digital", Pais = "ES", Ciudad = "Madrid", Email = "contacto@espanadigital.es", Telefono = "+34-91-123-456", FechaRegistro = new DateTime(2024, 2, 10), Activo = true },
            new Cliente { Id = 4, Nombre = "Argentina Tech", Pais = "AR", Ciudad = "Buenos Aires", Email = "info@argentinatech.ar", Telefono = "+54-11-4567", FechaRegistro = new DateTime(2024, 4, 5), Activo = true },
            new Cliente { Id = 5, Nombre = "Brasil Solutions", Pais = "BR", Ciudad = "São Paulo", Email = "contato@brasilsolutions.br", Telefono = "+55-11-9876", FechaRegistro = new DateTime(2024, 5, 12), Activo = true },
            new Cliente { Id = 6, Nombre = "California Ventures", Pais = "US", Ciudad = "San Francisco", Email = "hello@calventures.com", Telefono = "+1-415-5555", FechaRegistro = new DateTime(2025, 1, 8), Activo = true },
            new Cliente { Id = 7, Nombre = "Monterrey Industries", Pais = "MX", Ciudad = "Monterrey", Email = "contacto@mtyind.mx", Telefono = "+52-81-8888", FechaRegistro = new DateTime(2025, 3, 15), Activo = true },
            new Cliente { Id = 8, Nombre = "Barcelona Innovation", Pais = "ES", Ciudad = "Barcelona", Email = "info@bcninnovation.es", Telefono = "+34-93-999-888", FechaRegistro = new DateTime(2025, 2, 20), Activo = false },
        };
        
        return clientes;
    }
    
    private List<Empleado> GenerarEmpleados(List<Cliente> clientes)
    {
        var empleados = new List<Empleado>
        {
            new Empleado { Id = 1, Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@empresa.com", Telefono = "+1-555-1001", Pais = "US", Ciudad = "New York", ClienteAsignadoId = 1, FechaIngreso = new DateTime(2023, 1, 10), Activo = true },
            new Empleado { Id = 2, Nombre = "María", Apellido = "García", Email = "maria.garcia@empresa.com", Telefono = "+52-55-2001", Pais = "MX", Ciudad = "Ciudad de México", ClienteAsignadoId = 2, FechaIngreso = new DateTime(2023, 2, 15), Activo = true },
            new Empleado { Id = 3, Nombre = "Carlos", Apellido = "López", Email = "carlos.lopez@empresa.com", Telefono = "+34-91-3001", Pais = "ES", Ciudad = "Madrid", ClienteAsignadoId = 3, FechaIngreso = new DateTime(2023, 3, 20), Activo = true },
            new Empleado { Id = 4, Nombre = "Ana", Apellido = "Martínez", Email = "ana.martinez@empresa.com", Telefono = "+54-11-4001", Pais = "AR", Ciudad = "Buenos Aires", ClienteAsignadoId = 4, FechaIngreso = new DateTime(2023, 4, 5), Activo = true },
            new Empleado { Id = 5, Nombre = "Pedro", Apellido = "Rodríguez", Email = "pedro.rodriguez@empresa.com", Telefono = "+55-11-5001", Pais = "BR", Ciudad = "São Paulo", ClienteAsignadoId = 5, FechaIngreso = new DateTime(2023, 5, 10), Activo = true },
            new Empleado { Id = 6, Nombre = "Laura", Apellido = "Fernández", Email = "laura.fernandez@empresa.com", Telefono = "+1-415-6001", Pais = "US", Ciudad = "San Francisco", ClienteAsignadoId = 6, FechaIngreso = new DateTime(2023, 6, 15), Activo = true },
            new Empleado { Id = 7, Nombre = "Diego", Apellido = "Sánchez", Email = "diego.sanchez@empresa.com", Telefono = "+52-81-7001", Pais = "MX", Ciudad = "Monterrey", ClienteAsignadoId = 7, FechaIngreso = new DateTime(2023, 7, 20), Activo = true },
            new Empleado { Id = 8, Nombre = "Sofia", Apellido = "Torres", Email = "sofia.torres@empresa.com", Telefono = "+34-93-8001", Pais = "ES", Ciudad = "Barcelona", ClienteAsignadoId = null, FechaIngreso = new DateTime(2023, 8, 25), Activo = true },
            new Empleado { Id = 9, Nombre = "Miguel", Apellido = "Ramírez", Email = "miguel.ramirez@empresa.com", Telefono = "+54-11-9001", Pais = "AR", Ciudad = "Córdoba", ClienteAsignadoId = null, FechaIngreso = new DateTime(2023, 9, 30), Activo = true },
            new Empleado { Id = 10, Nombre = "Lucía", Apellido = "Flores", Email = "lucia.flores@empresa.com", Telefono = "+55-11-1002", Pais = "BR", Ciudad = "Rio de Janeiro", ClienteAsignadoId = null, FechaIngreso = new DateTime(2023, 10, 5), Activo = true },
            new Empleado { Id = 11, Nombre = "Roberto", Apellido = "Morales", Email = "roberto.morales@empresa.com", Telefono = "+1-555-1103", Pais = "US", Ciudad = "Chicago", ClienteAsignadoId = 1, FechaIngreso = new DateTime(2024, 1, 10), Activo = true },
            new Empleado { Id = 12, Nombre = "Carmen", Apellido = "Jiménez", Email = "carmen.jimenez@empresa.com", Telefono = "+52-55-1204", Pais = "MX", Ciudad = "Guadalajara", ClienteAsignadoId = 2, FechaIngreso = new DateTime(2024, 2, 14), Activo = true },
            new Empleado { Id = 13, Nombre = "Javier", Apellido = "Ruiz", Email = "javier.ruiz@empresa.com", Telefono = "+34-91-1305", Pais = "ES", Ciudad = "Valencia", ClienteAsignadoId = 3, FechaIngreso = new DateTime(2024, 3, 18), Activo = true },
            new Empleado { Id = 14, Nombre = "Valentina", Apellido = "Castro", Email = "valentina.castro@empresa.com", Telefono = "+54-11-1406", Pais = "AR", Ciudad = "Rosario", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 4, 22), Activo = true },
            new Empleado { Id = 15, Nombre = "Fernando", Apellido = "Silva", Email = "fernando.silva@empresa.com", Telefono = "+55-21-1507", Pais = "BR", Ciudad = "Brasília", ClienteAsignadoId = 5, FechaIngreso = new DateTime(2024, 5, 26), Activo = true },
            new Empleado { Id = 16, Nombre = "Isabel", Apellido = "Ortiz", Email = "isabel.ortiz@empresa.com", Telefono = "+1-415-1608", Pais = "US", Ciudad = "Seattle", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 6, 30), Activo = true },
            new Empleado { Id = 17, Nombre = "Ricardo", Apellido = "Vargas", Email = "ricardo.vargas@empresa.com", Telefono = "+52-81-1709", Pais = "MX", Ciudad = "Tijuana", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 7, 4), Activo = true },
            new Empleado { Id = 18, Nombre = "Patricia", Apellido = "Herrera", Email = "patricia.herrera@empresa.com", Telefono = "+34-93-1810", Pais = "ES", Ciudad = "Sevilla", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 8, 8), Activo = true },
            new Empleado { Id = 19, Nombre = "Andrés", Apellido = "Mendoza", Email = "andres.mendoza@empresa.com", Telefono = "+54-11-1911", Pais = "AR", Ciudad = "Mendoza", ClienteAsignadoId = 4, FechaIngreso = new DateTime(2024, 9, 12), Activo = true },
            new Empleado { Id = 20, Nombre = "Gabriela", Apellido = "Costa", Email = "gabriela.costa@empresa.com", Telefono = "+55-11-2012", Pais = "BR", Ciudad = "Curitiba", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 10, 16), Activo = true },
            new Empleado { Id = 21, Nombre = "Daniel", Apellido = "Reyes", Email = "daniel.reyes@empresa.com", Telefono = "+1-555-2113", Pais = "US", Ciudad = "Boston", ClienteAsignadoId = 6, FechaIngreso = new DateTime(2024, 11, 20), Activo = true },
            new Empleado { Id = 22, Nombre = "Natalia", Apellido = "Romero", Email = "natalia.romero@empresa.com", Telefono = "+52-55-2214", Pais = "MX", Ciudad = "Puebla", ClienteAsignadoId = null, FechaIngreso = new DateTime(2024, 12, 24), Activo = true },
            new Empleado { Id = 23, Nombre = "Alberto", Apellido = "Navarro", Email = "alberto.navarro@empresa.com", Telefono = "+34-91-2315", Pais = "ES", Ciudad = "Bilbao", ClienteAsignadoId = null, FechaIngreso = new DateTime(2025, 1, 28), Activo = true },
            new Empleado { Id = 24, Nombre = "Camila", Apellido = "Medina", Email = "camila.medina@empresa.com", Telefono = "+54-11-2416", Pais = "AR", Ciudad = "La Plata", ClienteAsignadoId = null, FechaIngreso = new DateTime(2025, 2, 1), Activo = true },
            new Empleado { Id = 25, Nombre = "Raúl", Apellido = "Santos", Email = "raul.santos@empresa.com", Telefono = "+55-11-2517", Pais = "BR", Ciudad = "Salvador", ClienteAsignadoId = null, FechaIngreso = new DateTime(2025, 3, 7), Activo = true },
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
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 8, ClienteId = 3, FechaInicio = new DateTime(2024, 1, 1), FechaFin = new DateTime(2024, 6, 30), Activa = false, Observaciones = "Proyecto completado" });
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 9, ClienteId = 4, FechaInicio = new DateTime(2024, 2, 1), FechaFin = new DateTime(2024, 8, 31), Activa = false, Observaciones = "Finalizado" });
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 10, ClienteId = 5, FechaInicio = new DateTime(2024, 3, 1), FechaFin = new DateTime(2024, 9, 30), Activa = false, Observaciones = "Contrato terminado" });
        
        // Conflicto intencional: asignación múltiple
        asignaciones.Add(new Asignacion { Id = id++, EmpleadoId = 1, ClienteId = 6, FechaInicio = new DateTime(2026, 1, 1), FechaFin = null, Activa = true, Observaciones = "Segunda asignación (conflicto)" });
        
        return asignaciones;
    }
    
    private List<Vacacion> GenerarVacaciones(List<Empleado> empleados)
    {
        var vacaciones = new List<Vacacion>
        {
            // Vacaciones normales
            new Vacacion { Id = 1, EmpleadoId = 1, FechaInicio = new DateTime(2026, 3, 10), FechaFin = new DateTime(2026, 3, 20), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 2, EmpleadoId = 2, FechaInicio = new DateTime(2026, 4, 15), FechaFin = new DateTime(2026, 4, 25), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 3, EmpleadoId = 3, FechaInicio = new DateTime(2026, 5, 1), FechaFin = new DateTime(2026, 5, 10), Estado = "Pendiente", Observaciones = "" },
            new Vacacion { Id = 4, EmpleadoId = 4, FechaInicio = new DateTime(2026, 6, 5), FechaFin = new DateTime(2026, 6, 15), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 5, EmpleadoId = 5, FechaInicio = new DateTime(2026, 7, 20), FechaFin = new DateTime(2026, 7, 30), Estado = "Aprobada", Observaciones = "" },
            
            // Conflicto intencional: vacaciones que coinciden con viaje (empleado 6)
            new Vacacion { Id = 6, EmpleadoId = 6, FechaInicio = new DateTime(2026, 8, 10), FechaFin = new DateTime(2026, 8, 20), Estado = "Aprobada", Observaciones = "Conflicto con viaje" },
            
            // Conflicto intencional: vacaciones durante turno de soporte (empleado 7)
            new Vacacion { Id = 7, EmpleadoId = 7, FechaInicio = new DateTime(2026, 2, 9), FechaFin = new DateTime(2026, 2, 16), Estado = "Pendiente", Observaciones = "Conflicto con soporte" },
            
            new Vacacion { Id = 8, EmpleadoId = 8, FechaInicio = new DateTime(2026, 9, 1), FechaFin = new DateTime(2026, 9, 10), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 9, EmpleadoId = 9, FechaInicio = new DateTime(2026, 10, 5), FechaFin = new DateTime(2026, 10, 15), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 10, EmpleadoId = 10, FechaInicio = new DateTime(2026, 11, 10), FechaFin = new DateTime(2026, 11, 20), Estado = "Pendiente", Observaciones = "" },
            new Vacacion { Id = 11, EmpleadoId = 11, FechaInicio = new DateTime(2026, 12, 20), FechaFin = new DateTime(2026, 12, 30), Estado = "Aprobada", Observaciones = "" },
            
            // Vacaciones que incluyen feriados
            new Vacacion { Id = 12, EmpleadoId = 12, FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 1, 10), Estado = "Aprobada", Observaciones = "Incluye feriados" },
            new Vacacion { Id = 13, EmpleadoId = 13, FechaInicio = new DateTime(2026, 7, 4), FechaFin = new DateTime(2026, 7, 14), Estado = "Aprobada", Observaciones = "" },
            new Vacacion { Id = 14, EmpleadoId = 14, FechaInicio = new DateTime(2026, 3, 25), FechaFin = new DateTime(2026, 4, 5), Estado = "Pendiente", Observaciones = "" },
            new Vacacion { Id = 15, EmpleadoId = 15, FechaInicio = new DateTime(2026, 5, 15), FechaFin = new DateTime(2026, 5, 25), Estado = "Aprobada", Observaciones = "" },
        };
        
        return vacaciones;
    }
    
    private List<Viaje> GenerarViajes(List<Empleado> empleados, List<Cliente> clientes)
    {
        var viajes = new List<Viaje>
        {
            new Viaje { Id = 1, EmpleadoId = 1, ClienteId = 1, PaisDestino = "US", CiudadDestino = "New York", FechaInicio = new DateTime(2026, 2, 10), FechaFin = new DateTime(2026, 2, 15), Motivo = "Reunión con cliente", Estado = "Planificado" },
            new Viaje { Id = 2, EmpleadoId = 2, ClienteId = 2, PaisDestino = "MX", CiudadDestino = "Ciudad de México", FechaInicio = new DateTime(2026, 3, 5), FechaFin = new DateTime(2026, 3, 10), Motivo = "Implementación de proyecto", Estado = "Planificado" },
            new Viaje { Id = 3, EmpleadoId = 3, ClienteId = 3, PaisDestino = "ES", CiudadDestino = "Barcelona", FechaInicio = new DateTime(2026, 4, 20), FechaFin = new DateTime(2026, 4, 25), Motivo = "Capacitación", Estado = "Planificado" },
            new Viaje { Id = 4, EmpleadoId = 4, ClienteId = 4, PaisDestino = "AR", CiudadDestino = "Buenos Aires", FechaInicio = new DateTime(2026, 5, 15), FechaFin = new DateTime(2026, 5, 20), Motivo = "Soporte técnico", Estado = "Planificado" },
            new Viaje { Id = 5, EmpleadoId = 5, ClienteId = 5, PaisDestino = "BR", CiudadDestino = "Rio de Janeiro", FechaInicio = new DateTime(2026, 6, 10), FechaFin = new DateTime(2026, 6, 15), Motivo = "Revisión de infraestructura", Estado = "Planificado" },
            
            // Conflicto intencional: viaje que coincide con vacaciones (empleado 6)
            new Viaje { Id = 6, EmpleadoId = 6, ClienteId = 6, PaisDestino = "US", CiudadDestino = "San Francisco", FechaInicio = new DateTime(2026, 8, 12), FechaFin = new DateTime(2026, 8, 18), Motivo = "Workshop", Estado = "Planificado" },
            
            // Viaje durante turno de soporte (empleado 7)
            new Viaje { Id = 7, EmpleadoId = 7, ClienteId = 7, PaisDestino = "MX", CiudadDestino = "Monterrey", FechaInicio = new DateTime(2026, 2, 11), FechaFin = new DateTime(2026, 2, 14), Motivo = "Visita cliente", Estado = "Planificado" },
            
            // Viaje en feriado (1 de mayo - Día del Trabajo en varios países)
            new Viaje { Id = 8, EmpleadoId = 8, ClienteId = 3, PaisDestino = "ES", CiudadDestino = "Madrid", FechaInicio = new DateTime(2026, 5, 1), FechaFin = new DateTime(2026, 5, 5), Motivo = "Reunión estratégica", Estado = "Planificado" },
            
            // Viaje en feriado USA (4 de julio)
            new Viaje { Id = 9, EmpleadoId = 11, ClienteId = 1, PaisDestino = "US", CiudadDestino = "New York", FechaInicio = new DateTime(2026, 7, 4), FechaFin = new DateTime(2026, 7, 8), Motivo = "Presentación producto", Estado = "Planificado" },
            
            new Viaje { Id = 10, EmpleadoId = 12, ClienteId = 2, PaisDestino = "MX", CiudadDestino = "Guadalajara", FechaInicio = new DateTime(2026, 9, 15), FechaFin = new DateTime(2026, 9, 20), Motivo = "Auditoría", Estado = "Planificado" },
        };
        
        return viajes;
    }
    
    private List<TurnoSoporte> GenerarTurnosSoporte(List<Empleado> empleados)
    {
        var turnos = new List<TurnoSoporte>();
        int id = 1;
        
        // Generar turnos semanales para 2026 (primera mitad del año)
        var fechaInicio = new DateTime(2026, 1, 5); // Primera semana de 2026
        var empleadosRotacion = empleados.Take(10).ToList(); // Usar los primeros 10 empleados para rotación
        
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
