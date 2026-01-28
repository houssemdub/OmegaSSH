using System;

namespace OmegaSSH.Models;

public class AppSettingsModel
{
    public string Theme { get; set; } = "default";
    public bool AutoConnect { get; set; } = false;
    public string LastUsedUser { get; set; } = string.Empty;
    
    // Terminal Settings
    public string TerminalFontFamily { get; set; } = "Consolas";
    public double TerminalFontSize { get; set; } = 14;
    public bool EnableAnsiColors { get; set; } = true;
    
    // Connection Settings
    public int KeepAliveInterval { get; set; } = 30; // seconds
    public string SessionLogPath { get; set; } = "log";
    
    public double WindowWidth { get; set; } = 1100;
    public double WindowHeight { get; set; } = 700;
}
