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
            File.AppendAllText(path, $"[{DateTime.Now}] {message}\r\n");
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
        Log("OnStartup - Starting boot sequence");

        try 
        {
            // 0. Init DI
            InitDI();

            // Global Error Handling
            AppDomain.CurrentDomain.UnhandledException += (s, ev) => HandleFatalError(ev.ExceptionObject as Exception);
            DispatcherUnhandledException += (s, ev) => { HandleFatalError(ev.Exception); ev.Handled = true; };

            // 1. Show Splash Screen (Blocks until it closes)
            Log("OnStartup - Showing Splash");
            var splash = new OmegaSSH.Views.SplashWindow();
            bool? result = splash.ShowDialog();
            Log($"OnStartup - Splash terminated with result: {result}");

            if (result == true)
            {
                // 2. Setup Main Window
                Log("OnStartup - Initializing MainWindow");
                var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                
                // Restore settings
                mainWindow.Width = settingsService.Settings.WindowWidth > 100 ? settingsService.Settings.WindowWidth : 1100;
                mainWindow.Height = settingsService.Settings.WindowHeight > 100 ? settingsService.Settings.WindowHeight : 700;
                
                Log("OnStartup - Displaying MainWindow");
                mainWindow.Show();
                
                // Allow exit when main window closes
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Log("OnStartup - Splash closed without success result, shutting down.");
                Application.Current.Shutdown();
            }
        }
        catch (Exception ex)
        {
            string error = $"CRITICAL BOOT ERROR: {ex.Message}\n{ex.StackTrace}";
            Log(error);
            MessageBox.Show(error, "OmegaSSH - Critical Boot Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    private void HandleFatalError(Exception? ex)
    {
        if (ex == null) return;
        
        // Log to file for debugging the "closing by itself" issue
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmegaSSH", "crash.log");
            File.AppendAllText(logPath, $"\n[{DateTime.Now}] CRASH: {ex.Message}\n{ex.StackTrace}\n");
        }
        catch { }

        OmegaSSH.Views.NotificationWindow.Show($"The system encountered a fatal error:\n{ex.Message}", "SYSTEM FAILURE", true);
        
        // If it's early in startup, we must shutdown
        // Application.Current.Shutdown(); 
    }
}

