using System;

namespace OmegaSSH.Models;

public class TunnelModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int LocalPort { get; set; }
    public string RemoteHost { get; set; } = "localhost";
    public int RemotePort { get; set; }
    public bool IsEnabled { get; set; }
}
