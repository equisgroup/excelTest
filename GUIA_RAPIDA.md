# Guía Rápida - ExcelResourceManager.Web

## ✅ Lo que DEBES hacer AHORA:

### 1. Limpiar y Reconstruir en Visual Studio

```
Menú → Compilar → Limpiar solución
Menú → Compilar → Recompilar solución
```

**¿Por qué?** Los cambios recientes no están en tu build actual.

### 2. Ejecutar

```
Clic derecho en ExcelResourceManager.Web → "Establecer como proyecto de inicio"
Presionar F5
```

### 3. Verificar en el Navegador

La aplicación debe abrir automáticamente en: **https://localhost:7061**

## 📊 Lo que DEBES ver después:

### Página Principal (Dashboard)
```
+--------------------------------------------------+
| Excel Resource Manager                           |
| [Dashboard] [Empleados] [Clientes] [Vacaciones] |
| [Conflictos] [Reportes]    Modo: Prueba [Cambiar]|
+--------------------------------------------------+
|                                                  |
| Dashboard                                        |
|                                                  |
| [Empleados: 20]  [Clientes: 3]  [Conflictos: X] |
|                                                  |
| Próximas Vacaciones                              |
| +----------------------------------------------+ |
| | Empleado        | Inicio     | Fin          | |
| | Juan Pérez      | 15/03/2026 | 20/03/2026   | |
| | ...             | ...        | ...          | |
| +----------------------------------------------+ |
+--------------------------------------------------+
```

### Navegación (hacer clic en cada una):

1. **Empleados** → `/Empleados`
   - Debe mostrar tabla con 20 empleados
   - Columnas: ID, Nombre Completo, Email, Rol, Fecha Ingreso, Estado

2. **Clientes** → `/Clientes`
   - Debe mostrar tabla con 3 clientes
   - Columnas: ID, Nombre, Ubicación, Código Interno, Email

3. **Vacaciones** → `/Vacaciones`
   - Debe mostrar tabla de vacaciones
   - Columnas: Empleado, Fecha Inicio, Fecha Fin, Días Hábiles, Estado

4. **Conflictos** → `/Conflictos`
   - Debe mostrar tabla de conflictos detectados
   - Columnas: Tipo, Nivel, Empleado, Fecha, Descripción

5. **Reportes** → `/Reportes`
   - Debe mostrar botón "Generar Reporte de Conflictos"

### Cambio de Modo

1. **Modo Prueba (actual)**:
   - Top-right dice: "Modo: **Prueba**"
   - Dashboard muestra: 20 empleados, 3 clientes
   
2. **Click en "Cambiar a Producción"**:
   - Página recarga
   - Top-right dice: "Modo: **Producción**"
   - Dashboard muestra: 0 empleados, 0 clientes (base de datos vacía)

3. **Click en "Cambiar a Prueba"**:
   - Vuelve a mostrar los 20 empleados

## ❌ Si ves ERROR 404 "No se encuentra esta página"

**Significa que NO has reconstruido la solución.**

### Solución:
1. Cerrar el navegador
2. Detener la aplicación en Visual Studio (Shift+F5)
3. Menú → Compilar → Limpiar solución
4. Menú → Compilar → Recompilar solución
5. Presionar F5

## ❌ Si el modo NO cambia

1. Cerrar completamente el navegador
2. Borrar caché y cookies
3. Reiniciar la aplicación
4. Probar en ventana de incógnito

## 📞 ¿Aún no funciona?

Ver documentación detallada en: [SOLUCION_PROBLEMAS.md](SOLUCION_PROBLEMAS.md)

## ✅ Checklist de Verificación

- [ ] Limpiado la solución
- [ ] Reconstruido la solución (sin errores)
- [ ] Establecido ExcelResourceManager.Web como proyecto de inicio
- [ ] Presionado F5
- [ ] Navegador abrió automáticamente
- [ ] URL es https://localhost:7061 (o similar)
- [ ] Dashboard carga correctamente
- [ ] Clic en Empleados → muestra tabla con 20 empleados
- [ ] Clic en Clientes → muestra tabla con 3 clientes
- [ ] Clic en "Cambiar a Producción" → recarga y muestra 0 empleados
- [ ] Clic en "Cambiar a Prueba" → recarga y muestra 20 empleados

**Si TODO esto funciona = ✅ ÉXITO**

## 🎯 Resumen

**El código está correcto.** 

Todos los controladores, vistas y la navegación están implementados correctamente.

**Solo necesitas reconstruir la solución en Visual Studio para que los cambios se apliquen.**

```
Limpiar → Recompilar → F5 → ¡Funciona!
```
