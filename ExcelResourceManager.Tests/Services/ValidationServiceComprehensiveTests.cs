using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Repositories;
using ExcelResourceManager.Core.Services;
using Moq;
using Xunit;

namespace ExcelResourceManager.Tests.Services;

public class ValidationServiceComprehensiveTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFeriadoService> _mockFeriadoService;
    private readonly ValidationService _validationService;

    public ValidationServiceComprehensiveTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockFeriadoService = new Mock<IFeriadoService>();
        _validationService = new ValidationService(_mockUnitOfWork.Object, _mockFeriadoService.Object);
    }

    [Fact]
    public async Task ValidarTodosFuturosAsync_DetectsVacationTripConflict()
    {
        // Arrange
        var today = DateTime.Today;
        var vacacion = new Vacacion 
        { 
            Id = 1, EmpleadoId = 1, 
            FechaInicio = today.AddDays(10), 
            FechaFin = today.AddDays(15),
            Estado = EstadoVacacion.Aprobada
        };
        
        var viaje = new Viaje 
        { 
            Id = 1, EmpleadoId = 1,
            FechaInicio = today.AddDays(12),
            FechaFin = today.AddDays(14),
            Estado = EstadoViaje.Confirmado
        };

        _mockUnitOfWork.Setup(u => u.Vacaciones.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Vacacion, bool>>>()))
            .ReturnsAsync(new[] { vacacion });
        _mockUnitOfWork.Setup(u => u.Viajes.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Viaje, bool>>>()))
            .ReturnsAsync(new[] { viaje });
        _mockUnitOfWork.Setup(u => u.TurnosSoporte.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<TurnoSoporte, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<TurnoSoporte>());
        _mockUnitOfWork.Setup(u => u.Feriados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Feriado>());
        _mockUnitOfWork.Setup(u => u.Empleados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Empleado>());
        _mockUnitOfWork.Setup(u => u.RolesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<RolCliente>());
        _mockUnitOfWork.Setup(u => u.AsignacionesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<AsignacionCliente>());

        // Act
        var conflictos = await _validationService.ValidarTodosFuturosAsync();

        // Assert
        Assert.Contains(conflictos, c => c.Tipo == TipoConflicto.VacacionVsViaje && c.Nivel == NivelConflicto.Critico);
    }

    [Fact]
    public async Task ValidarTodosFuturosAsync_NoConflicts_ReturnsEmpty()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Vacaciones.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Vacacion, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Vacacion>());
        _mockUnitOfWork.Setup(u => u.Viajes.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Viaje, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Viaje>());
        _mockUnitOfWork.Setup(u => u.TurnosSoporte.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<TurnoSoporte, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<TurnoSoporte>());
        _mockUnitOfWork.Setup(u => u.Feriados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Feriado>());
        _mockUnitOfWork.Setup(u => u.Empleados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Empleado>());
        _mockUnitOfWork.Setup(u => u.RolesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<RolCliente>());
        _mockUnitOfWork.Setup(u => u.AsignacionesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<AsignacionCliente>());

        // Act
        var conflictos = await _validationService.ValidarTodosFuturosAsync();

        // Assert
        Assert.Empty(conflictos);
    }

    [Fact]
    public async Task ValidarTodosFuturosAsync_OnlyFutureConflicts()
    {
        // Arrange
        var today = DateTime.Today;
        var vacacionFutura = new Vacacion 
        { 
            Id = 2, EmpleadoId = 2,
            FechaInicio = today.AddDays(10),
            FechaFin = today.AddDays(15),
            Estado = EstadoVacacion.Aprobada
        };

        _mockUnitOfWork.Setup(u => u.Vacaciones.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Vacacion, bool>>>()))
            .ReturnsAsync(new[] { vacacionFutura });
        _mockUnitOfWork.Setup(u => u.Viajes.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Viaje, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Viaje>());
        _mockUnitOfWork.Setup(u => u.TurnosSoporte.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<TurnoSoporte, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<TurnoSoporte>());
        _mockUnitOfWork.Setup(u => u.Feriados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Feriado>());
        _mockUnitOfWork.Setup(u => u.Empleados.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Empleado>());
        _mockUnitOfWork.Setup(u => u.RolesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<RolCliente>());
        _mockUnitOfWork.Setup(u => u.AsignacionesCliente.GetAllAsync()).ReturnsAsync(Enumerable.Empty<AsignacionCliente>());

        // Act
        var conflictos = await _validationService.ValidarTodosFuturosAsync();

        // Assert
        Assert.All(conflictos, c => Assert.True(c.FechaConflicto >= today));
    }
}
