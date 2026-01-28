using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

    [ObservableProperty]
    private string _sessionFilter = string.Empty;

    partial void OnSessionFilterChanged(string value) => FilterSessions(value);

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
    private void OpenSettings()
    {
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var themeService = _serviceProvider.GetRequiredService<IThemeService>();
        var vm = new SettingsViewModel(settingsService, themeService);
        var win = new OmegaSSH.Views.SettingsWindow(vm) { Owner = System.Windows.Application.Current.MainWindow };
        win.ShowDialog();
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
                StatusText = "No sessions found. Add your first connection!";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading sessions: {ex.Message}";
        }
    }

    private void FilterSessions(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            _ = LoadSessions(); // Reload full tree
            return;
        }

        var lowerFilter = filter.ToLower();
        var filteredSessions = Sessions.Where(s => 
            s.Name.ToLower().Contains(lowerFilter) || 
            s.Host.ToLower().Contains(lowerFilter) ||
            (s.Folder?.ToLower().Contains(lowerFilter) ?? false)).ToList();

        SessionTree.Clear();
        foreach (var s in filteredSessions)
        {
            var folderPath = s.Folder ?? "General";
            var folderNode = SessionTree.FirstOrDefault(t => t.Name == folderPath && t.IsFolder);
            if (folderNode == null)
            {
                folderNode = new SessionTreeItemViewModel(folderPath, true);
                SessionTree.Add(folderNode);
            }
            folderNode.Children.Add(new SessionTreeItemViewModel(s.Name, false, s));
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
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var terminalVM = new TerminalViewModel(sshService, sessionLogger, settingsService, session);
        TerminalTabs.Add(terminalVM);
        SelectedTab = terminalVM;
        StatusText = $"Connected to {session.Name}";
    }

    [RelayCommand]
    private async Task AddSession()
    {
        var vm = new SessionEditViewModel();
        var win = new OmegaSSH.Views.SessionEditWindow(vm) { Owner = System.Windows.Application.Current.MainWindow };
        
        if (win.ShowDialog() == true && vm.IsSaved)
        {
            await _sessionService.SaveSessionAsync(vm.Result);
            await LoadSessions();
        }
    }

    [RelayCommand]
    private async Task EditSession(SessionModel session)
    {
        if (session == null) return;
        var vm = new SessionEditViewModel(session);
        var win = new OmegaSSH.Views.SessionEditWindow(vm) { Owner = System.Windows.Application.Current.MainWindow };
        
        if (win.ShowDialog() == true && vm.IsSaved)
        {
            await _sessionService.SaveSessionAsync(vm.Result);
            await LoadSessions();
        }
    }

    [RelayCommand]
    private async Task DeleteSession(SessionModel session)
    {
        if (session == null) return;
        var result = System.Windows.MessageBox.Show($"Are you sure you want to delete session '{session.Name}'?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            await _sessionService.DeleteSessionAsync(session.Id);
            await LoadSessions();
        }
    }

    [RelayCommand]
    private async Task DuplicateSession(SessionModel session)
    {
        if (session == null) return;
        var clone = new SessionModel
        {
            Name = session.Name + " (Copy)",
            Host = session.Host,
            Port = session.Port,
            Username = session.Username,
            Password = session.Password,
            PrivateKeyPath = session.PrivateKeyPath,
            Folder = session.Folder
        };
        await _sessionService.SaveSessionAsync(clone);
        await LoadSessions();
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
