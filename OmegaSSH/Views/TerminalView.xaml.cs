using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OmegaSSH.ViewModels;

namespace OmegaSSH.Views;

public partial class TerminalView : UserControl
{
    public TerminalView()
    {
        InitializeComponent();
        this.Loaded += TerminalView_Loaded;
    }

    private void TerminalView_Loaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.PreviewKeyDown += Window_PreviewKeyDown;
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            SearchOverlay.Visibility = SearchOverlay.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            if (SearchOverlay.Visibility == Visibility.Visible)
            {
                SearchBox.Focus();
                SearchBox.SelectAll();
            }
            e.Handled = true;
        }
        else if (e.Key == Key.Escape && SearchOverlay.Visibility == Visibility.Visible)
        {
            SearchOverlay.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
    }

    private void ToggleSftp_Click(object sender, RoutedEventArgs e)
    {
        if (SftpPane.Visibility == Visibility.Visible)
        {
            SftpPane.Visibility = Visibility.Collapsed;
        }
        else
        {
            SftpPane.Visibility = Visibility.Visible;
        }
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is TerminalViewModel vm)
            {
                vm.SearchCommand.Execute(SearchBox.Text);
            }
        }
    }

    private void CloseSearch_Click(object sender, RoutedEventArgs e)
    {
        SearchOverlay.Visibility = Visibility.Collapsed;
    }
}
