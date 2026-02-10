# Excel Dashboard Generator

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

Generador de archivos Excel con Dashboard Gerencial interactivo y Sistema de Control completo de Asignaciones de Empleados, Clientes, Vacaciones, Viajes y Turnos de Soporte.

## 📋 Descripción

Este proyecto es una solución .NET 8.0 que genera automáticamente archivos Excel profesionales con:
- Dashboards interactivos con KPIs
- Sistema de alertas automáticas
- Control integral de empleados y clientes
- Gestión de asignaciones, vacaciones y viajes
- Turnos de soporte rotativos
- Integración con feriados internacionales
- Formato condicional y tablas con filtros
- Validaciones cruzadas de conflictos

## 🎯 Características Principales

### Sistema de Alertas Automáticas
- ⚠️ **Nivel Alto**: Conflictos críticos (vacaciones vs viajes, vacaciones vs soporte, asignaciones múltiples)
- ⚡ **Nivel Medio**: Situaciones a revisar (viajes durante soporte)
- ℹ️ **Nivel Bajo**: Alertas informativas (viajes/vacaciones en feriados)

### Gestión de Feriados
- Integración con librería **Nager.Date**
- Feriados de múltiples países (USA, México, España, Argentina, Brasil)
- Detección automática de conflictos con feriados
- Clasificación por país y fecha

### Hojas de Trabajo Generadas
1. **📊 Dashboard Gerencial** - KPIs y métricas principales
2. **🚨 Alertas** - Sistema de alertas y conflictos
3. **👥 Clientes** - Listado completo de clientes
4. **👨‍💼 Empleados** - Listado completo de empleados
5. **🔄 Asignaciones** - Historial de asignaciones
6. **🏖️ Vacaciones** - Registro de vacaciones
7. **✈️ Viajes** - Registro de viajes
8. **🛠️ Turnos Soporte** - Turnos semanales rotativos
9. **📅 Feriados** - Catálogo de feriados por país
10. **📊 Dashboard Ocupación** - Vista de ocupación de empleados
11. **ℹ️ Instrucciones** - Guía de uso completa

## 🚀 Requisitos Previos

- **.NET 8.0 SDK** o superior
- Sistema operativo: Windows, Linux o macOS
- Microsoft Excel (para abrir el archivo generado) o LibreOffice Calc

## 📦 Dependencias (NuGet Packages)

```xml
<PackageReference Include="ClosedXML" Version="0.102.3" />
<PackageReference Include="DocumentFormat.OpenXml" Version="3.0.2" />
<PackageReference Include="Nager.Date" Version="1.30.0" />
```

## 🔧 Instalación

1. **Clonar el repositorio:**
```bash
git clone https://github.com/equisgroup/excelTest.git
cd excelTest
```

2. **Restaurar dependencias:**
```bash
dotnet restore
```

3. **Compilar el proyecto:**
```bash
dotnet build
```

## ▶️ Ejecución

Ejecutar el proyecto:
```bash
dotnet run
```

El programa:
1. Genera datos de ejemplo automáticamente
2. Carga feriados de 2026 para todos los países
3. Ejecuta validaciones cruzadas
4. Genera el archivo Excel
5. Muestra un resumen en consola
6. Abre el archivo automáticamente (si es posible)

## 📂 Estructura del Proyecto

```
ExcelDashboardGenerator/
├── ExcelDashboardGenerator.csproj
├── Program.cs
├── Models/
│   ├── Cliente.cs
│   ├── Empleado.cs
│   ├── Asignacion.cs
│   ├── Vacacion.cs
│   ├── Viaje.cs
│   ├── TurnoSoporte.cs
│   ├── Feriado.cs
│   └── Alerta.cs
├── Services/
│   ├── ExcelGeneratorService.cs
│   ├── DashboardService.cs
│   ├── SlicerService.cs
│   ├── ValidationService.cs
│   ├── FeriadoService.cs
│   └── AlertaService.cs
├── Data/
│   ├── DataContainer.cs
│   └── SampleDataGenerator.cs
└── README.md
```

## 🎨 Sistema de Validaciones

