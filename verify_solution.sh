#!/bin/bash
echo "╔════════════════════════════════════════════════════════════╗"
echo "║         VERIFICACIÓN DE SOLUCIÓN COMPLETA                  ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Verificar que existen todos los archivos
echo "✓ Verificando estructura del proyecto..."
check_file() {
    if [ -f "$1" ]; then
        echo "  ✓ $1"
    else
        echo "  ✗ $1 NO ENCONTRADO"
        exit 1
    fi
}

check_file "ExcelDashboardGenerator.csproj"
check_file "Program.cs"
check_file "README.md"
check_file ".gitignore"

# Models
echo ""
echo "✓ Verificando modelos..."
for model in Cliente Empleado Asignacion Vacacion Viaje TurnoSoporte Feriado Alerta; do
    check_file "Models/${model}.cs"
done

# Services
echo ""
echo "✓ Verificando servicios..."
for service in ExcelGeneratorService DashboardService SlicerService ValidationService FeriadoService AlertaService; do
    check_file "Services/${service}.cs"
done

# Data
echo ""
echo "✓ Verificando datos..."
check_file "Data/DataContainer.cs"
check_file "Data/SampleDataGenerator.cs"

echo ""
echo "✓ Compilando proyecto..."
dotnet build --nologo -v quiet
if [ $? -eq 0 ]; then
    echo "  ✓ Compilación exitosa"
else
    echo "  ✗ Error en compilación"
    exit 1
fi

echo ""
echo "╔════════════════════════════════════════════════════════════╗"
echo "║       ✅ VERIFICACIÓN COMPLETADA EXITOSAMENTE              ║"
echo "╚════════════════════════════════════════════════════════════╝"
