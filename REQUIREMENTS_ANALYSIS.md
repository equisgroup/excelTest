# Requirements Analysis - Comprehensive Conflict System Redesign

## User Requirements Summary

### 1. CORRECT Conflict Types (Remove Sobreasignacion)
**Current:** 6 types including Sobreasignacion
**Required:** 5 types only:
- Vacación + Viaje solapados
- Vacación + Turno de soporte  
- Viaje + Turno de soporte
- Viaje en feriado
- Vacación en feriado
**Action:** Remove Sobreasignacion from TipoConflicto enum

### 2. Client Contract Management
**New Fields for Cliente:**
- FechaContratoInicio (Contract start date)
- FechaContratoFin (Contract end date)

### 3. Role Requirements Management
**New Model: RolCliente**
- ClienteId
- Rol (string - role name)
- CantidadRequerida (int - number of resources needed)
- FechaInicio (period start)
- FechaFin (period end)

### 4. Role-Based Assignments
**Update AsignacionCliente:**
- Add: Rol (string - specific role for this assignment)
- Keep: EmpleadoId, ClienteId, FechaInicio, FechaFin, PorcentajeAsignacion

### 5. New Conflict Types for Role Coverage
**Add to TipoConflicto enum:**
- RolNoAsignado (Role required by client but no assignment exists)
- RolSinCobertura (Role loses coverage when employee goes on vacation)
- CoberturaSuperaContratada (More employees assigned than contracted, considering vacations)
- AsignacionFueraContrato (Employee-Role assigned outside client contract period)

### 6. Fix Empty Excel Reports
**Current Issue:** Reports show 0 conflicts, all sheets empty
**Root Cause:** Need to verify ValidarTodosFuturosAsync() is returning data
**Action:** Debug and fix data flow

## Implementation Estimate

### Phase 1: Core Model Changes (2-3 hours)
- Update TipoConflicto enum
- Update Cliente model
- Create RolCliente model
- Update AsignacionCliente model
- Database migration/seed data updates

### Phase 2: Validation Logic (3-4 hours)
- Remove Sobreasignacion validation
- Add RolNoAsignado validation
- Add RolSinCobertura validation
- Add CoberturaSuperaContratada validation
- Add AsignacionFueraContrato validation

### Phase 3: UI Updates (2-3 hours)
- Cliente CRUD with contract dates
- RolCliente CRUD interface
- AsignacionCliente with role selection
- Conflict display updates

### Phase 4: Testing & Debugging (2-3 hours)
- Fix Excel report generation
- Test all conflict scenarios
- Verify data flow

**Total Estimate:** 9-13 hours of development work

## Recommendation

Due to the extensive nature of these changes, I recommend implementing in phases:

**Immediate (This Session):**
1. Remove Sobreasignacion conflict type
2. Fix Excel report debugging
3. Add contract date fields to Cliente

**Next Session:**
4. Implement RolCliente model and CRUD
5. Update AsignacionCliente with roles
6. Implement new validation logic

This approach ensures we make progress while maintaining system stability.
