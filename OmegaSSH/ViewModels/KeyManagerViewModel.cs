using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OmegaSSH.ViewModels;

public partial class KeyManagerViewModel : ObservableObject
{
    private readonly IKeyService _keyService;
    private readonly IVaultService _vaultService;
    private readonly IKeyStorageService _storageService;

    [ObservableProperty]
    private ObservableCollection<SshKeyModel> _keys = new();

    [ObservableProperty]
    private string _newKeyName = "work-laptop";

    [ObservableProperty]
    private string _newKeyComment = "omega-ssh-generated";

    public KeyManagerViewModel(IKeyService keyService, IVaultService vaultService, IKeyStorageService storageService)
    {
        _keyService = keyService;
        _vaultService = vaultService;
        _storageService = storageService;
        _ = LoadKeysAsync();
    }

    private async Task LoadKeysAsync()
    {
        var keys = await _storageService.LoadKeysAsync();
        Keys = new ObservableCollection<SshKeyModel>(keys);
    }

    [RelayCommand]
    private async Task GenerateKey()
    {
        var key = _keyService.GenerateEd25519Key(NewKeyName, NewKeyComment);
        
        // Encrypt private key before adding
        if (_vaultService.IsUnlocked)
        {
            key.PrivateKey = _vaultService.Encrypt(key.PrivateKey);
        }

        Keys.Add(key);
        await _storageService.SaveKeysAsync(Keys.ToList());
    }

    [RelayCommand]
    private void CopyPublicKey(SshKeyModel key)
    {
        if (key != null)
        {
            var openSshKey = _keyService.ExportToOpenSsh(key, false);
            Clipboard.SetText(openSshKey);
            // We could show a notification here if we had an INotificationService
        }
    }

    [RelayCommand]
    private async Task DeleteKey(SshKeyModel key)
    {
        if (key == null) return;
        var result = MessageBox.Show($"Are you sure you want to delete SSH key '{key.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            Keys.Remove(key);
            await _storageService.SaveKeysAsync(Keys.ToList());
        }
    }

    [RelayCommand]
    private async Task ExportKey(SshKeyModel key)
    {
        if (key == null) return;
        
        var saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = key.Name,
            Filter = "Private Key File|*",
            Title = "Export Private Key"
        };

        if (saveDialog.ShowDialog() == true)
        {
            string exportContent;
            if (_vaultService.IsUnlocked)
            {
                try
                {
                    var decrypted = _vaultService.Decrypt(key.PrivateKey);
                    exportContent = _keyService.ExportToOpenSsh(new SshKeyModel { PrivateKey = decrypted, Type = key.Type }, true);
                }
                catch
                {
                    exportContent = _keyService.ExportToOpenSsh(key, true); // Fallback to raw if decryption fails (though it shouldn't)
                }
            }
            else
            {
                MessageBox.Show("Vault is locked. Cannot export private key.", "Vault Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await File.WriteAllTextAsync(saveDialog.FileName, exportContent);
        }
    }

    [RelayCommand]
    private async Task ImportKey()
    {
        var openDialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Import Private Key"
        };

        if (openDialog.ShowDialog() == true)
        {
            try
            {
                var content = await File.ReadAllTextAsync(openDialog.FileName);
                // Very basic import logic for demo
                var key = new SshKeyModel
                {
                    Name = Path.GetFileName(openDialog.FileName),
                    Type = "Imported",
                    PrivateKey = content,
                    PublicKey = "Imported Key",
                    CreatedAt = DateTime.Now
                };

                if (_vaultService.IsUnlocked)
                {
                    key.PrivateKey = _vaultService.Encrypt(key.PrivateKey);
                }

                Keys.Add(key);
                await _storageService.SaveKeysAsync(Keys.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import key: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
