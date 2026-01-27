using System;

namespace OmegaSSH.Models;

public class SshKeyModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty; // Should be encrypted in vault
    public string Type { get; set; } = "Ed25519";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
