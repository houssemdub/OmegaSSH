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
            case "mobaxtrem":
                // Dark admin style, sharp corners
                ApplyTheme(resources, "#000000", "#1C1C1C", "#00A2E8", "#FFFFFF", "#333333", "#22B14C", "#FFC90E", "#A0A0A0", 2, "Segoe UI");
                break;
            case "github":
                // Clean GitHub Dark
                ApplyTheme(resources, "#0D1117", "#010409", "#2F81F7", "#C9D1D9", "#30363D", "#79C0FF", "#D29922", "#8B949E", 6, "Segoe UI");
                break;
            case "winxp":
                // Classic Luna Blue
                ApplyTheme(resources, "#ECE9D8", "#D6DFF7", "#0055E1", "#000000", "#003399", "#3B913B", "#FF8C00", "#4B4B4B", 0, "Tahoma");
                break;
            case "omegassh":
            case "default":
                // Futuristic Cyberpunk
                ApplyTheme(resources, "#0D1117", "#090C10", "#58A6FF", "#C9D1D9", "#30363D", "#FF00E5", "#FDF500", "#8B949E", 12, "Segoe UI");
                break;
            case "dracula":
                ApplyTheme(resources, "#282A36", "#44475A", "#BD93F9", "#F8F8F2", "#6272A4", "#FF79C6", "#50FA7B", "#9BA2B2", 10, "Segoe UI");
                break;
            case "nord":
                ApplyTheme(resources, "#2E3440", "#3B4252", "#88C0D0", "#D8DEE9", "#4C566A", "#81A1C1", "#EBCB8B", "#AAB0BE", 8, "Segoe UI");
                break;
            case "monokai":
                ApplyTheme(resources, "#272822", "#3E3D32", "#F92672", "#F8F8F2", "#75715E", "#66D9EF", "#A6E22E", "#A4A089", 8); // Added default corner radius
                break;
            case "cyberneon":
                ApplyTheme(resources, "#050505", "#0A0E1A", "#00F2FF", "#FFFFFF", "#112244", "#FF00E5", "#FDF500", "#88CCFF", 15, "Segoe UI");
                break;
            case "solarized":
                ApplyTheme(resources, "#002B36", "#073642", "#268BD2", "#FDF6E3", "#586E75", "#2AA198", "#859900", "#93A1A1", 8); // Added default corner radius
                break;
            case "retro":
                ApplyTheme(resources, "#000000", "#1A1A1A", "#00FF00", "#00FF00", "#006600", "#00FF00", "#FFFF00", "#00AA00", 4); // Added default corner radius
                break;
            default:
                ApplyTheme(resources, "#0D1117", "#161B22", "#58A6FF", "#C9D1D9", "#30363D", "#FF00E5", "#FDF500", "#8B949E", 12);
                break;
        }
    }

    private void ApplyTheme(ResourceDictionary resources, string bg, string sidebar, string primary, 
        string text, string border, string secondary, string highlight, string? dimText = null, double cornerRadius = 12, string fontFamily = "Segoe UI")
    {
        var bgColor = (Color)ColorConverter.ConvertFromString(bg);
        var sidebarColor = (Color)ColorConverter.ConvertFromString(sidebar);
        var primaryColor = (Color)ColorConverter.ConvertFromString(primary);
        var textColor = (Color)ColorConverter.ConvertFromString(text);
        var borderColor = (Color)ColorConverter.ConvertFromString(border);
        var secondaryColor = (Color)ColorConverter.ConvertFromString(secondary);
        var highlightColor = (Color)ColorConverter.ConvertFromString(highlight);
        var dimTextColor = (Color)ColorConverter.ConvertFromString(dimText ?? "#888888");

        resources["BgBrush"] = CreateFrozenBrush(bgColor);
        resources["SidebarBrush"] = CreateFrozenBrush(sidebarColor);
        resources["SurfaceBrush"] = CreateFrozenBrush(sidebarColor); // Reuse sidebar for surface
        resources["PrimaryAccentBrush"] = CreateFrozenBrush(primaryColor);
        resources["SecondaryAccentBrush"] = CreateFrozenBrush(secondaryColor);
        resources["HighlightAccentBrush"] = CreateFrozenBrush(highlightColor);
        resources["BorderBrush"] = CreateFrozenBrush(borderColor);
        resources["TextBrush"] = CreateFrozenBrush(textColor);
        resources["TextDimBrush"] = CreateFrozenBrush(dimTextColor);

        // UI Shape & Typography
        resources["MainCornerRadius"] = new CornerRadius(cornerRadius);
        resources["MainFontFamily"] = new FontFamily(fontFamily);

        // Title Bar Brush (Specific for XP and futuristic themes)
        if (CurrentTheme == "winxp")
        {
            var titleGrad = new LinearGradientBrush();
            titleGrad.StartPoint = new Point(0, 0);
            titleGrad.EndPoint = new Point(1, 0);
            titleGrad.GradientStops.Add(new GradientStop(Color.FromRgb(0, 88, 238), 0.0));
            titleGrad.GradientStops.Add(new GradientStop(Color.FromRgb(55, 128, 238), 0.5));
            titleGrad.GradientStops.Add(new GradientStop(Color.FromRgb(0, 88, 238), 1.0));
            titleGrad.Freeze();
            resources["TitleBarBrush"] = titleGrad;
        }
        else if (CurrentTheme == "mobaxtrem")
        {
            resources["TitleBarBrush"] = CreateFrozenBrush(Color.FromRgb(44, 44, 44));
        }
        else
        {
            resources["TitleBarBrush"] = CreateFrozenBrush(sidebarColor);
        }

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
        var glass = new LinearGradientBrush();
        glass.StartPoint = new Point(0, 0);
        glass.EndPoint = new Point(1, 1);
        glass.GradientStops.Add(new GradientStop(Color.FromArgb(26, 255, 255, 255), 0));
        glass.GradientStops.Add(new GradientStop(Color.FromArgb(8, 255, 255, 255), 1));
        glass.Freeze();
        resources["GlassBrush"] = glass;

        var glassBorder = new LinearGradientBrush();
        glassBorder.StartPoint = new Point(0, 0);
        glassBorder.EndPoint = new Point(1, 1);
        glassBorder.GradientStops.Add(new GradientStop(Color.FromArgb(51, 255, 255, 255), 0));
        glassBorder.GradientStops.Add(new GradientStop(Color.FromArgb(17, 255, 255, 255), 1));
        glassBorder.Freeze();
        resources["GlassBorderBrush"] = glassBorder;

        // Colors for triggers
        resources["AccentColor"] = primaryColor;
        resources["BgColor"] = bgColor;
        resources["SidebarColor"] = sidebarColor;
        resources["TextColor"] = textColor;
        resources["BorderColor"] = borderColor;
        resources["HighlightColor"] = highlightColor;
        resources["SurfaceColor"] = sidebarColor; // Kept as it was not explicitly removed by the diff
    }

    private SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
