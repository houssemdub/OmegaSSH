using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface ISettingsService
{
    AppSettingsModel Settings { get; }
    Task LoadSettingsAsync();
    Task SaveSettingsAsync();
}

public class SettingsService : ISettingsService
{
    private readonly string _filePath;
    public AppSettingsModel Settings { get; private set; } = new();

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "OmegaSSH");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "settings.json");
    }

    public async Task LoadSettingsAsync()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                Settings = JsonSerializer.Deserialize<AppSettingsModel>(json) ?? new AppSettingsModel();
            }
            catch
            {
                Settings = new AppSettingsModel();
            }
        }
    }

    public async Task SaveSettingsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch { }
    }
}
