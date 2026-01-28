using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class TerminalViewModel : ObservableObject, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingsService _settingsService;
    private readonly SessionModel _session;

    [ObservableProperty]
    private string _sessionName;

    [ObservableProperty]
    private SftpViewModel? _sftp;

    [ObservableProperty] private string _terminalFontFamily;
    [ObservableProperty] private double _terminalFontSize;

    [ObservableProperty]
    private ObservableCollection<TerminalPaneViewModel> _panes = new();

    [ObservableProperty]
    private TerminalPaneViewModel? _activePane;

    [ObservableProperty]
    private bool _isSplit;

    [ObservableProperty]
    private bool _isVerticalSplit = true;

    public TerminalViewModel(IShellEngine engine, ISessionLogger logger, ISettingsService settingsService, IServiceProvider serviceProvider, SessionModel session)
    {
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _session = session;
        _sessionName = session.Name;
        
        // The first pane uses the injected engine
        var firstPane = new TerminalPaneViewModel(engine, logger, session);
        Panes.Add(firstPane);
        ActivePane = firstPane;

        if (engine is ISshService ssh)
        {
            _sftp = new SftpViewModel(ssh);
        }
        else
        {
            // Placeholder or null for local sessions
            // _sftp = null; // Need to make Sftp nullable in property if so
        }

        _terminalFontFamily = settingsService.Settings.TerminalFontFamily;
        _terminalFontSize = settingsService.Settings.TerminalFontSize;
    }

    [RelayCommand]
    private void SplitVertical()
    {
        Split(true);
    }

    [RelayCommand]
    private void SplitHorizontal()
    {
        Split(false);
    }

    private void Split(bool vertical)
    {
        if (Panes.Count >= 4) return; // Limit for demo

        IShellEngine newEngine;
        var logger = _serviceProvider.GetRequiredService<ISessionLogger>();

        if (_session.Name.StartsWith("local:"))
        {
            var shell = _session.Name.Split(':')[1];
            newEngine = new LocalTerminalService(shell + ".exe");
        }
        else
        {
            newEngine = _serviceProvider.GetRequiredService<ISshService>();
        }
        
        var newPane = new TerminalPaneViewModel(newEngine, logger, _session);
        
        Panes.Add(newPane);
        ActivePane = newPane;
        IsSplit = true;
        IsVerticalSplit = vertical;
    }

    [RelayCommand]
    private void ClosePane(TerminalPaneViewModel? pane)
    {
        if (pane == null) pane = ActivePane;
        if (pane == null || Panes.Count <= 1) return;

        Panes.Remove(pane);
        pane.Dispose();
        ActivePane = Panes.LastOrDefault();
        if (Panes.Count <= 1) IsSplit = false;
    }

    [RelayCommand]
    private async Task SendInput(string input)
    {
        if (ActivePane != null)
        {
            await ActivePane.SendInputCommand.ExecuteAsync(input);
        }
    }

    public void Dispose()
    {
        foreach (var pane in Panes)
        {
            pane.Dispose();
        }
        Sftp?.RefreshAsync().Wait(); // Just placeholder
    }
}
