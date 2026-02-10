using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ExcelResourceManager.Core.Enums;

namespace ExcelResourceManager.Desktop.Converters;

public class NivelConflictoToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NivelConflicto nivel)
            return new SolidColorBrush(Colors.Gray);

        return nivel switch
        {
            NivelConflicto.Critico => new SolidColorBrush(Color.FromRgb(255, 235, 238)),      // Light Red
            NivelConflicto.Alto => new SolidColorBrush(Color.FromRgb(255, 243, 224)),         // Light Orange
            NivelConflicto.Medio => new SolidColorBrush(Color.FromRgb(255, 249, 196)),        // Light Yellow
            NivelConflicto.Bajo => new SolidColorBrush(Color.FromRgb(232, 245, 233)),         // Light Green
            NivelConflicto.Informativo => new SolidColorBrush(Color.FromRgb(227, 242, 253)),  // Light Blue
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NivelConflictoToBorderColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NivelConflicto nivel)
            return new SolidColorBrush(Colors.Gray);

        return nivel switch
        {
            NivelConflicto.Critico => new SolidColorBrush(Color.FromRgb(211, 47, 47)),       // Dark Red
            NivelConflicto.Alto => new SolidColorBrush(Color.FromRgb(255, 152, 0)),          // Dark Orange
            NivelConflicto.Medio => new SolidColorBrush(Color.FromRgb(251, 192, 45)),        // Dark Yellow
            NivelConflicto.Bajo => new SolidColorBrush(Color.FromRgb(56, 142, 60)),          // Dark Green
            NivelConflicto.Informativo => new SolidColorBrush(Color.FromRgb(2, 136, 209)),   // Dark Blue
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EstadoVacacionToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not EstadoVacacion estado)
            return new SolidColorBrush(Colors.Gray);

        return estado switch
        {
            EstadoVacacion.Solicitada => new SolidColorBrush(Color.FromRgb(33, 150, 243)),   // Blue
            EstadoVacacion.Aprobada => new SolidColorBrush(Color.FromRgb(76, 175, 80)),      // Green
            EstadoVacacion.Rechazada => new SolidColorBrush(Color.FromRgb(244, 67, 54)),     // Red
            EstadoVacacion.Cancelada => new SolidColorBrush(Color.FromRgb(158, 158, 158)),   // Gray
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
