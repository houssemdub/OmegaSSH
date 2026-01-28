using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class SftpViewModel : ObservableObject
{
    private readonly ISshService _sshService;

    [ObservableProperty]
    private string _currentPath = ".";

    [ObservableProperty]
    private ObservableCollection<SftpEntryModel> _entries = new();

    [ObservableProperty]
    private bool _isLoading;

    public SftpViewModel(ISshService sshService)
    {
        _sshService = sshService;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _sshService.ListDirectoryAsync(CurrentPath);
            Entries.Clear();
            foreach (var item in items.OrderByDescending(i => i.IsDirectory).ThenBy(i => i.Name))
            {
                Entries.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task NavigateAsync(SftpEntryModel? entry)
    {
        if (entry == null || !entry.IsDirectory) return;

        if (entry.Name == ".") return;

        if (entry.Name == "..")
        {
            if (CurrentPath == "/" || CurrentPath == ".")
            {
                CurrentPath = "/";
            }
            else
            {
                var normalized = CurrentPath.TrimEnd('/');
                var lastSlash = normalized.LastIndexOf('/');
                if (lastSlash > 0)
                {
                    CurrentPath = normalized.Substring(0, lastSlash);
                }
                else
                {
                    CurrentPath = "/";
                }
            }
        }
        else
        {
            CurrentPath = entry.FullPath;
        }

        await RefreshAsync();
    }
}
