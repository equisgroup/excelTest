using ExcelResourceManager.Core.Enums;
using ExcelResourceManager.Core.Models;
using ExcelResourceManager.Core.Services;
using ExcelResourceManager.Core.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExcelResourceManager.Tests;

public class ValidationServiceTests
{
    [Fact]
    public async Task FeriadoService_CalcularDiasHabiles_ExcluyeFinesDeSemana()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Feriados.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Feriado, bool>>>()))
            .ReturnsAsync(new List<Feriado>());
        
        var feriadoService = new FeriadoService(unitOfWorkMock.Object);
        
        // Act - Lunes 2 mar a Viernes 6 mar 2026 = 5 días hábiles
        var diasHabiles = await feriadoService.CalcularDiasHabilesAsync(
            new DateTime(2026, 3, 2),  // Lunes
            new DateTime(2026, 3, 6),  // Viernes
            1
        );
        
        // Assert
        diasHabiles.Should().Be(5);
    }
    
    [Fact]
    public void Empleado_NombreCompleto_CombinaNombreYApellido()
    {
        // Arrange & Act
        var empleado = new Empleado 
        { 
            Nombre = "Juan", 
            Apellido = "Pérez" 
        };
        
        // Assert
        empleado.NombreCompleto.Should().Be("Juan Pérez");
    }
}
