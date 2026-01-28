using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class TerminalViewModel : ObservableObject, IDisposable
{
    private readonly ISshService _sshService;
    private readonly ISessionLogger _logger;
    private readonly StringBuilder _outputBuffer = new StringBuilder();
    private readonly System.Timers.Timer _updateTimer;
    
    [ObservableProperty]
    private string _terminalOutput = string.Empty;

    [ObservableProperty]
    private string _sessionName;

    [ObservableProperty]
    private SftpViewModel _sftp;

    public TerminalViewModel(ISshService sshService, ISessionLogger logger, SessionModel session)
    {
        _sshService = sshService;
        _logger = logger;
        _sessionName = session.Name;
        _sftp = new SftpViewModel(sshService);
        _sshService.DataReceived += OnDataReceived;
        
        _logger.Initialize(session.Name);
        
        // Initialize update timer (50ms for smooth updates without freezing)
        _updateTimer = new System.Timers.Timer(50);
        _updateTimer.Elapsed += (s, e) => FlushBuffer();
        _updateTimer.AutoReset = true;
        _updateTimer.Start();

        Connect(session);
    }

    private void FlushBuffer()
    {
        string dataToProcess;
        lock (_outputBuffer)
        {
            if (_outputBuffer.Length == 0) return;
            dataToProcess = _outputBuffer.ToString();
            _outputBuffer.Clear();
        }

        // Notify the view (ANSI Parser)
        DataReceived?.Invoke(dataToProcess);
        
        // Log to file
        _logger.Log(dataToProcess);

        // Update the string property for backup/history (limited)
        App.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (TerminalOutput.Length > 50000) 
                TerminalOutput = TerminalOutput.Substring(25000);
            TerminalOutput += dataToProcess;
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    public event Action<string>? DataReceived;

    private void OnDataReceived(string data)
    {
        lock (_outputBuffer)
        {
            _outputBuffer.Append(data);
        }
    }

    private async void Connect(SessionModel session)
    {
        try
        {
            lock (_outputBuffer) { _outputBuffer.Append($"Connecting to {session.Host}...\n"); }
            await _sshService.ConnectAsync(session);
            lock (_outputBuffer) { _outputBuffer.Append("Connected.\n"); }
            await Sftp.RefreshAsync();
        }
        catch (Exception ex)
        {
            lock (_outputBuffer) { _outputBuffer.Append($"\nConnection Error: {ex.Message}\n"); }
        }
    }

    [RelayCommand]
    private async Task SendInput(string input)
    {
        await _sshService.SendCommandAsync(input);
    }

    public void Dispose()
    {
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        _logger?.Dispose();
        _sshService.DataReceived -= OnDataReceived;
        _sshService.Dispose();
    }
}