### 1. Vacaciones vs Viajes
- **Nivel**: Alto
- **Descripción**: Detecta si un empleado tiene vacaciones y viajes en fechas superpuestas
- **Acción**: Cancelar o reprogramar uno de los dos

### 2. Vacaciones vs Soporte
- **Nivel**: Alto
- **Descripción**: Detecta si un empleado tiene vacaciones durante su turno de soporte
- **Acción**: Reasignar turno de soporte o reprogramar vacaciones

### 3. Viajes vs Soporte
- **Nivel**: Medio
- **Descripción**: Detecta si un empleado tiene viaje durante su turno de soporte
- **Acción**: Confirmar disponibilidad para soporte remoto

### 4. Viajes en Feriados
- **Nivel**: Bajo
- **Descripción**: Detecta si un viaje está planificado en fecha de feriado del país destino
- **Acción**: Verificar disponibilidad del cliente

### 5. Vacaciones en Feriados
- **Nivel**: Bajo
- **Descripción**: Detecta si las vacaciones incluyen feriados
- **Acción**: Considerar extensión automática

### 6. Asignaciones Múltiples
- **Nivel**: Alto
- **Descripción**: Detecta si un empleado tiene múltiples asignaciones activas
- **Acción**: Revisar carga de trabajo y priorizar

## 📊 Formato del Archivo Excel

### Características de Formato
- **Headers**: Fondo azul oscuro con texto blanco
- **Tablas estructuradas**: Con filtros automáticos
- **Formato condicional**: Colores según estado/prioridad
- **Formato de fechas**: DD/MM/YYYY
- **Ancho de columnas**: Ajustado automáticamente
- **Lookups**: Relaciones entre empleados y clientes

### Leyenda de Colores
- 🟢 **Verde**: Asignaciones activas / Vacaciones aprobadas
- 🟡 **Amarillo**: Pendiente de aprobación / En curso
- 🔴 **Rojo**: Alertas de alta prioridad / Rechazado
- 🔵 **Azul**: Información / Planificado
- ⚫ **Gris**: Inactivo / Histórico

## 💡 Casos de Uso

1. **Gestión de RRHH**: Control de vacaciones, turnos y disponibilidad de empleados
2. **Planificación de Proyectos**: Asignación de empleados a clientes y proyectos
3. **Control de Viajes**: Seguimiento de viajes de negocio y coordinación
4. **Gestión de Soporte**: Rotación automática de turnos de soporte
5. **Prevención de Conflictos**: Detección temprana de solapamientos

## 🔐 Seguridad y Privacidad

- No se almacenan datos sensibles en el código
- Los datos de ejemplo son ficticios
- El archivo Excel se genera localmente
- No hay conexión a servicios externos (excepto Nager.Date para feriados)

## 🛠️ Personalización

### Cambiar Datos de Ejemplo
Editar `Data/SampleDataGenerator.cs` para modificar:
- Número de empleados y clientes
- Países y ciudades
- Fechas de asignaciones
- Conflictos intencionales

### Agregar Nuevas Validaciones
1. Crear método en `Services/ValidationService.cs`
2. Agregar llamada en método `ValidarTodo()`
3. Definir tipo y nivel de alerta

### Personalizar Formato
Modificar `Services/ExcelGeneratorService.cs` para cambiar:
- Colores de las hojas
- Formato de tablas
- KPIs del dashboard
- Estructura de las hojas

## 🐛 Solución de Problemas

### El archivo no se abre automáticamente
**Solución**: Abrir manualmente el archivo desde la carpeta del proyecto.

### Error al cargar feriados
**Solución**: Verificar conexión a internet (Nager.Date requiere acceso en primera ejecución).

### Error de compilación
**Solución**: Verificar que .NET 8.0 SDK esté instalado correctamente con `dotnet --version`.

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo LICENSE para más detalles.

## 👥 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📧 Contacto

Para preguntas o sugerencias, por favor abrir un issue en GitHub.

## 🎓 Créditos

- **ClosedXML**: Para generación de archivos Excel
- **Open XML SDK**: Para funcionalidades avanzadas
- **Nager.Date**: Para gestión de feriados internacionales

---

**Desarrollado con ❤️ usando .NET 8.0**