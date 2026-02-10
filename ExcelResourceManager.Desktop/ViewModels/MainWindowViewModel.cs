using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace ExcelResourceManager.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase? _currentView;
    private bool _isTestMode = true;

    public ViewModelBase? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public bool IsTestMode
    {
        get => _isTestMode;
        set => this.RaiseAndSetIfChanged(ref _isTestMode, value);
    }

    public string ModoActual => IsTestMode ? "Modo: Prueba" : "Modo: Producción";

    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainWindowViewModel()
    {
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);

        // Observar cambios en IsTestMode
        this.WhenAnyValue(x => x.IsTestMode)
            .Skip(1) // Saltar el valor inicial
            .Subscribe(isTest =>
            {
                Console.WriteLine($"Modo cambiado a: {(isTest ? "Prueba" : "Producción")}");
                // Aquí se podría recargar la base de datos según el modo
            });

        // Cargar Dashboard por defecto
        Navigate("Dashboard");
    }

    private void Navigate(string destination)
    {
        CurrentView = destination switch
        {
            "Dashboard" => new DashboardViewModel(),
            "Vacaciones" => new VacacionesViewModel(),
            "Empleados" => new EmpleadosViewModel(),
            "Clientes" => new ClientesViewModel(),
            "Viajes" => new ViajesViewModel(),
            "Soporte" => new TurnosSoporteViewModel(),
            "Feriados" => new FeriadosViewModel(),
            "Conflictos" => new ConflictosViewModel(),
            "Reportes" => new ReportesViewModel(),
            _ => CurrentView
        };

        Console.WriteLine($"Navegando a: {destination}");
    }
}
