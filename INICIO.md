# 🚀 Guía de Inicio - Excel Resource Manager

## ⚠️ IMPORTANTE: Nueva Arquitectura Web

### Respuestas a Preguntas Frecuentes

#### ❓ ¿La solución existente de Visual Studio ya no sirve?
**Respuesta:** La solución SÍ sirve y ha sido actualizada. Ahora incluye:
- ✅ **ExcelResourceManager.Web** (NUEVO - USAR ESTE)
- ⚠️ **ExcelResourceManager.Desktop** (DEPRECATED - No usar, solo referencia)
- ✅ ExcelResourceManager.Core (Lógica de negocio)
- ✅ ExcelResourceManager.Data (Base de datos)
- ✅ ExcelResourceManager.Reports (Reportes Excel)
- ✅ ExcelResourceManager.Tests (Pruebas)

#### ❓ ¿Hay que eliminar algo?
**Respuesta:** NO eliminar nada. El proyecto Desktop se mantiene como referencia, pero **usar solamente el proyecto Web**.

#### ❓ ¿Cómo se carga e inicia ExcelResourceManager.Web?

## 📋 Opción 1: Visual Studio 2022 (Recomendado)

### Paso 1: Abrir la Solución
1. Abrir **Visual Studio 2022** (o 2019)
2. Seleccionar `Archivo → Abrir → Proyecto/Solución`
3. Navegar a la carpeta del proyecto
4. Abrir `ExcelResourceManager.slnx`

### Paso 2: Configurar Proyecto de Inicio
1. En el **Explorador de Soluciones**, buscar `ExcelResourceManager.Web`
2. Click derecho sobre `ExcelResourceManager.Web`
3. Seleccionar **"Establecer como proyecto de inicio"**
4. El proyecto se pondrá en **negrita**

### Paso 3: Ejecutar la Aplicación
1. Presionar **F5** o click en el botón ▶️ **"ExcelResourceManager.Web"**
2. Visual Studio compilará y ejecutará la aplicación
3. Se abrirá automáticamente el navegador en `https://localhost:5001`

### Paso 4: Verificar Funcionamiento
- Deberías ver el **Dashboard** con:
  - KPIs de Empleados, Clientes, Conflictos
  - Lista de próximas vacaciones
  - Menú de navegación superior

## 📋 Opción 2: Línea de Comandos (dotnet CLI)

### Paso 1: Navegar al Proyecto Web
```bash
cd ExcelResourceManager.Web
```

### Paso 2: Restaurar Dependencias (primera vez)
```bash
dotnet restore
```

### Paso 3: Compilar el Proyecto
```bash
dotnet build
```

### Paso 4: Ejecutar la Aplicación
```bash
dotnet run
```

### Paso 5: Abrir en Navegador
Abrir manualmente el navegador en:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

## 🔧 Configuración del Proyecto

### Estructura de Archivos
```
ExcelResourceManager/
├── ExcelResourceManager.slnx           ← Solución Visual Studio
├── ExcelResourceManager.Web/           ← 🌟 PROYECTO PRINCIPAL (USAR ESTE)
│   ├── Controllers/                    - Controladores MVC
│   ├── Views/                          - Vistas Razor
│   ├── wwwroot/                        - Archivos estáticos (CSS, JS)
│   ├── Program.cs                      - Configuración de la app
│   └── appsettings.json                - Configuración
│
├── ExcelResourceManager.Desktop/       ← ⚠️ DEPRECATED (No usar)
├── ExcelResourceManager.Core/          ← Lógica de negocio
├── ExcelResourceManager.Data/          ← Acceso a datos (LiteDB)
├── ExcelResourceManager.Reports/       ← Generación de Excel
└── ExcelResourceManager.Tests/         ← Pruebas unitarias
```

### Archivo de Configuración
El archivo `ExcelResourceManager.Web/appsettings.json` contiene:
```json
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
```

**Modo Test**: Usa `database-test.db` con datos de prueba (20 empleados, 15 vacaciones, etc.)  
**Modo Producción**: Usa `database-prod.db` vacía

## 🎯 Funcionalidades Disponibles

Una vez ejecutada la aplicación, puedes acceder a:

### Menú Principal
1. **Dashboard** - Vista general con KPIs
2. **Empleados** - Gestión de empleados (próximamente)
3. **Clientes** - Gestión de clientes (próximamente)
4. **Vacaciones** - Solicitudes de vacaciones (próximamente)
5. **Conflictos** - Visualización de conflictos (próximamente)
6. **Reportes** - Generación de reportes Excel (próximamente)

### Dashboard (Implementado)
- ✅ Total de empleados activos
- ✅ Total de clientes activos
- ✅ Conflictos pendientes
- ✅ Próximas vacaciones (30 días)

## ❗ Solución de Problemas Comunes

### Error: "No se puede conectar a localhost"
**Solución:** Verificar que la aplicación esté ejecutándose. Debería ver en la consola:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### Error: "El puerto ya está en uso"
**Solución:** 
1. Detener cualquier otra aplicación que use el puerto 5001
2. O cambiar el puerto en `Properties/launchSettings.json`

### Error: "No se encuentra la base de datos"
**Solución:** La aplicación creará automáticamente `database-test.db` en la primera ejecución.

### No aparecen datos en el Dashboard
**Solución:** 
1. Verificar que existe `database-test.db` en la raíz del proyecto Web
2. Eliminar `database-test.db` y reiniciar la aplicación para regenerar datos

## 🔄 Cambio de Modo Test/Producción

Editar `appsettings.json`:
```json
{
  "App": {
    "DefaultMode": "Test"    ← Cambiar a "Production" para usar BD vacía
  }
}
```

Reiniciar la aplicación para aplicar cambios.

## 🧪 Ejecutar Pruebas

### Desde Visual Studio
1. Menú `Prueba → Ejecutar todas las pruebas`
2. Ver resultados en el **Explorador de Pruebas**

### Desde Línea de Comandos
```bash
dotnet test
```

## 📚 Documentación Adicional

- **README.md** - Información general del proyecto
- **ExcelResourceManager.Web/README.md** - Detalles de la aplicación web
- **Logs/** - Registros de la aplicación (se crean automáticamente)

## 🆘 Ayuda

Si encuentras problemas:
1. Revisar los logs en `ExcelResourceManager.Web/Logs/log-{fecha}.txt`
2. Verificar que .NET 8.0 SDK esté instalado: `dotnet --version`
3. Asegurarse de que todos los proyectos compilen sin errores

---

## ✅ Checklist de Inicio Rápido

- [ ] Abrir `ExcelResourceManager.slnx` en Visual Studio
- [ ] Establecer `ExcelResourceManager.Web` como proyecto de inicio
- [ ] Presionar F5 para ejecutar
- [ ] Verificar que se abre el navegador en https://localhost:5001
- [ ] Ver el Dashboard con datos de prueba

**¡Listo para usar!** 🎉
