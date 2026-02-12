# Diagrama de Arquitectura - Excel Resource Manager

```
┌─────────────────────────────────────────────────────────────────┐
│                    SOLUCIÓN VISUAL STUDIO                        │
│                  ExcelResourceManager.slnx                       │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │
        ┌─────────────────────┴─────────────────────┐
        │                                           │
        ▼                                           ▼
┌───────────────────┐                    ┌──────────────────────┐
│ ✅ PROYECTO WEB   │                    │ ⚠️ PROYECTO DESKTOP │
│ (USAR ESTE)       │                    │ (NO USAR)            │
├───────────────────┤                    ├──────────────────────┤
│ ExcelResource     │                    │ ExcelResource        │
│ Manager.Web       │                    │ Manager.Desktop      │
│                   │                    │                      │
│ ASP.NET Core MVC  │                    │ Avalonia UI          │
│ Bootstrap 5       │                    │ ReactiveUI           │
│ Controllers       │                    │ DEPRECATED           │
│ Razor Views       │                    │ Threading issues     │
└─────────┬─────────┘                    └──────────────────────┘
          │
          │ Referencias
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│              CAPA DE NEGOCIO (COMPARTIDA)                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────┐  ┌──────────────────┐  ┌───────────────┐ │
│  │ Core            │  │ Data             │  │ Reports       │ │
│  ├─────────────────┤  ├──────────────────┤  ├───────────────┤ │
│  │ • Models        │  │ • LiteDB Context │  │ • ClosedXML   │ │
│  │ • Enums         │  │ • Repositories   │  │ • Excel Gen   │ │
│  │ • Services      │  │ • UnitOfWork     │  │ • Formatters  │ │
│  │ • Validation    │  │ • Migrations     │  │               │ │
│  └─────────────────┘  └──────────────────┘  └───────────────┘ │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Tests (xUnit + Moq + FluentAssertions)                   │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │
                              ▼
                    ┌─────────────────┐
                    │ LiteDB Database │
                    ├─────────────────┤
                    │ Test Mode:      │
                    │ database-test.db│
                    │                 │
                    │ Prod Mode:      │
                    │ database-prod.db│
                    └─────────────────┘
```

## Flujo de Trabajo

### 1. Desarrollo con Visual Studio
```
[Visual Studio 2022]
        │
        ├─ Abrir: ExcelResourceManager.slnx
        │
        ├─ Establecer como startup: ExcelResourceManager.Web
        │
        ├─ Presionar F5
        │
        └─> [Browser] https://localhost:5001
                │
                └─> Dashboard con KPIs
```

### 2. Desarrollo con CLI
```
[Terminal/CMD]
        │
        ├─ cd ExcelResourceManager.Web
        │
        ├─ dotnet run
        │
        └─> Abrir manualmente: https://localhost:5001
```

## Componentes del Sistema

### Proyecto Web (ASP.NET Core)
```
ExcelResourceManager.Web/
│
├── Controllers/
│   └── DashboardController.cs    ← Lógica de negocio
│
├── Views/
│   ├── Dashboard/
│   │   └── Index.cshtml          ← Vista HTML
│   └── Shared/
│       └── _Layout.cshtml         ← Layout compartido
│
├── wwwroot/
│   ├── css/                       ← Estilos
│   ├── js/                        ← JavaScript
│   └── lib/                       ← Bootstrap, jQuery
│
├── Program.cs                     ← Configuración app
└── appsettings.json              ← Configuración DB
```

### Capa de Negocio (Compartida)
```
ExcelResourceManager.Core/
├── Models/
│   ├── Empleado.cs
│   ├── Cliente.cs
│   ├── Vacacion.cs
│   └── ...
│
├── Enums/
│   ├── EstadoVacacion.cs
│   └── NivelConflicto.cs
│
└── Services/
    ├── IValidationService.cs
    ├── ValidationService.cs
    └── ...
```

## Base de Datos

### Modo Test (Por defecto)
```
database-test.db
├── 3 Ubicaciones (Guayaquil, Quito, Asunción)
├── 20 Empleados
├── 3 Clientes
├── 20 Asignaciones
├── 15 Vacaciones (con conflictos)
├── 10 Viajes
├── 10 Turnos Soporte
└── 31 Feriados 2026
```

### Modo Producción
```
database-prod.db
├── (vacía - lista para datos reales)
```

## Navegación en la Aplicación

```
┌─────────────────────────────────────────────────────┐
│  [Excel Resource Manager]                           │
│  ─────────────────────────────────────────────────  │
│  Dashboard │ Empleados │ Clientes │ Vacaciones     │
│  Conflictos │ Reportes │                            │
└─────────────────────────────────────────────────────┘
                    │
                    ▼
        ┌───────────────────────┐
        │   DASHBOARD           │
        ├───────────────────────┤
        │ KPI Cards:            │
        │  • Empleados: 20/20   │
        │  • Clientes: 3/3      │
        │  • Conflictos: X      │
        │                       │
        │ Próximas Vacaciones:  │
        │  [Tabla con datos]    │
        └───────────────────────┘
```

## Verificación de Estado

### ✅ Sistema Funcionando
```
✓ Solución compila sin errores
✓ Tests pasan (2/2)
✓ Web server responde en localhost:5001
✓ Dashboard muestra datos de prueba
✓ Navegación funciona correctamente
```

### ❌ Proyecto Desktop (No usar)
```
✗ Threading issues
✗ Dispatcher errors
✗ Difícil de depurar
✗ DEPRECATED
```

---

**Ver también:**
- `INICIO.md` - Guía completa de inicio
- `README.md` - Documentación general
