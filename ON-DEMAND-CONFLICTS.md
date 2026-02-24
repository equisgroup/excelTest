# On-Demand Conflict Calculation - Implementation Guide

## Overview

The system has been updated to calculate conflicts **on-demand** instead of storing them in the database. This ensures conflicts are always current and only shows future conflicts relevant for planning.

## Key Changes

### 1. Automatic Calculation on Page Load

**Conflictos Page (`/Conflictos`):**
- Automatically calculates all future conflicts when you visit the page
- No manual "Recalcular" button needed
- Shows only conflicts from today onwards

**How it works:**
```csharp
// ConflictosController.Index()
var conflictos = await _validationService.ValidarTodosFuturosAsync();
```

### 2. Reports Calculate On-Demand

**Both reports now calculate conflicts when generated:**

**Reporte de Conflictos:**
- Click "Generar Reporte de Conflictos"
- System calculates all future conflicts
- Generates Excel with 3 sheets (Summary, Detailed, By Employee)

**Dashboard Gerencial:** ✅ NOW ACTIVE
- Click "Generar Dashboard Gerencial" (no longer says "Próximamente")
- System calculates conflicts for dashboard
- Generates Excel with KPIs, vacations, trips, and conflict summary

### 3. Future Conflicts Only

The system now focuses only on **future conflicts** (from today onwards):

**Why?**
- Historical conflicts are not relevant for planning
- Reduces clutter and improves performance
- Makes the view more actionable

**What's included:**
- Vacations with end date >= today
- Trips with end date >= today
- Support shifts with end date >= today
- Client over-allocation (always current)

### 4. No Database Storage

**Before:** Conflicts were saved to the database  
**Now:** Conflicts are calculated in memory and not persisted

**Benefits:**
- ✅ Always current (no stale data)
- ✅ Automatically reflects changes in vacations/trips
- ✅ No need to manually recalculate
- ✅ Simpler codebase

## User Guide

### Viewing Conflicts

1. Go to **Conflictos** in the navigation menu
2. Page automatically calculates and shows all future conflicts
3. You'll see:
   - Conflict type (Vacación vs Viaje, etc.)
   - Severity level (Crítico, Alto, Medio, Bajo)
   - Employee name and email
   - Conflict date
   - Description and recommendation

### Creating Vacations/Trips

1. Create a vacation or trip as normal
2. System validates for conflicts
3. If conflicts detected, you'll see a warning
4. Record is still created (you can proceed)
5. Visit **Conflictos** page to see detailed conflicts

### Generating Reports

**Conflict Report:**
1. Go to **Reportes**
2. Click "Generar Reporte de Conflictos"
3. Excel file downloads with current future conflicts
4. Includes 3 sheets with different views

**Dashboard Report:**
1. Go to **Reportes**
2. Click "Generar Dashboard Gerencial"
3. Excel file downloads with KPIs and conflict summary
4. Includes upcoming vacations, recent trips, and conflict counts

## Technical Details

### New Method: ValidarTodosFuturosAsync()

Located in: `ExcelResourceManager.Core/Services/ValidationService.cs`

```csharp
public async Task<List<Conflicto>> ValidarTodosFuturosAsync()
{
    var conflictos = new List<Conflicto>();
    var hoy = DateTime.Now.Date;
    
    // Only validate future records
    var vacaciones = await _unitOfWork.Vacaciones.FindAsync(v => 
        v.FechaFin >= hoy && 
        v.Estado != EstadoVacacion.Cancelada);
    
    var viajes = await _unitOfWork.Viajes.FindAsync(v => 
        v.FechaFin >= hoy && 
        v.Estado != EstadoViaje.Cancelado);
    
    // Validate each record...
    return conflictos;
}
```

### Conflict Types Detected

All 6 types are still detected:

1. **Vacación + Viaje solapados** → Crítico
2. **Vacación + Turno de soporte** → Crítico
3. **Viaje + Turno de soporte** → Medio
4. **Viaje en feriado** → Bajo
5. **Vacación en feriado** → Bajo
6. **Sobreasignación >100%** → Alto

### Performance Considerations

**Is it fast enough?**
- Yes, for typical HR scenarios (hundreds of employees)
- Calculation happens only when viewing Conflictos page or generating reports
- Uses efficient LINQ queries with date filtering

**If needed, caching could be added:**
- Cache conflicts for 5-10 minutes
- Invalidate cache when vacations/trips change
- For now, direct calculation is fast enough

## Migration Notes

### What Was Removed

- ❌ Year filter dropdown on Conflictos page
- ❌ "Recalcular Conflictos" button
- ❌ "Resolver" button (conflicts not stored)
- ❌ Conflict persistence in VacacionesController
- ❌ Conflict persistence in ViajesController

### What Was Added

- ✅ ValidarTodosFuturosAsync() method
- ✅ On-demand calculation in ConflictosController
- ✅ On-demand calculation in reports
- ✅ Info banner explaining automatic calculation
- ✅ Active Dashboard Gerencial report

### Database Impact

**Conflictos table is no longer used** for storing conflicts. It can be:
- Left as-is (ignored by the application)
- Dropped in a future migration
- Kept for potential future audit/history needs

## FAQ

**Q: Do I need to do anything special to see conflicts?**  
A: No, just visit the Conflictos page and they'll be calculated automatically.

**Q: What if I create a vacation today?**  
A: Next time you visit Conflictos, any new conflicts will be shown automatically.

**Q: Can I still see historical conflicts?**  
A: No, only future conflicts are shown. This is by design for better planning.

**Q: What if the page is slow?**  
A: The calculation is typically very fast. If needed, caching can be implemented.

**Q: Are reports always up-to-date?**  
A: Yes! Reports calculate conflicts at the moment of generation, so they're always current.

**Q: What happened to the Dashboard Gerencial report?**  
A: It's now fully functional! It was showing "Próximamente" but now you can generate it.

## Summary

The on-demand conflict calculation approach provides:

✅ **Always current** - No stale data  
✅ **Future-focused** - Only relevant conflicts  
✅ **Automatic** - No manual steps  
✅ **Simpler** - Less code, fewer bugs  
✅ **Complete reporting** - Dashboard now active  

The system now works as requested: conflicts are calculated when you enter the Conflictos page and when generating reports, showing only future conflicts for better planning.
