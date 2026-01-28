using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace OmegaSSH.ViewModels;

public partial class SessionEditViewModel : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _host = string.Empty;
    [ObservableProperty] private int _port = 22;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _privateKeyPath = string.Empty;
    [ObservableProperty] private string _folder = "General";
    [ObservableProperty] private bool _agentForwarding;
    
    public ObservableCollection<TunnelModel> Tunnels { get; } = new();
    
    public SessionModel Result { get; private set; }
    public bool IsSaved { get; private set; }

    public SessionEditViewModel(SessionModel? session = null)
    {
        if (session != null)
        {
            Result = session; // Edit existing
            Name = session.Name;
            Host = session.Host;
            Port = session.Port;
            Username = session.Username;
            Password = session.Password ?? string.Empty;
            PrivateKeyPath = session.PrivateKeyPath ?? string.Empty;
            Folder = session.Folder ?? "General";
            AgentForwarding = session.AgentForwarding;
            foreach (var t in session.Tunnels) Tunnels.Add(t);
        }
        else
        {
            Result = new SessionModel(); // New session
        }
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Host))
        {
            MessageBox.Show("Name and Host are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.Name = Name;
        Result.Host = Host;
        Result.Port = Port;
        Result.Username = Username;
        Result.Password = Password;
        Result.PrivateKeyPath = PrivateKeyPath;
        Result.Folder = Folder;
        Result.AgentForwarding = AgentForwarding;
        Result.Tunnels = new List<TunnelModel>(Tunnels);

        IsSaved = true;
        window.DialogResult = true;
        window.Close();
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.DialogResult = false;
        window.Close();
    }

    [RelayCommand]
    private void BrowseKey()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();
        if (dialog.ShowDialog() == true)
        {
            PrivateKeyPath = dialog.FileName;
        }
    }
    [RelayCommand]
    private void AddTunnel()
    {
        Tunnels.Add(new TunnelModel { LocalPort = 8080, RemoteHost = "localhost", RemotePort = 80, IsEnabled = true });
    }

    [RelayCommand]
    private void RemoveTunnel(TunnelModel tunnel)
    {
        if (tunnel != null) Tunnels.Remove(tunnel);
    }
}
