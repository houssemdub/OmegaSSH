using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
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

    [ObservableProperty]
    private bool _isSidebarVisible = true;

    [ObservableProperty]
    private bool _isSnippetsPanelVisible = true;

    [ObservableProperty]
    private bool _isStatusBarVisible = true;

    [ObservableProperty]
    private bool _isZenMode = false;

    [ObservableProperty]
    private bool _isBroadcastMode = false;

    partial void OnSessionFilterChanged(string value) => FilterSessions(value);

    public MainViewModel(ISessionService sessionService, ISnippetService snippetService, ISettingsService settingsService, IServiceProvider serviceProvider, IKeyService keyService, IVaultService vaultService, IThemeService themeService, IKeyStorageService keyStorageService)
    {
        _sessionService = sessionService;
        _snippetService = snippetService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _themeService = themeService;
        _keyManager = new KeyManagerViewModel(keyService, vaultService, keyStorageService);
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
    private void OpenMultiCommander()
    {
        var vm = _serviceProvider.GetRequiredService<JobOrchestratorViewModel>();
        var win = new OmegaSSH.Views.MultiCommanderWindow { DataContext = vm };
        win.Owner = App.Current.MainWindow;
        win.ShowDialog();
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
        await LoadSnippets();
    }

    private async Task LoadSnippets()
    {
        try
        {
            var snips = await _snippetService.GetAllSnippetsAsync();
            Snippets.Clear();
            foreach (var s in snips) Snippets.Add(s);
        }
        catch { }
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
        var terminalVM = new TerminalViewModel(sshService, sessionLogger, settingsService, _serviceProvider, session);
        TerminalTabs.Add(terminalVM);
        SelectedTab = terminalVM;
        StatusText = $"Connected to {session.Name}";
    }

    [RelayCommand]
    private void OpenLocalTerminal(string shellType = "powershell.exe")
    {
        var engine = new LocalTerminalService(shellType);
        var logger = _serviceProvider.GetRequiredService<ISessionLogger>();
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        
        // We simulate a session for the UI
        var session = new SessionModel { Name = $"local:{shellType.Replace(".exe", "")}" };
        var terminalVM = new TerminalViewModel(engine, logger, settingsService, _serviceProvider, session);
        
        TerminalTabs.Add(terminalVM);
        SelectedTab = terminalVM;
        StatusText = $"Opened local {shellType}";
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
    private void QuickConnect(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        // basic parsing: user@host:port
        string user = "root";
        string host = connectionString;
        int port = 22;

        if (connectionString.Contains("@"))
        {
            var parts = connectionString.Split('@');
            user = parts[0];
            host = parts[1];
        }

        if (host.Contains(":"))
        {
            var parts = host.Split(':');
            host = parts[0];
            if (int.TryParse(parts[1], out int p)) port = p;
        }

        var session = new SessionModel
        {
            Name = $"{user}@{host}",
            Host = host,
            Port = port,
            Username = user
        };

        ConnectToSession(session);
    }

    [RelayCommand]
    private async Task ExportSessions()
    {
        var saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = "omega_sessions.json",
            Filter = "JSON File|*.json",
            Title = "Export Sessions"
        };

        if (saveDialog.ShowDialog() == true)
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(sessions, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(saveDialog.FileName, json);
            StatusText = $"Exported {sessions.Count} sessions.";
        }
    }

    [RelayCommand]
    private async Task ImportSessions()
    {
        var openDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON File|*.json",
            Title = "Import Sessions"
        };

        if (openDialog.ShowDialog() == true)
        {
            try
            {
                var json = await File.ReadAllTextAsync(openDialog.FileName);
                var sessions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SessionModel>>(json);
                if (sessions != null)
                {
                    foreach (var s in sessions)
                    {
                        s.Id = Guid.NewGuid(); // Ensure new IDs
                        await _sessionService.SaveSessionAsync(s);
                    }
                    await LoadSessions();
                    StatusText = $"Imported {sessions.Count} sessions.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private async Task AddSnippet()
    {
        var vm = new SnippetEditViewModel();
        var win = new OmegaSSH.Views.SnippetEditWindow(vm) { Owner = System.Windows.Application.Current.MainWindow };
        
        if (win.ShowDialog() == true && vm.IsSaved)
        {
            await _snippetService.SaveSnippetAsync(vm.Result);
            await LoadSessions(); // This also reloads snippets in current implementation
        }
    }

    [RelayCommand]
    private async Task EditSnippet(SnippetModel snippet)
    {
        if (snippet == null) return;
        var vm = new SnippetEditViewModel(snippet);
        var win = new OmegaSSH.Views.SnippetEditWindow(vm) { Owner = System.Windows.Application.Current.MainWindow };
        
        if (win.ShowDialog() == true && vm.IsSaved)
        {
            await _snippetService.SaveSnippetAsync(vm.Result);
            await LoadSessions();
        }
    }

    [RelayCommand]
    private async Task DeleteSnippet(SnippetModel snippet)
    {
        if (snippet == null) return;
        var result = System.Windows.MessageBox.Show($"Are you sure you want to delete snippet '{snippet.Name}'?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            await _snippetService.DeleteSnippetAsync(snippet.Id);
            await LoadSessions();
        }
    }

    [RelayCommand]
    private async Task CreateTestSession()
    {
        var testSession = new SessionModel { Name = "Example Session", Host = "127.0.0.1", Username = "user", Port = 22 };
        await _sessionService.SaveSessionAsync(testSession);
        await LoadSessions();
    }

    [RelayCommand]
    private async Task ExecuteSnippet(SnippetModel snippet)
    {
        if (IsBroadcastMode)
        {
            var tasks = TerminalTabs.Select(t => t.SendInputCommand.ExecuteAsync(snippet.Command));
            await Task.WhenAll(tasks);
        }
        else if (SelectedTab != null)
        {
            await SelectedTab.SendInputCommand.ExecuteAsync(snippet.Command);
        }
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarVisible = !IsSidebarVisible;
    }

    [RelayCommand]
    private void ToggleSnippetsPanel()
    {
        IsSnippetsPanelVisible = !IsSnippetsPanelVisible;
    }

    [RelayCommand]
    private void ToggleStatusBar()
    {
        IsStatusBarVisible = !IsStatusBarVisible;
    }

    [RelayCommand]
    private void ToggleZenMode()
    {
        IsZenMode = !IsZenMode;
        if (IsZenMode)
        {
            IsSidebarVisible = false;
            IsSnippetsPanelVisible = false;
            IsStatusBarVisible = false;
        }
        else
        {
            IsSidebarVisible = true;
            IsSnippetsPanelVisible = true;
            IsStatusBarVisible = true;
        }
    }

    [RelayCommand]
    private void ToggleBroadcastMode()
    {
        IsBroadcastMode = !IsBroadcastMode;
    }
}
