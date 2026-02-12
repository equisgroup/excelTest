# Solución de Problemas - ExcelResourceManager.Web

## Problema Reportado
- Las páginas como `/Empleados` devuelven 404
- No se puede cambiar entre modo Prueba y Producción

## Solución: Limpiar y Reconstruir en Visual Studio

### Pasos en Visual Studio 2022:

1. **Limpiar la Solución**
   - Clic en el menú "Compilar" → "Limpiar solución"
   - O presionar: `Ctrl+Shift+B` (después de seleccionar Limpiar)

2. **Restaurar Paquetes NuGet**
   - Clic derecho en la solución → "Restaurar paquetes NuGet"
   - O en la consola del Administrador de paquetes: `dotnet restore`

3. **Reconstruir la Solución**
   - Clic en el menú "Compilar" → "Recompilar solución"  
   - O presionar: `Ctrl+Alt+F7`

4. **Configurar Proyecto de Inicio**
   - Clic derecho en `ExcelResourceManager.Web`
   - Seleccionar "Establecer como proyecto de inicio"

5. **Ejecutar**
   - Presionar `F5` o clic en el botón "https" verde en la barra de herramientas
   - Esperar a que abra el navegador

### Pasos desde Línea de Comandos:

```bash
# 1. Navegar al directorio raíz
cd /ruta/a/excelTest

# 2. Limpiar
dotnet clean

# 3. Restaurar paquetes
dotnet restore

# 4. Reconstruir
dotnet build

# 5. Ejecutar la aplicación Web
cd ExcelResourceManager.Web
dotnet run
```

## Verificación de Funcionamiento

### 1. Verificar que el navegador abre automáticamente

La aplicación debería abrir en: `https://localhost:7061`

### 2. Probar Navegación

Hacer clic en cada opción del menú:
- ✅ **Dashboard** → https://localhost:7061/Dashboard
- ✅ **Empleados** → https://localhost:7061/Empleados  
- ✅ **Clientes** → https://localhost:7061/Clientes
- ✅ **Vacaciones** → https://localhost:7061/Vacaciones
- ✅ **Conflictos** → https://localhost:7061/Conflictos
- ✅ **Reportes** → https://localhost:7061/Reportes

### 3. Probar Cambio de Modo

1. Observar en la esquina superior derecha: "Modo: **Prueba**"
2. Hacer clic en el botón "Cambiar a Producción"
3. La página recarga y ahora dice: "Modo: **Producción**"
4. El Dashboard en Producción mostrará 0 empleados (base de datos vacía)
5. Cambiar de vuelta a Prueba para ver los 20 empleados de prueba

## Problemas Comunes

### 404 en todas las páginas
**Causa**: La aplicación no se recompiló después de los últimos cambios.
**Solución**: Seguir los pasos de limpieza y reconstrucción arriba.

### El modo no cambia
**Causa**: La sesión no está funcionando correctamente.
**Solución**: 
1. Cerrar completamente el navegador
2. Borrar cookies/caché del navegador
3. Reiniciar la aplicación
4. Abrir en modo incógnito para probar

### Error "No se puede encontrar la página"
**Causa**: La aplicación no está ejecutándose.
**Solución**: Verificar que la aplicación esté corriendo en Visual Studio (debería decir "Ejecutando..." en la esquina inferior izquierda).

### Puerto ya en uso
**Causa**: Otra instancia de la aplicación está ejecutándose.
**Solución**:
- Cerrar todas las instancias de Visual Studio
- Abrir el Administrador de tareas
- Buscar procesos "dotnet.exe" o "ExcelResourceManager.Web.exe"
- Finalizar esos procesos
- Reiniciar Visual Studio

## Verificación Técnica (Para Desarrolladores)

### Verificar que los controladores existen:
```bash
ls -l ExcelResourceManager.Web/Controllers/
```

Deberías ver:
- DashboardController.cs
- EmpleadosController.cs
- ClientesController.cs
- VacacionesController.cs
- ConflictosController.cs
- ReportesController.cs
- HomeController.cs

### Verificar que las vistas existen:
```bash
ls -l ExcelResourceManager.Web/Views/Empleados/
```

Debería existir: `Index.cshtml`

### Verificar la configuración de rutas en Program.cs:
Buscar esta línea (debería estar en línea ~88):
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
```

## Estado Actual del Proyecto

✅ **Todos los controladores implementados**  
✅ **Todas las vistas creadas**  
✅ **Navegación configurada correctamente**  
✅ **Cambio de modo implementado**  
✅ **Base de datos de prueba con 20 empleados**  
✅ **Base de datos de producción vacía**  

## Si Nada Funciona

1. Clonar el repositorio de nuevo en un directorio limpio
2. Abrir la solución en Visual Studio
3. Restaurar paquetes NuGet
4. Compilar
5. Ejecutar

O desde línea de comandos:
```bash
git clone https://github.com/equisgroup/excelTest.git nuevo-directorio
cd nuevo-directorio
dotnet restore
dotnet build
cd ExcelResourceManager.Web
dotnet run
```

Luego abrir: https://localhost:7061
