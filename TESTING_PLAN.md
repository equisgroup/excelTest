# Comprehensive Testing Plan

## Overview
This document outlines the complete testing strategy for the role-based conflict detection system.

## Testing Goals
1. **Coverage:** 80%+ for core services, 90%+ for validation logic
2. **Quality:** All 9 conflict types thoroughly tested
3. **Reliability:** Integration tests for critical workflows
4. **Maintainability:** Clear test structure and naming

## Test Structure

### 1. Unit Tests

#### ValidationServiceComprehensiveTests ⭐ PRIORITY
Tests all 9 conflict types with isolated scenarios:

**Conflict Type 1: Vacación + Viaje solapados (Critical)**
- Test: Vacation overlaps with trip dates
- Expected: Critical conflict detected
- Verify: Proper date range overlap logic

**Conflict Type 2: Vacación + Turno de soporte (Critical)**
- Test: Vacation during support shift
- Expected: Critical conflict detected
- Verify: Cannot take vacation during support duty

**Conflict Type 3: Viaje + Turno de soporte (Medium)**
- Test: Trip during support shift
- Expected: Medium conflict (can work remotely)
- Verify: Less severe than vacation conflict

**Conflict Type 4: Viaje en feriado (Low)**
- Test: Trip scheduled on destination holiday
- Expected: Low priority warning
- Verify: Holiday service integration

**Conflict Type 5: Vacación en feriado (Low)**
- Test: Vacation on public holiday
- Expected: Low priority (wasted vacation day)
- Verify: Location-based holiday check

**Conflict Type 6: Rol no asignado (High/Medium)**
- Test: Client requires role but no one assigned
- Expected: High/Medium conflict
- Verify: Role requirement vs assignment comparison

**Conflict Type 7: Rol sin cobertura (High)**
- Test: Vacation leaves role uncovered
- Expected: High priority conflict
- Verify: Coverage calculation during vacation

**Conflict Type 8: Cobertura > contratada (Low)**
- Test: More resources than contracted
- Expected: Low priority (over-staffing)
- Verify: Assignment count vs requirement

**Conflict Type 9: Asignación fuera de contrato (Critical)**
- Test: Assignment outside contract dates
- Expected: Critical conflict
- Verify: Contract period validation

**No Conflict Scenarios:**
- Test: Valid vacation with no overlaps
- Test: Valid trip with no conflicts
- Expected: Empty conflict list

#### FeriadoServiceTests
- Test: Ecuador holidays 2026
- Test: Paraguay holidays 2026
- Test: Location-specific holidays
- Test: National vs local holidays
- Expected: Accurate holiday detection

#### DataSeedServiceTests
- Test: Generates all required entities
- Test: Creates realistic scenarios
- Test: Produces expected conflicts
- Expected: Consistent test data

### 2. Integration Tests

#### Conflict Detection Workflow
```
Create Vacation → Validate → Detect Conflicts → Display
```
- Test full workflow
- Verify conflicts appear in UI
- Check Excel report includes conflicts

#### Role Coverage Workflow
```
Define Role → Assign Employee → Schedule Vacation → Detect Coverage Gap
```
- Test role requirement definition
- Verify coverage calculation
- Confirm conflict generation

#### Report Generation Workflow
```
Calculate Conflicts → Format Data → Generate Excel → Verify Content
```
- Test all 3 sheets populated
- Verify conditional formatting
- Confirm correct conflict counts

### 3. Controller Tests

#### ConflictosController
- Test: Index action calculates conflicts
- Test: Conflicts display correctly
- Expected: All 9 types visible

#### ReportesController
- Test: GenerarConflictos downloads Excel
- Test: GenerarDashboard generates report
- Expected: Valid Excel files

#### RolesClienteController (Once Implemented)
- Test: CRUD operations
- Test: Role requirement validation
- Expected: Proper data persistence

### 4. UI/E2E Tests (Manual Initially)

#### Cliente Management
- Create client with contract dates
- Edit contract dates
- Verify validation works

