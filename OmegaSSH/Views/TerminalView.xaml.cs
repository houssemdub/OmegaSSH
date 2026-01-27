using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OmegaSSH.Infrastructure;
using OmegaSSH.ViewModels;

namespace OmegaSSH.Views;

public partial class TerminalView : UserControl
{
    private AnsiParser _ansiParser = new AnsiParser();

    public TerminalView()
    {
        InitializeComponent();
        this.DataContextChanged += TerminalView_DataContextChanged;
    }

    private void TerminalView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is TerminalViewModel vm)
        {
            // Subscribe to the raw data stream for ANSI parsing
            vm.DataReceived -= Vm_DataReceived;
            vm.DataReceived += Vm_DataReceived;
            
            // Replay any buffered output if necessary (VM could store history)
            if (!string.IsNullOrEmpty(vm.TerminalOutput))
            {
                Vm_DataReceived(vm.TerminalOutput);
            }
        }
        else if (e.OldValue is TerminalViewModel oldVm)
        {
            oldVm.DataReceived -= Vm_DataReceived;
        }
    }

    private void Vm_DataReceived(string data)
    {
        if (string.IsNullOrEmpty(data)) return;
        _ansiParser.ParseAndAppend(OutputBox, data);
    }

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is TerminalViewModel vm)
            {
                await vm.SendInputCommand.ExecuteAsync(InputBox.Text + "\r");
                InputBox.Clear();
            }
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
}
