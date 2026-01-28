using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using OmegaSSH.Infrastructure;
using OmegaSSH.ViewModels;

namespace OmegaSSH.Views;

public partial class TerminalPaneView : UserControl
{
    private readonly AnsiParser _ansiParser;

    public TerminalPaneView()
    {
        InitializeComponent();
        
        var highlighter = new SyntaxHighlighter();
        // Resolve path relative to app execution - for development, we use the absolute path or check if it exists
        var highlightPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Highlights", "default.json");
        if (!System.IO.File.Exists(highlightPath))
        {
            // Try relative to project root or something
            highlightPath = @"d:\Visual Studio Projects\C#\OmegaSSH\OmegaSSH\Resources\Highlights\default.json";
        }
        
        highlighter.LoadSchema(highlightPath);
        _ansiParser = new AnsiParser(highlighter);

        this.DataContextChanged += TerminalPaneView_DataContextChanged;
    }

    private void TerminalPaneView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is TerminalPaneViewModel vm)
        {
            vm.DataReceived -= Vm_DataReceived;
            vm.DataReceived += Vm_DataReceived;

            if (!string.IsNullOrEmpty(vm.TerminalOutput))
            {
                Vm_DataReceived(vm.TerminalOutput);
            }
        }
    }

    private void Vm_DataReceived(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        if (data.Contains('\a'))
        {
            System.Media.SystemSounds.Beep.Play();
        }

        _ansiParser.ParseAndAppend(OutputBox, data);
    }

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is TerminalPaneViewModel vm)
            {
                await vm.SendInputCommand.ExecuteAsync(InputBox.Text + "\r");
                InputBox.Clear();
            }
        }
    }
}
