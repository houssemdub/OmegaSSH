using System;
using System.Windows;
using System.Windows.Media;

namespace OmegaSSH.Services;

public interface IThemeService
{
    void SetTheme(string themeName);
}

public class ThemeService : IThemeService
{
    public void SetTheme(string themeName)
    {
        var app = Application.Current;
        var colors = app.Resources.MergedDictionaries[0]; // Assuming Colors.xaml is the first

        switch (themeName.ToLower())
        {
            case "default":
                colors["BgBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D1117"));
                colors["PrimaryAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFF"));
                break;
            case "retro":
                colors["BgBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212"));
                colors["PrimaryAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                break;
            case "nord":
                colors["BgBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E3440"));
                colors["PrimaryAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88C0D0"));
                break;
        }
    }
}
