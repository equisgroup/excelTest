# ExcelResourceManager

Sistema de gestión de recursos humanos completo desarrollado con Avalonia UI 11.1, ReactiveUI y LiteDB para gestión de recursos humanos en Ecuador y Paraguay, con detección automática de conflictos y generación de reportes Excel.

## 📋 Características Principales

- ✅ **Gestión de Empleados y Clientes** - CRUD completo con asignaciones
- ✅ **Gestión de Vacaciones** - Con validación reactiva en tiempo real
- ✅ **Gestión de Viajes** - Planificación y seguimiento
- ✅ **Turnos de Soporte** - Calendario rotativo
- ✅ **Detección Automática de Conflictos** - 6 tipos de validaciones
- ✅ **Feriados 2026** - Ecuador y Paraguay precargados
- ✅ **Reportes Excel** - Generación con ClosedXML
- ✅ **Modo Test/Producción** - Toggle en tiempo real
- ✅ **Interfaz Moderna** - Avalonia UI con Fluent Design

## 🏗️ Arquitectura

```
ExcelResourceManager/
├── ExcelResourceManager.Desktop    # Aplicación Avalonia UI
│   ├── Views/                      # Vistas AXAML
│   ├── ViewModels/                 # ViewModels ReactiveUI
│   ├── Converters/                 # Value Converters
│   └── Styles/                     # Estilos personalizados
├── ExcelResourceManager.Core       # Lógica de negocio
│   ├── Models/                     # Modelos de dominio
│   ├── Enums/                      # Enumeraciones
│   └── Services/                   # Servicios e interfaces
├── ExcelResourceManager.Data       # Acceso a datos
│   └── Repositories/               # Patrón Repository + UnitOfWork
├── ExcelResourceManager.Reports    # Generación de reportes
│   └── Generators/                 # Generadores Excel
└── ExcelResourceManager.Tests      # Pruebas unitarias
```

## 🛠️ Stack Tecnológico

- **.NET 8.0**
- **Avalonia UI 11.1.3** - Framework UI multiplataforma
- **ReactiveUI** - MVVM reactivo
- **LiteDB 5.0.19** - Base de datos NoSQL embebida
- **ClosedXML 0.102.3** - Generación de reportes Excel
- **Serilog 3.1.1** - Logging estructurado
- **xUnit, Moq, FluentAssertions** - Testing

## 📦 Instalación

### Prerrequisitos
- .NET 8.0 SDK o superior
- Windows, Linux o macOS

### Clonar y Compilar

\`\`\`bash
# Clonar repositorio
git clone https://github.com/equisgroup/excelTest.git
cd excelTest

# Restaurar paquetes
dotnet restore

# Compilar solución
dotnet build

# Ejecutar aplicación
cd ExcelResourceManager.Desktop
dotnet run
\`\`\`

## 🚀 Uso

### Modo Test vs Producción

La aplicación inicia en **Modo Test** con datos de prueba precargados:
- 3 ubicaciones (Guayaquil, Quito, Asunción)
- 20 empleados con roles variados
- 3 clientes
- 15 vacaciones con conflictos intencionados
- 10 viajes
- 10 turnos de soporte
- Feriados 2026 completos

**Cambiar de modo:**
- Toggle switch en la esquina superior derecha
- Modo Producción inicia vacío (solo ubicaciones y feriados)

## 📊 Módulos

### 1. Dashboard
- KPIs principales (empleados, clientes, conflictos)
- Próximas vacaciones (30 días)
- Vista general del sistema

### 2. Empleados
- CRUD completo de empleados (placeholder)
- Asignación a clientes con porcentajes
- Historial de vacaciones y viajes

### 3. Clientes
- Gestión de clientes (placeholder)
- Ubicaciones y códigos internos
- Empleados asignados

### 4. Vacaciones
- **Solicitud de vacaciones con validación reactiva en tiempo real**
- Cálculo automático de días hábiles
- Detección de conflictos con viajes y turnos de soporte
- Estados: Solicitada, Aprobada, Rechazada, Cancelada

### 5. Viajes
- Planificación de viajes a clientes (placeholder)
- Detección de conflictos
- Estados: Planificado, Confirmado, En Curso, Completado, Cancelado

### 6. Turnos de Soporte
- Calendario de turnos rotativos (placeholder)
- Asignación por semana
- Detección de solapamientos

### 7. Feriados
- Vista por ubicación Ecuador/Paraguay (placeholder)
- Feriados nacionales y locales 2026

### 8. Conflictos
- Lista filtrable por nivel (placeholder)
- Resolución manual
- Exportación a Excel

### 9. Reportes
- **Reporte de Conflictos** - 3 hojas (Resumen, Detallado, Por Empleado)
- **Dashboard Gerencial** - KPIs y resúmenes
- Guardado en carpeta \`Reportes/\`

## ⚠️ Validaciones y Conflictos

El sistema detecta automáticamente **6 tipos de conflictos**:

| Tipo | Nivel | Descripción |
|------|-------|-------------|
| Vacación vs Viaje | **CRÍTICO** | Mismo empleado en vacación y viaje simultáneamente |
| Vacación vs Soporte | **CRÍTICO** | Empleado en vacación asignado a turno de soporte |
| Viaje vs Soporte | **MEDIO** | Empleado en viaje con turno de soporte (puede hacer remoto) |
| Viaje en Feriado | **BAJO** | Viaje programado durante feriado en destino |
| Vacación en Feriado | **BAJO** | Vacación incluye días feriados (informativo) |
| Sobreasignación | **ALTO** | Empleado asignado >100% a clientes |

### Códigos de Color

- 🔴 **Rojo** - Crítico (requiere acción inmediata)
- 🟠 **Naranja** - Alto (debe revisarse pronto)
- 🟡 **Amarillo** - Medio (planificar resolución)
- 🔵 **Azul** - Bajo (informativo)
- ⚪ **Gris** - Informativo (sin acción requerida)

## 📈 Reportes Excel

### Reporte de Conflictos
Genera archivo Excel con formato profesional:

**Hoja 1 - Resumen:**
- Total de conflictos por nivel
- Tabla resumen agrupada por tipo

**Hoja 2 - Lista Detallada:**
- Tabla completa con formato condicional por nivel
- Filtros automáticos
- Columnas ajustadas automáticamente

**Hoja 3 - Por Empleado:**
- Agrupación por empleado
- Subtotales

## ⚙️ Configuración

### appsettings.json

\`\`\`json
{
  "ConnectionStrings": {
    "TestDatabase": "Filename=database-test.db;Connection=shared",
    "ProdDatabase": "Filename=database-prod.db;Connection=shared"
  },
  "App": {
    "DefaultMode": "Test",
    "ReportsOutputDirectory": "./Reportes"
  }
}
\`\`\`

## 🧪 Tests

\`\`\`bash
# Ejecutar todos los tests
dotnet test
\`\`\`

## 🐛 Troubleshooting

### La aplicación no inicia
- Verificar que .NET 8.0 SDK está instalado: \`dotnet --version\`
- Restaurar paquetes: \`dotnet restore\`
- Limpiar y recompilar: \`dotnet clean && dotnet build\`

### No se cargan datos de prueba
- Eliminar archivo \`database-test.db\` y reiniciar
- Verificar logs en carpeta \`logs/\`

### Error al generar reportes
- Verificar que carpeta \`Reportes/\` tiene permisos de escritura

---

**Versión:** 1.0.0  
**Última actualización:** Febrero 2026
