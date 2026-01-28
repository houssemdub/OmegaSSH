using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSSH.Services;

public interface ILocalTerminalService
{
    void Start();
    void Stop();
    void Write(string input);
    event Action<string>? DataReceived;
}

public class LocalTerminalService : IShellEngine
{
    private Process? _process;
    private readonly string _shellType;
    public event Action<string>? DataReceived;
    public event Action? Disconnected;

    public LocalTerminalService(string shellType = "powershell.exe")
    {
        _shellType = shellType;
    }

    public Task ConnectAsync()
    {
        return Task.Run(() =>
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _shellType,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            _process.OutputDataReceived += (s, e) => { if (e.Data != null) DataReceived?.Invoke(e.Data + "\r\n"); };
            _process.ErrorDataReceived += (s, e) => { if (e.Data != null) DataReceived?.Invoke(e.Data + "\r\n"); };
            _process.Exited += (s, e) => Disconnected?.Invoke();
            _process.EnableRaisingEvents = true;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            
            DataReceived?.Invoke($"[LOCAL] Started {_shellType}...\r\n");
        });
    }

    public Task WriteAsync(string input)
    {
        if (_process != null && !_process.HasExited)
        {
            _process.StandardInput.WriteLine(input);
        }
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        Stop();
        return Task.CompletedTask;
    }

    public void Stop()
    {
        try { if (_process != null && !_process.HasExited) _process.Kill(); } catch { }
        _process?.Dispose();
        _process = null;
    }

    public void Dispose()
    {
        Stop();
    }
}
