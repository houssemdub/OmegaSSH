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

public class LocalTerminalService : ILocalTerminalService, IDisposable
{
    private Process? _process;
    public event Action<string>? DataReceived;

    public void Start()
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            }
        };

        _process.OutputDataReceived += (s, e) => { if (e.Data != null) DataReceived?.Invoke(e.Data + "\n"); };
        _process.ErrorDataReceived += (s, e) => { if (e.Data != null) DataReceived?.Invoke(e.Data + "\n"); };

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    public void Write(string input)
    {
        _process?.StandardInput.WriteLine(input);
    }

    public void Stop()
    {
        try { _process?.Kill(); } catch { }
        _process?.Dispose();
    }

    public void Dispose()
    {
        Stop();
    }
}
