using System;

namespace OmegaSSH.Models
{
    public enum PortForwardType
    {
        Local,
        Remote,
        Dynamic
    }

    public class PortForwardModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Tunnel";
        public PortForwardType Type { get; set; } = PortForwardType.Local;
        public string LocalHost { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; }
        public string RemoteHost { get; set; } = "localhost";
        public int RemotePort { get; set; }
        public bool IsActive { get; set; }
    }
}
