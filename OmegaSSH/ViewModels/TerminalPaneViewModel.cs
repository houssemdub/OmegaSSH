using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class TerminalPaneViewModel : ObservableObject, IDisposable
{
    public IShellEngine Engine => _engine;
    private readonly IShellEngine _engine;
    private readonly ISessionLogger _logger;
    private readonly SessionModel? _session;
    private readonly StringBuilder _outputBuffer = new StringBuilder();
    private readonly System.Timers.Timer _updateTimer;
    private int _reconnectAttempts = 0;
    private const int MaxReconnectAttempts = 5;

    [ObservableProperty]
    private string _terminalOutput = string.Empty;

    public TerminalPaneViewModel(IShellEngine engine, ISessionLogger logger, SessionModel? session = null)
    {
        _engine = engine;
        _logger = logger;
        _session = session;
        _engine.DataReceived += OnDataReceived;
        _engine.Disconnected += OnDisconnected;

        _logger.Initialize(session?.Name ?? "Local Terminal");

        _updateTimer = new System.Timers.Timer(50);
        _updateTimer.Elapsed += (s, e) => FlushBuffer();
        _updateTimer.AutoReset = true;
        _updateTimer.Start();

        Initialize();
    }

    private void OnDisconnected()
    {
        if (_session != null && _reconnectAttempts < MaxReconnectAttempts)
        {
            _reconnectAttempts++;
            lock (_outputBuffer) { _outputBuffer.Append($"\r\n[SYSTEM] Disconnected. Reconnecting (Attempt {_reconnectAttempts}/{MaxReconnectAttempts})...\r\n"); }
            Task.Delay(3000).ContinueWith(_ => Initialize());
        }
        else
        {
            lock (_outputBuffer) { _outputBuffer.Append("\r\n[SYSTEM] Session ended.\r\n"); }
        }
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

        DataReceived?.Invoke(dataToProcess);
        _logger.Log(dataToProcess);

        App.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (TerminalOutput.Length > 100000) 
                TerminalOutput = TerminalOutput.Substring(50000);
            TerminalOutput += dataToProcess;
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    public event Action<string>? DataReceived;
    public event Action<int>? SearchResultFound;

    private void OnDataReceived(string data)
    {
        lock (_outputBuffer)
        {
            _outputBuffer.Append(data);
        }
    }

    public void Search(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        // Simple search in the visible buffer
        var index = TerminalOutput.LastIndexOf(text, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            SearchResultFound?.Invoke(index);
        }
    }

    private async void Initialize()
    {
        try
        {
            if (_session != null && _engine is ISshService ssh)
            {
                lock (_outputBuffer) { _outputBuffer.Append($"Connecting to {_session.Host}...\n"); }
                await ssh.ConnectAsync(_session);
            }
            else
            {
                await _engine.ConnectAsync();
            }
            _reconnectAttempts = 0;
        }
        catch (Exception ex)
        {
            lock (_outputBuffer) { _outputBuffer.Append($"\nError: {ex.Message}\n"); }
        }
    }

    [RelayCommand]
    private async Task SendInput(string input)
    {
        await _engine.WriteAsync(input);
    }

    [RelayCommand]
    private async Task Interrupt()
    {
        // Send Ctrl+C (ETX - End of Text) signal
        await _engine.WriteAsync("\x03");
    }

    public void Dispose()
    {
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        _logger?.Dispose();
        _engine.DataReceived -= OnDataReceived;
        _engine.Disconnected -= OnDisconnected;
        _engine.Dispose();
    }
}
