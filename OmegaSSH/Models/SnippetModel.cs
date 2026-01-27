using System;

namespace OmegaSSH.Models;

public class SnippetModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string? Description { get; set; }
}
