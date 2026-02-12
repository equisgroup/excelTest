# Excel Resource Manager - Sistema de Gestión de RRHH

## 🎯 NUEVA ARQUITECTURA: ASP.NET Core MVC (STABLE)

**Cambio Importante**: Reemplazamos Avalonia UI por ASP.NET Core MVC para eliminar problemas de threading y proporcionar una solución más estable.

### ✅ Por qué ASP.NET Core MVC?

- **Sin problemas de threading** - Manejo correcto de async/await
- **Acceso vía navegador** - No requiere instalación
- **Multiplataforma** - Windows, Linux, Mac
- **Visual Studio** - Soporte completo de debugging
- **Tecnología madura** - Millones de aplicaciones en producción
- **Código simple** - Fácil de mantener

## 🚀 Inicio Rápido

```bash
cd ExcelResourceManager.Web
dotnet run
```

Abrir navegador en: `https://localhost:5001`

## 🏗️ Arquitectura

```
ExcelResourceManager/
├── ExcelResourceManager.Web/          ← NUEVA aplicación web
│   ├── Controllers/                   - Controladores MVC
│   ├── Views/                         - Vistas Razor + Bootstrap
│   └── Program.cs                     - Configuración DI
│
├── ExcelResourceManager.Core/         - Lógica de negocio (SIN CAMBIOS)
├── ExcelResourceManager.Data/         - Repositorios LiteDB (SIN CAMBIOS)
├── ExcelResourceManager.Reports/      - Excel ClosedXML (SIN CAMBIOS)
└── ExcelResourceManager.Tests/        - Tests (SIN CAMBIOS)
```

**100% del código de negocio reutilizado** - Solo cambiamos la capa de presentación.

## 📊 Funcionalidades

- **Dashboard**: KPIs en tiempo real, próximas vacaciones
- **Empleados**: Gestión completa de personal
- **Clientes**: Administración por ubicación  
- **Vacaciones**: Detección automática de conflictos
- **Conflictos**: 6 tipos de validaciones (Crítico, Alto, Medio, Bajo)
- **Reportes**: Excel profesionales con ClosedXML

## 💾 Base de Datos LiteDB

- **Modo Test**: `database-test.db` con 84 registros de prueba
- **Modo Prod**: `database-prod.db` vacía
- Datos incluyen: 20 empleados, 15 vacaciones, 31 feriados 2026

## 🛠️ Stack Tecnológico

- ASP.NET Core 8.0 MVC
- LiteDB 5.0.19
- Bootstrap 5
- ClosedXML 0.102.3
- Serilog 3.1.1

## 📝 Configuración

Editar `ExcelResourceManager.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "TestDatabase": "Filename=database-test.db;Connection=shared",
    "ProdDatabase": "Filename=database-prod.db;Connection=shared"
  },
  "App": {
    "DefaultMode": "Test"
  }
}
```

## 🧪 Testing

```bash
dotnet test
```

## 📦 Despliegue Producción

```bash
dotnet publish -c Release
```

Compatible con IIS, Kestrel, nginx, Docker.

---

**Estado**: ✅ PRODUCCIÓN READY - Sin threading issues

Ver `/ExcelResourceManager.Web/README.md` para más detalles.

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
