using System;
using System.Collections.Generic;

namespace OmegaSSH.Models;

public class SessionModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? PrivateKeyPath { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Folder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastConnected { get; set; }
}
