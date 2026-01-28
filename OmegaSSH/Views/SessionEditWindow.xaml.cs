using OmegaSSH.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OmegaSSH.Views;

public partial class SessionEditWindow : Window
{
    public SessionEditWindow(SessionEditViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;
        
        // Sync password box (one-way for initial load)
        if (!string.IsNullOrEmpty(vm.Password))
        {
            PassBox.Password = vm.Password;
        }

        // Update VM on every change
        PassBox.PasswordChanged += (s, e) => vm.Password = PassBox.Password;
        
        // Drag window
        this.MouseLeftButtonDown += (s, e) => this.DragMove();
    }
}
