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

    [ObservableProperty]
    private bool _isTransferring;

    [ObservableProperty]
    private double _transferProgress;

    public SftpViewModel(ISshService sshService)
    {
        _sshService = sshService;
    }

    [RelayCommand]
    public async Task UploadFilesAsync(string[] filePaths)
    {
        if (filePaths == null || filePaths.Length == 0) return;

        IsTransferring = true;
        TransferProgress = 0;
        int count = 0;

        try
        {
            foreach (var path in filePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    var remotePath = CurrentPath.TrimEnd('/') + "/" + System.IO.Path.GetFileName(path);
                    await _sshService.UploadFileAsync(path, remotePath);
                    count++;
                    TransferProgress = (double)count / filePaths.Length * 100;
                }
            }
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Upload failed: {ex.Message}");
        }
        finally
        {
            IsTransferring = false;
        }
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
    public async Task NavigateUpAsync()
    {
        if (CurrentPath == "/" || CurrentPath == ".") return;
        
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
        await RefreshAsync();
    }

    [RelayCommand]
    public async Task NavigateAsync(SftpEntryModel? entry)
    {
        if (entry == null || !entry.IsDirectory) return;

        if (entry.Name == ".") return;

        if (entry.Name == "..")
        {
            await NavigateUpAsync();
        }
        else
        {
            CurrentPath = entry.FullPath;
            await RefreshAsync();
        }
    }

    [RelayCommand]
    public async Task DownloadAsync(SftpEntryModel? entry)
    {
        if (entry == null || entry.IsDirectory) return;

        var sfd = new Microsoft.Win32.SaveFileDialog
        {
            FileName = entry.Name,
            Title = "Download File"
        };

        if (sfd.ShowDialog() == true)
        {
            IsTransferring = true;
            try
            {
                await _sshService.DownloadFileAsync(entry.FullPath, sfd.FileName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Download failed: {ex.Message}");
            }
            finally
            {
                IsTransferring = false;
            }
        }
    }

    [RelayCommand]
    public async Task DeleteAsync(SftpEntryModel? entry)
    {
        if (entry == null) return;
        
        if (System.Windows.MessageBox.Show($"Are you sure you want to delete {entry.Name}?", "Confirm Delete", 
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                if (entry.IsDirectory) await _sshService.DeleteDirectoryAsync(entry.FullPath);
                else await _sshService.DeleteFileAsync(entry.FullPath);
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Delete failed: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    public async Task RenameAsync(SftpEntryModel? entry)
    {
        // Simple rename logic would need a dialog, skip for now or use a prompt.
        // For now, let's keep it simple.
    }
}
