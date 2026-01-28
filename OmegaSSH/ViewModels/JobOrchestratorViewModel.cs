using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Infrastructure;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class JobOrchestratorViewModel : ObservableObject
{
    private readonly IJobOrchestrator _orchestrator;
    private readonly ISessionService _sessionService;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string _command = string.Empty;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private double _progress;

    public ObservableCollection<SessionSelectionViewModel> Sessions { get; } = new();
    public ObservableCollection<JobResult> Results { get; } = new();

    public JobOrchestratorViewModel(IJobOrchestrator orchestrator, ISessionService sessionService)
    {
        _orchestrator = orchestrator;
        _sessionService = sessionService;
        LoadSessions();
    }

    private async void LoadSessions()
    {
        var rawSessions = await _sessionService.GetAllSessionsAsync();
        foreach (var s in rawSessions)
        {
            Sessions.Add(new SessionSelectionViewModel { Session = s, IsSelected = false });
        }
    }

    [RelayCommand]
    private async Task ExecuteParallel()
    {
        if (string.IsNullOrWhiteSpace(Command)) return;
        var selected = Sessions.Where(s => s.IsSelected).Select(s => s.Session).ToList();
        if (selected.Count == 0) return;

        IsRunning = true;
        Results.Clear();
        Progress = 0;
        _cts = new CancellationTokenSource();

        int total = selected.Count;
        int completed = 0;

        try
        {
            await _orchestrator.ExecuteParallelAsync(selected, Command, (res) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Results.Add(res);
                    completed++;
                    Progress = (double)completed / total * 100;
                });
            }, _cts.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _cts?.Cancel();
    }
}

public partial class SessionSelectionViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    public SessionModel Session { get; set; } = null!;
}
