using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSSH.Services;

public interface ISessionLogger : IDisposable
{
    void Initialize(string sessionName);
    void Log(string data);
}

public class SessionLogger : ISessionLogger
{
    private StreamWriter? _writer;
    private readonly string _logBaseDir;

    public SessionLogger()
    {
        // Use the application's base directory for relative "./log"
        _logBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        if (!Directory.Exists(_logBaseDir))
        {
            Directory.CreateDirectory(_logBaseDir);
        }
    }

    public void Initialize(string sessionName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string safeName = string.Join("_", sessionName.Split(Path.GetInvalidFileNameChars()));
        string fileName = $"session_{safeName}_{timestamp}.log";
        string filePath = Path.Combine(_logBaseDir, fileName);

        _writer = new StreamWriter(filePath, true, Encoding.UTF8) { AutoFlush = true };
        _writer.WriteLine($"=== Session Started: {sessionName} ({DateTime.Now}) ===");
    }

    public void Log(string data)
    {
        if (_writer == null) return;
        
        // Remove ANSI codes if we want a clean log, or keep them? 
        // User said "log its sessions", usually keeping ANSI is okay for raw logs, 
        // but stripping them makes it readable in Notepad.
        // For now, let's keep it simple and log raw data.
        _writer.Write(data);
    }

    public void Dispose()
    {
        if (_writer != null)
        {
            _writer.WriteLine();
            _writer.WriteLine($"=== Session Ended: {DateTime.Now} ===");
            _writer.Dispose();
            _writer = null;
        }
    }
}
