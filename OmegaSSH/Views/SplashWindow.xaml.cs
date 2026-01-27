using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace OmegaSSH.Views;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
        this.Loaded += (s, e) => RunInitialization();
    }

    private async void RunInitialization()
    {
        try
        {
            SetStatus("Loading configuration...", 20);
            var settingsService = App.Current.ServiceProvider.GetRequiredService<OmegaSSH.Services.ISettingsService>();
            await settingsService.LoadSettingsAsync();
            await Task.Delay(300);

            SetStatus("Checking filesystem...", 40);
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmegaSSH");
            if (!Directory.Exists(appData)) Directory.CreateDirectory(appData);
            await Task.Delay(300);

            SetStatus("Applying visual theme...", 60);
            var themeService = App.Current.ServiceProvider.GetRequiredService<OmegaSSH.Services.IThemeService>();
            themeService.SetTheme(settingsService.Settings.Theme);
            await Task.Delay(300);

            SetStatus("Initializing UI Engine...", 80);
            await Task.Delay(400);

            SetStatus("Ready.", 100);
            await Task.Delay(200);

            this.DialogResult = true;
            this.Close();
        }
        catch (Exception ex)
        {
            // Log the error
            try {
                File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmegaSSH", "boot.log"), $"SPLASH ERROR: {ex.Message}\n");
            } catch {}
            
            MessageBox.Show($"Boot failure: {ex.Message}", "OmegaSSH", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    private void SetStatus(string msg, double progress)
    {
        StatusLabel.Text = msg.ToUpper();
        LoadProgress.Value = progress;
    }
}
