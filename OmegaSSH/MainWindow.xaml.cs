using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using OmegaSSH.ViewModels;

namespace OmegaSSH;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Closing += MainWindow_Closing;
        this.StateChanged += MainWindow_StateChanged;
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        // Fix border when maximized
        if (WindowState == WindowState.Maximized)
        {
            BorderThickness = new Thickness(8); // Account for resize border
        }
        else
        {
            BorderThickness = new Thickness(1);
        }
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            var ss = App.Current.ServiceProvider.GetRequiredService<OmegaSSH.Services.ISettingsService>();
            if (ss != null && this.ActualWidth > 100 && this.ActualHeight > 100)
            {
                ss.Settings.WindowWidth = this.ActualWidth;
                ss.Settings.WindowHeight = this.ActualHeight;
                await ss.SaveSettingsAsync();
            }
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
        else
            WindowState = WindowState.Maximized;
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void TerminalBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        textBox?.ScrollToEnd();
    }

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.DataContext is TerminalViewModel vm)
            {
                await vm.SendInputCommand.ExecuteAsync(textBox.Text + "\r");
                textBox.Clear();
            }
        }
    }

    private void SessionTreeItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        var item = sender as TreeViewItem;
        if (item != null && item.Header is SessionTreeItemViewModel vm && !vm.IsFolder && vm.Session != null)
        {
            if (DataContext is MainViewModel mainVm)
            {
                mainVm.ConnectToSessionCommand.Execute(vm.Session);
            }
        }
    }

    private void QuickConnectBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var text = QuickConnectBox.Text;
            if (string.IsNullOrWhiteSpace(text)) return;

            if (DataContext is MainViewModel vm)
            {
                vm.QuickConnectCommand.Execute(text);
                QuickConnectBox.Clear();
            }
        }
    }
}