#### RolCliente Management
- Create role requirement
- Edit quantity/dates
- Delete requirement

#### Conflict Detection
- Create vacation that triggers conflict
- Verify warning appears
- Check conflict shows in Conflictos page

#### Report Generation
- Generate conflict report
- Verify Excel downloads
- Check all sheets populated

## Test Data Scenarios

### Scenario 1: Vacation-Trip Overlap
```
Employee: Juan Pérez
Vacation: March 15-20
Trip: March 18-22 (Client site)
Expected: Critical conflict
```

### Scenario 2: Role Coverage Gap
```
Client: TechCorp
Role Required: Developer (2 people)
Assigned: 2 Developers
Vacation: Dev 1 on March 15-20
Expected: High conflict if only 1 developer remains
```

### Scenario 3: Assignment Outside Contract
```
Client: GlobalCorp
Contract: Jan 1 - Jun 30
Assignment: July 1 - Dec 31
Expected: Critical conflict
```

### Scenario 4: Over-Staffing
```
Client: StartupCo
Role Required: QA (1 person)
Assigned: 2 QA Engineers
Expected: Low conflict (wasted resources)
```

## Test Implementation

### Phase 1: Core Validation Tests (90 min)
1. Create ValidationServiceComprehensiveTests.cs
2. Implement all 9 conflict type tests
3. Add edge case tests
4. Run and verify all pass

### Phase 2: Service Tests (30 min)
1. Create FeriadoServiceTests.cs
2. Test holiday detection logic
3. Create DataSeedServiceTests.cs
4. Verify test data generation

### Phase 3: Integration Tests (45 min)
1. Create ConflictWorkflowTests.cs
2. Test end-to-end scenarios
3. Verify report generation
4. Check Excel content

### Phase 4: Manual UI Testing (30 min)
1. Test all CRUD operations
2. Verify conflict warnings
3. Check report downloads
4. Document any issues

## Coverage Targets

### Minimum Coverage
- ValidationService: 90% (critical business logic)
- FeriadoService: 80%
- DataSeedService: 70%
- Controllers: 60%
- Overall: 75%

### Priority Coverage
1. Conflict detection methods (all 9 types)
2. Role-based validation logic
3. Date overlap calculations
4. Holiday detection

## Quality Gates

Before marking as complete:
- [ ] All 20+ unit tests pass
- [ ] Integration tests pass
- [ ] Build succeeds with 0 errors
- [ ] Code coverage meets targets
- [ ] Manual UI testing completed
- [ ] Excel reports verified
- [ ] Documentation updated

## Test Maintenance

### When Adding New Features
1. Write tests first (TDD approach)
2. Ensure tests fail initially
3. Implement feature
4. Verify tests pass
5. Check coverage maintained

### When Fixing Bugs
1. Write test that reproduces bug
2. Verify test fails
3. Fix the bug
4. Verify test passes
5. Add to regression suite

## Test Naming Conventions

```csharp
[Fact]
public async Task <MethodName>_When<Scenario>_Should<ExpectedOutcome>()
{
    // Arrange
    // Act
    // Assert
}
```

Example:
```csharp
ValidarVacacion_WhenOverlapsWithTrip_ShouldReturnCriticalConflict()
```

## Test Tools

- **xUnit:** Test framework
- **Moq:** Mocking framework
- **FluentAssertions:** Readable assertions
- **Coverlet:** Code coverage
- **Visual Studio:** Test runner & debugger

## Running Tests

```bash
# All tests
dotnet test

# Specific test file
dotnet test --filter "FullyQualifiedName~ValidationServiceComprehensiveTests"

# With coverage
dotnet test /p:CollectCoverage=true

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Success Criteria

System is ready for production when:
1. ✅ All tests pass
2. ✅ Coverage targets met
3. ✅ No critical bugs
4. ✅ Excel reports work correctly
5. ✅ All 9 conflict types detect properly
6. ✅ UI workflows complete
7. ✅ Documentation current

---

**Status:** 📋 Testing strategy defined - Ready for implementation
