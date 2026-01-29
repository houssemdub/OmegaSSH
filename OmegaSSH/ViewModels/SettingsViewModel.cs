using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;

    [ObservableProperty] private bool _autoConnect;
    [ObservableProperty] private string _terminalFontFamily;
    [ObservableProperty] private double _terminalFontSize;
    [ObservableProperty] private bool _enableAnsiColors;
    [ObservableProperty] private int _keepAliveInterval;
    [ObservableProperty] private string _sessionLogPath;

    public SettingsViewModel(ISettingsService settingsService, IThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;

        var s = _settingsService.Settings;
        _autoConnect = s.AutoConnect;
        _terminalFontFamily = s.TerminalFontFamily;
        _terminalFontSize = s.TerminalFontSize;
        _enableAnsiColors = s.EnableAnsiColors;
        _keepAliveInterval = s.KeepAliveInterval;
        _sessionLogPath = s.SessionLogPath;
    }

    [RelayCommand]
    private async Task Save(Window window)
    {
        var s = _settingsService.Settings;
        s.AutoConnect = AutoConnect;
        s.TerminalFontFamily = TerminalFontFamily;
        s.TerminalFontSize = TerminalFontSize;
        s.EnableAnsiColors = EnableAnsiColors;
        s.KeepAliveInterval = KeepAliveInterval;
        s.SessionLogPath = SessionLogPath;

        await _settingsService.SaveSettingsAsync();
        
        window.DialogResult = true;
        window.Close();
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.DialogResult = false;
        window.Close();
    }

    [RelayCommand]
    private void BrowseLogPath()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Select Session Log Directory",
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            SessionLogPath = dialog.FolderName;
        }
    }
}
