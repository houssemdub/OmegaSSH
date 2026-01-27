using System;

namespace OmegaSSH.Models;

public class AppSettingsModel
{
    public string Theme { get; set; } = "cyberpunk";
    public bool AutoConnect { get; set; } = false;
    public string LastUsedUser { get; set; } = string.Empty;
    public double WindowWidth { get; set; } = 1100;
    public double WindowHeight { get; set; } = 700;
}
