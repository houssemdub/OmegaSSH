using System.Windows;
using System.Windows.Input;
using OmegaSSH.Services;

namespace OmegaSSH.Views;

public partial class UnlockWindow : Window
{
    private readonly IVaultService _vaultService;

    public UnlockWindow(IVaultService vaultService)
    {
        InitializeComponent();
        _vaultService = vaultService;
        this.MouseLeftButtonDown += (s, e) => this.DragMove();
        VaultPassBox.Focus();
    }

    private void Unlock_Click(object sender, RoutedEventArgs e)
    {
        TryUnlock();
    }

    private void VaultPassBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryUnlock();
        }
    }

    private void TryUnlock()
    {
        string pass = VaultPassBox.Password;
        if (string.IsNullOrEmpty(pass)) return;

        // For this demo, we just set it. In production, we'd verify a hash.
        _vaultService.Unlock(pass);
        this.DialogResult = true;
        this.Close();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
