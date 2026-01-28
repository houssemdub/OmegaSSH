using System;
using System.Windows;
using System.Windows.Media;

namespace OmegaSSH.Services;

public interface IThemeService
{
    void SetTheme(string themeName);
    string CurrentTheme { get; }
}

public class ThemeService : IThemeService
{
    public string CurrentTheme { get; private set; } = "default";

    public void SetTheme(string themeName)
    {
        var app = Application.Current;
        if (app == null) return;

        var resources = app.Resources;
        
        CurrentTheme = themeName.ToLower();

        switch (CurrentTheme)
        {
            case "dracula":
                ApplyTheme(resources, "#282A36", "#44475A", "#BD93F9", "#F8F8F2", "#6272A4", "#FF79C6", "#50FA7B", "#9BA2B2");
                break;
            case "nord":
                ApplyTheme(resources, "#2E3440", "#3B4252", "#88C0D0", "#ECEFF4", "#4C566A", "#81A1C1", "#A3BE8C", "#949FB5");
                break;
            case "monokai":
                ApplyTheme(resources, "#272822", "#3E3D32", "#F92672", "#F8F8F2", "#75715E", "#66D9EF", "#A6E22E", "#A4A089");
                break;
            case "cyberneon":
                ApplyTheme(resources, "#0A0E1A", "#1A1F35", "#00FFFF", "#E0E0E0", "#4A5568", "#FF00FF", "#00FF00", "#718096");
                break;
            case "solarized":
                ApplyTheme(resources, "#002B36", "#073642", "#268BD2", "#FDF6E3", "#586E75", "#2AA198", "#859900", "#93A1A1");
                break;
            case "retro":
                ApplyTheme(resources, "#000000", "#1A1A1A", "#00FF00", "#00FF00", "#006600", "#00FF00", "#FFFF00", "#00AA00");
                break;
            default: // "default"
                ApplyTheme(resources, "#0D1117", "#161B22", "#58A6FF", "#C9D1D9", "#30363D", "#FF00E5", "#FDF500", "#8B949E");
                break;
        }
    }

    private void ApplyTheme(ResourceDictionary resources, string bg, string sidebar, string primary, 
        string text, string border, string secondary, string highlight, string? dimText = null)
    {
        var bgColor = (Color)ColorConverter.ConvertFromString(bg);
        var sidebarColor = (Color)ColorConverter.ConvertFromString(sidebar);
        var primaryColor = (Color)ColorConverter.ConvertFromString(primary);
        var textColor = (Color)ColorConverter.ConvertFromString(text);
        var borderColor = (Color)ColorConverter.ConvertFromString(border);
        var secondaryColor = (Color)ColorConverter.ConvertFromString(secondary);
        var highlightColor = (Color)ColorConverter.ConvertFromString(highlight);
        var dtColor = (Color)ColorConverter.ConvertFromString(dimText ?? border);

        resources["BgBrush"] = new SolidColorBrush(bgColor);
        resources["SidebarBrush"] = new SolidColorBrush(sidebarColor);
        resources["PrimaryAccentBrush"] = new SolidColorBrush(primaryColor);
        resources["AccentBrush"] = new SolidColorBrush(primaryColor);
        resources["TextBrush"] = new SolidColorBrush(textColor);
        resources["BorderBrush"] = new SolidColorBrush(borderColor);
        resources["SecondaryAccentBrush"] = new SolidColorBrush(secondaryColor);
        resources["HighlightAccentBrush"] = new SolidColorBrush(highlightColor);
        resources["TextDimBrush"] = new SolidColorBrush(dtColor);

        // Surface is usually a mix of BG and Sidebar or slightly lighter Sidebar
        var surfaceColor = Color.FromArgb(255, 
            (byte)Math.Min(255, sidebarColor.R + 10), 
            (byte)Math.Min(255, sidebarColor.G + 10), 
            (byte)Math.Min(255, sidebarColor.B + 10));
        resources["SurfaceBrush"] = new SolidColorBrush(surfaceColor);
        resources["SurfaceColor"] = surfaceColor;

        // Gradients
        resources["PrimaryGradient"] = new LinearGradientBrush(primaryColor, Color.FromArgb(255, (byte)(primaryColor.R*0.8), (byte)(primaryColor.G*0.8), (byte)(primaryColor.B*0.8)), 45);
        resources["SecondaryGradient"] = new LinearGradientBrush(secondaryColor, Color.FromArgb(255, (byte)(secondaryColor.R*0.8), (byte)(secondaryColor.G*0.8), (byte)(secondaryColor.B*0.8)), 45);
        
        var sidebarGrad = new LinearGradientBrush();
        sidebarGrad.StartPoint = new Point(0, 0);
        sidebarGrad.EndPoint = new Point(0, 1);
        sidebarGrad.GradientStops.Add(new GradientStop(bgColor, 0));
        sidebarGrad.GradientStops.Add(new GradientStop(sidebarColor, 1));
        resources["SidebarGradient"] = sidebarGrad;

        var glow = new RadialGradientBrush();
        glow.GradientStops.Add(new GradientStop(Color.FromArgb(68, primaryColor.R, primaryColor.G, primaryColor.B), 0));
        glow.GradientStops.Add(new GradientStop(Color.FromArgb(0, primaryColor.R, primaryColor.G, primaryColor.B), 1));
        resources["AccentGlowBrush"] = glow;

        // Colors for triggers
        resources["BgColor"] = bgColor;
        resources["SidebarColor"] = sidebarColor;
        resources["AccentColor"] = primaryColor;
        resources["TextColor"] = textColor;
        resources["BorderColor"] = borderColor;
    }
}
