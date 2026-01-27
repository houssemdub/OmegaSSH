using System;
using System.Threading.Tasks;

namespace OmegaSSH.Services;

public interface IShellEngine : IDisposable
{
    Task ConnectAsync();
    Task DisconnectAsync();
    Task WriteAsync(string input);
    event Action<string>? DataReceived;
}
