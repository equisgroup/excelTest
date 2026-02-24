namespace ExcelResourceManager.Core.Enums;

public enum TipoConflicto
{
    // Conflictos de solapamiento temporal
    VacacionVsViaje,
    VacacionVsSoporte,
    ViajeVsSoporte,
    ViajeEnFeriado,
    VacacionEnFeriado,
    
    // Conflictos de asignación de roles
    RolNoAsignado,              // Rol requerido por cliente pero no hay asignación
    RolSinCobertura,            // Rol sin cobertura por vacaciones de empleado
    CoberturaSuperaContratada,  // Más empleados asignados que los contratados
    AsignacionFueraContrato     // Asignación fuera del período de contrato
}
