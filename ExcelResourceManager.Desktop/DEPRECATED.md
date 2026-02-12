# ⚠️ PROYECTO DEPRECATED - NO USAR

## Este proyecto (ExcelResourceManager.Desktop) está DEPRECADO

### ❌ Problemas conocidos:
- **Threading issues** - `InvalidOperationException: "Call from invalid thread"`
- **CheckAccess() failures** - Dispatcher errors con async/await
- **Complejidad de ReactiveUI** - Difícil de mantener y depurar

### ✅ Usar en su lugar:
**ExcelResourceManager.Web** - Aplicación ASP.NET Core MVC

## ¿Por qué se mantiene este proyecto?

Se mantiene como **referencia histórica** y para entender la evolución del proyecto, pero **NO debe ser usado** en desarrollo o producción.

## ¿Qué hacer?

1. **NO ejecutar este proyecto**
2. **Usar ExcelResourceManager.Web** para toda funcionalidad
3. Toda la lógica de negocio en `ExcelResourceManager.Core` funciona igual

## Migración completada

Todo el código de negocio (Core, Data, Reports) se reutiliza 100% en la nueva aplicación Web, sin los problemas de threading de Avalonia.

---

**Para más información, ver:**
- `/INICIO.md` - Guía de inicio completa
- `/README.md` - Documentación general
- `/ExcelResourceManager.Web/README.md` - Detalles de la aplicación web
