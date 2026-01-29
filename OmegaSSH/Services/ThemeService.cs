using System;
using System.Windows;
using System.Windows.Media;

namespace OmegaSSH.Services;

public interface IThemeService
{
    void ApplyDefaultTheme();
    string CurrentTheme { get; }
}

public class ThemeService : IThemeService
{
    public string CurrentTheme => "OmegaDark";

    public void ApplyDefaultTheme()
    {
        var app = Application.Current;
        if (app == null) return;

        var resources = app.Resources;
        
        // OMEGA DARK: Premium Professional Slate
        ApplyTheme(resources, 
            bg: "#0A0B10",       // Deep Slate Background
            sidebar: "#12141D",  // Slightly Lighter Sidebar
            primary: "#58A6FF",  // Modern Blue Accent
            text: "#FFFFFF",     // Pure White Text
            border: "#1F2232",   // Dark Subtle Border
            secondary: "#79C0FF", 
            highlight: "#58A6FF", 
            dimText: "#8B949E", 
            cornerRadius: 4, 
            fontFamily: "Inter, Segoe UI");
    }

    private void ApplyTheme(ResourceDictionary resources, string bg, string sidebar, string primary, 
        string text, string border, string secondary, string highlight, string? dimText = null, double cornerRadius = 8, string fontFamily = "Segoe UI")
    {
        var bgColor = (Color)ColorConverter.ConvertFromString(bg);
        var sidebarColor = (Color)ColorConverter.ConvertFromString(sidebar);
        var primaryColor = (Color)ColorConverter.ConvertFromString(primary);
        var textColor = (Color)ColorConverter.ConvertFromString(text);
        var borderColor = (Color)ColorConverter.ConvertFromString(border);
        var secondaryColor = (Color)ColorConverter.ConvertFromString(secondary);
        var highlightColor = (Color)ColorConverter.ConvertFromString(highlight);
        var dimTextColor = (Color)ColorConverter.ConvertFromString(dimText ?? "#8B949E");

        resources["BgBrush"] = CreateFrozenBrush(bgColor);
        resources["SidebarBrush"] = CreateFrozenBrush(sidebarColor);
        resources["SurfaceBrush"] = CreateFrozenBrush(sidebarColor);
        resources["PrimaryAccentBrush"] = CreateFrozenBrush(primaryColor);
        resources["SecondaryAccentBrush"] = CreateFrozenBrush(secondaryColor);
        resources["HighlightAccentBrush"] = CreateFrozenBrush(highlightColor);
        resources["BorderBrush"] = CreateFrozenBrush(borderColor);
        resources["TextBrush"] = CreateFrozenBrush(textColor);
        resources["TextDimBrush"] = CreateFrozenBrush(dimTextColor);

        resources["MainCornerRadius"] = new CornerRadius(cornerRadius);
        resources["MainFontFamily"] = new FontFamily(fontFamily);
        resources["TitleBarBrush"] = CreateFrozenBrush(sidebarColor);

        var primaryGradient = new LinearGradientBrush();
        primaryGradient.StartPoint = new Point(0, 0);
        primaryGradient.EndPoint = new Point(1, 1);
        primaryGradient.GradientStops.Add(new GradientStop(primaryColor, 0));
        primaryGradient.GradientStops.Add(new GradientStop(Color.FromRgb((byte)Math.Max(0, primaryColor.R - 40), (byte)Math.Max(0, primaryColor.G - 40), (byte)Math.Max(0, primaryColor.B - 40)), 1));
        primaryGradient.Freeze();
        resources["PrimaryGradient"] = primaryGradient;
        
        var sidebarGradient = new LinearGradientBrush();
        sidebarGradient.StartPoint = new Point(0, 0);
        sidebarGradient.EndPoint = new Point(0, 1);
        sidebarGradient.GradientStops.Add(new GradientStop(bgColor, 0));
        sidebarGradient.GradientStops.Add(new GradientStop(sidebarColor, 1));
        sidebarGradient.Freeze();
        resources["SidebarGradient"] = sidebarGradient;

        var glow = new RadialGradientBrush();
        glow.GradientStops.Add(new GradientStop(Color.FromArgb(68, primaryColor.R, primaryColor.G, primaryColor.B), 0));
        glow.GradientStops.Add(new GradientStop(Color.FromArgb(0, primaryColor.R, primaryColor.G, primaryColor.B), 1));
        glow.Freeze();
        resources["AccentGlowBrush"] = glow;

        // Glass Brushes
        var glass = new SolidColorBrush(Color.FromArgb(15, 255, 255, 255));
        glass.Freeze();
        resources["GlassBrush"] = glass;

        var glassBorder = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255));
        glassBorder.Freeze();
        resources["GlassBorderBrush"] = glassBorder;

        // Colors for triggers
        resources["AccentColor"] = primaryColor;
        resources["BgColor"] = bgColor;
        resources["SidebarColor"] = sidebarColor;
        resources["TextColor"] = textColor;
        resources["BorderColor"] = borderColor;
        resources["HighlightColor"] = highlightColor;
        resources["SurfaceColor"] = sidebarColor;
    }

    private SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    // Compatibility method for existing calls
    public void SetTheme(string themeName) => ApplyDefaultTheme();
}
