using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OmegaSSH.ViewModels;
using OmegaSSH.Services;
using System;
using System.IO;

namespace OmegaSSH;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        // Prevent shutdown when splash closes
        this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
    }

    private void Log(string message)
    {
        try
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmegaSSH");
            if (!Directory.Exists(appData)) Directory.CreateDirectory(appData);
            string path = Path.Combine(appData, "boot.log");
            File.AppendAllText(path, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\r\n");
        }
        catch { }
    }

    private void InitDI()
    {
        Log("InitDI - Building ServiceProvider");
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // ViewModels
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>(s => new MainWindow
        {
            DataContext = s.GetRequiredService<MainViewModel>()
        });

        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IVaultService, VaultService>();
        services.AddTransient<ISshService, SshService>();
        services.AddTransient<ILocalTerminalService, LocalTerminalService>();
        services.AddSingleton<ISnippetService, SnippetService>();
        services.AddSingleton<IKeyService, KeyService>();
        services.AddSingleton<IThemeService, ThemeService>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Log("===== BOOT SEQUENCE START =====");

        // Global Error Handling - The "Black Box"
        AppDomain.CurrentDomain.UnhandledException += (s, ev) => {
            Log("!!! FATAL DOMAIN EXCEPTION !!!");
            HandleFatalError(ev.ExceptionObject as Exception);
        };
        DispatcherUnhandledException += (s, ev) => {
            Log("!!! UI DISPATCHER EXCEPTION !!!");
            HandleFatalError(ev.Exception);
            ev.Handled = true;
        };
        TaskScheduler.UnobservedTaskException += (s, ev) => {
            Log("!!! UNOBSERVED TASK EXCEPTION !!!");
            HandleFatalError(ev.Exception);
            ev.SetObserved();
        };

        try 
        {
            Log("Step 0: Init DI");
            InitDI();

            Log("Step 1: Showing Splash");
            var splash = new OmegaSSH.Views.SplashWindow();
            bool? result = splash.ShowDialog();
            Log($"Step 1 Complete: Splash result = {result}");

            if (result == true)
            {
                Log("Step 2: Initializing MainWindow");
                var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                
                Log("Step 3: Loading Window Dimensions");
                mainWindow.Width = settingsService.Settings.WindowWidth > 100 ? settingsService.Settings.WindowWidth : 1100;
                mainWindow.Height = settingsService.Settings.WindowHeight > 100 ? settingsService.Settings.WindowHeight : 700;
                
                Log("Step 4: Executing MainWindow.Show()");
                mainWindow.Show();
                
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Log("===== BOOT SEQUENCE SUCCESS =====");
            }
            else
            {
                Log("Boot Cancelled: Splash closed without success.");
                Application.Current.Shutdown();
            }
        }
        catch (Exception ex)
        {
            Log($"!!! BOOT CRASH !!! {ex.Message}\n{ex.StackTrace}");
            HandleFatalError(ex);
        }
    }

    private void HandleFatalError(Exception? ex)
    {
        if (ex == null) return;
        
        string errorLog = $"TIME: {DateTime.Now}\nMESSAGE: {ex.Message}\nSTACK: {ex.StackTrace}\n";
        if (ex.InnerException != null)
        {
            errorLog += $"INNER: {ex.InnerException.Message}\nINNER STACK: {ex.InnerException.StackTrace}\n";
        }
        
        Log($"FATAL ERROR LOGGED:\n{errorLog}");

        // Log to crash.log as well
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmegaSSH", "crash.log");
            File.AppendAllText(logPath, $"[{DateTime.Now}] FATAL: {ex.Message}\n{ex.StackTrace}\n");
        }
        catch { }

        // Using standard MessageBox here because NotificationWindow might fail if UI is broken
        MessageBox.Show(
            $"OMEGASSH CRITICAL ERROR\n\n" +
            $"The app will now close to prevent data corruption.\n\n" +
            $"Error: {ex.Message}\n\n" +
            $"Check %AppData%\\OmegaSSH\\boot.log for details.", 
            "Neural Link Failure", 
            MessageBoxButton.OK, 
            MessageBoxImage.Error);

        Application.Current.Shutdown(); 
    }
}

