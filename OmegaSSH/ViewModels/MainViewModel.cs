using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OmegaSSH.Models;
using OmegaSSH.Services;

namespace OmegaSSH.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISessionService _sessionService;
    private readonly ISnippetService _snippetService;
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private ObservableCollection<SessionModel> _sessions = new();

    [ObservableProperty]
    private ObservableCollection<SnippetModel> _snippets = new();

    [ObservableProperty]
    private ObservableCollection<SessionTreeItemViewModel> _sessionTree = new();

    [ObservableProperty]
    private SessionModel? _selectedSession;

    [ObservableProperty]
    private ObservableCollection<TerminalViewModel> _terminalTabs = new();

    [ObservableProperty]
    private TerminalViewModel? _selectedTab;

    [ObservableProperty]
    private KeyManagerViewModel _keyManager;

    [ObservableProperty]
    private bool _isKeyManagerActive;

    public MainViewModel(ISessionService sessionService, ISnippetService snippetService, ISettingsService settingsService, IServiceProvider serviceProvider, IKeyService keyService, IVaultService vaultService, IThemeService themeService)
    {
        _sessionService = sessionService;
        _snippetService = snippetService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _themeService = themeService;
        _keyManager = new KeyManagerViewModel(keyService, vaultService);
        LoadSessionsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task ChangeTheme(string themeName)
    {
        _themeService.SetTheme(themeName);
        _settingsService.Settings.Theme = themeName;
        await _settingsService.SaveSettingsAsync();
        StatusText = $"Theme changed to {themeName}";
    }

    [RelayCommand]
    private void OpenKeyManager()
    {
        IsKeyManagerActive = true;
    }

    [RelayCommand]
    private void CloseKeyManager()
    {
        IsKeyManagerActive = false;
    }

    [RelayCommand]
    private async Task LoadSessions()
    {
        try
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            var snippets = await _snippetService.GetAllSnippetsAsync();

            Sessions.Clear();
            SessionTree.Clear();
            Snippets.Clear();

            foreach (var s in snippets) Snippets.Add(s);

            foreach (var s in sessions)
            {
                Sessions.Add(s);
                
                // Build tree
                var folderPath = s.Folder ?? "General";
                var folderNode = SessionTree.FirstOrDefault(t => t.Name == folderPath && t.IsFolder);
                if (folderNode == null)
                {
                    folderNode = new SessionTreeItemViewModel(folderPath, true);
                    SessionTree.Add(folderNode);
                }
                folderNode.Children.Add(new SessionTreeItemViewModel(s.Name, false, s));
            }
            
            if (Sessions.Count == 0)
            {
                StatusText = "No sessions found. Create one!";
            }
        }
        catch (Exception ex)
        {
            OmegaSSH.Views.NotificationWindow.Show($"Failed to load sessions: {ex.Message}", "SYNC ERROR", true);
        }
    }

    [RelayCommand]
    private async Task BroadcastCommand(string command)
    {
        foreach (var tab in TerminalTabs)
        {
            await tab.SendInputCommand.ExecuteAsync(command);
        }
    }

    [RelayCommand]
    private void ConnectToSession(SessionModel session)
    {
        var sshService = _serviceProvider.GetRequiredService<ISshService>();
        var sessionLogger = _serviceProvider.GetRequiredService<ISessionLogger>();
        var terminalVM = new TerminalViewModel(sshService, sessionLogger, session);
        TerminalTabs.Add(terminalVM);
        SelectedTab = terminalVM;
        StatusText = $"Connected to {session.Name}";
    }

    [RelayCommand]
    private void AddTab()
    {
        // For now, just a placeholder or local terminal
    }

    [RelayCommand]
    private void CloseTab(TerminalViewModel tab)
    {
        if (tab != null)
        {
            TerminalTabs.Remove(tab);
            tab.Dispose();
            if (SelectedTab == tab) SelectedTab = TerminalTabs.LastOrDefault();
        }
    }

    [RelayCommand]
    private async Task CreateTestSession()
    {
        var testSession = new SessionModel { Name = "Localhost", Host = "127.0.0.1", Username = "user", Port = 22 };
        await _sessionService.SaveSessionAsync(testSession);
        
        var testSnippet = new SnippetModel { Name = "Refresh List", Command = "ls -la\r" };
        await _snippetService.SaveSnippetAsync(testSnippet);

        await LoadSessions();
    }

    [RelayCommand]
    private async Task ExecuteSnippet(SnippetModel snippet)
    {
        if (SelectedTab != null)
        {
            await SelectedTab.SendInputCommand.ExecuteAsync(snippet.Command);
        }
    }
}
