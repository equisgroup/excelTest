using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;

namespace ExcelResourceManager.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase? _currentView;
    private bool _isTestMode = true;
    private string _modoActual = "Modo: Prueba";

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

    public string ModoActual
    {
        get => _modoActual;
        private set => this.RaiseAndSetIfChanged(ref _modoActual, value);
    }

    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainWindowViewModel()
    {
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);

        // Observar cambios en IsTestMode y actualizar ModoActual
        this.WhenAnyValue(x => x.IsTestMode)
            .Subscribe(isTest =>
            {
                ModoActual = isTest ? "Modo: Prueba" : "Modo: Producción";
            });

        // Observar cambios en IsTestMode para recargar la vista actual
        this.WhenAnyValue(x => x.IsTestMode)
            .Skip(1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(isTest =>
            {
                Log.Information("Modo cambiado a: {Modo}", isTest ? "Prueba" : "Producción");
                
                // Recargar la vista actual para reflejar los datos del nuevo modo
                if (CurrentView is DashboardViewModel)
                {
                    Navigate("Dashboard");
                }
                else if (CurrentView is VacacionesViewModel)
                {
                    Navigate("Vacaciones");
                }
            });

        // Cargar Dashboard por defecto
        Navigate("Dashboard");
    }

    private void Navigate(string destination)
    {
        // Asegurarse de que la navegación ocurra en el hilo de UI
        Dispatcher.UIThread.Post(() =>
        {
            var newView = destination switch
            {
                "Dashboard" => CreateViewModel<DashboardViewModel>(),
                "Vacaciones" => CreateViewModel<VacacionesViewModel>(),
                "Empleados" => new EmpleadosViewModel(),
                "Clientes" => new ClientesViewModel(),
                "Viajes" => new ViajesViewModel(),
                "Soporte" => new TurnosSoporteViewModel(),
                "Feriados" => new FeriadosViewModel(),
                "Conflictos" => new ConflictosViewModel(),
                "Reportes" => new ReportesViewModel(),
                _ => CurrentView
            };

            // Inicializar ViewModels que requieren carga de datos
            if (newView is DashboardViewModel dashboardVM)
            {
                CurrentView = dashboardVM;
                dashboardVM.Initialize();
            }
            else if (newView is VacacionesViewModel vacacionesVM)
            {
                CurrentView = vacacionesVM;
                vacacionesVM.Initialize();
            }
            else
            {
                CurrentView = newView;
            }

            Log.Information("Navegando a: {Destino}", destination);
        });
    }

    private T CreateViewModel<T>() where T : ViewModelBase
    {
        try
        {
            return App.GetService<T>();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "No se pudo crear {ViewModelType} con DI, usando constructor por defecto", typeof(T).Name);
            return (T)Activator.CreateInstance(typeof(T))!;
        }
    }
}
