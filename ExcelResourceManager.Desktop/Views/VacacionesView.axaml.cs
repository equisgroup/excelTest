using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ExcelResourceManager.Desktop.Views;

public partial class VacacionesView : UserControl
{
    public VacacionesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
