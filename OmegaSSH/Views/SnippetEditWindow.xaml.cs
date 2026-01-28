using OmegaSSH.ViewModels;
using System.Windows;

namespace OmegaSSH.Views;

public partial class SnippetEditWindow : Window
{
    public SnippetEditWindow(SnippetEditViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;
        this.MouseLeftButtonDown += (s, e) => this.DragMove();
    }
}
