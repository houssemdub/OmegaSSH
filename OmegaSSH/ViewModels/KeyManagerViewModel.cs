using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using OmegaSSH.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OmegaSSH.ViewModels;

public partial class KeyManagerViewModel : ObservableObject
{
    private readonly IKeyService _keyService;
    private readonly IVaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SshKeyModel> _keys = new();

    [ObservableProperty]
    private string _newKeyName = "work-laptop";

    [ObservableProperty]
    private string _newKeyComment = "omega-ssh-generated";

    public KeyManagerViewModel(IKeyService keyService, IVaultService vaultService)
    {
        _keyService = keyService;
        _vaultService = vaultService;
    }

    [RelayCommand]
    private void GenerateKey()
    {
        var key = _keyService.GenerateEd25519Key(NewKeyName, NewKeyComment);
        
        // Encrypt private key before adding
        if (_vaultService.IsUnlocked)
        {
            key.PrivateKey = _vaultService.Encrypt(key.PrivateKey);
        }

        Keys.Add(key);
    }
}
