using OmegaSSH.ViewModels;
using System.Windows;

namespace OmegaSSH.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;
        this.MouseLeftButtonDown += (s, e) => this.DragMove();
    }
}
