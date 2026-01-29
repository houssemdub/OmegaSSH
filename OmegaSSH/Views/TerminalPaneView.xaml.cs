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
        if (e.OldValue is TerminalPaneViewModel oldVm)
        {
            oldVm.DataReceived -= Vm_DataReceived;
            oldVm.SearchResultFound -= Vm_SearchResultFound;
        }

        if (e.NewValue is TerminalPaneViewModel vm)
        {
            vm.DataReceived += Vm_DataReceived;
            vm.SearchResultFound += Vm_SearchResultFound;

            if (!string.IsNullOrEmpty(vm.TerminalOutput))
            {
                OutputBox.Document.Blocks.Clear();
                Vm_DataReceived(vm.TerminalOutput);
            }
        }
    }

    private void Vm_SearchResultFound(int index)
    {
        // Simple highlighting: find the text in the RichTextBox and select it
        // Note: index in string buffer doesn't map perfectly to RichTextBox text
        // but for now we just scroll to the end or try to find it.
        OutputBox.Focus();
        OutputBox.ScrollToEnd();
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
        if (DataContext is not TerminalPaneViewModel vm) return;

        // Smart Keyboard Handling - Capture before TextBox/RichTextBox handles them
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.C)
            {
                // Smart Ctrl+C: If there's a selection in the OutputBox, we COPY. Otherwise, we INTERRUPT.
                if (sender is RichTextBox rtb && !rtb.Selection.IsEmpty)
                {
                    // Allow normal copy to proceed (don't set e.Handled)
                    return;
                }
                
                // Send break signal (ETX)
                await vm.InterruptCommand.ExecuteAsync(null);
                
                // Visual feedback: blink the stop icon or change status if needed
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.V)
            {
                // Paste from clipboard
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    await vm.SendInputCommand.ExecuteAsync(text);
                }
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.L)
            {
                // Smart Clear: Ctrl+L
                OutputBox.Document.Blocks.Clear();
                e.Handled = true;
                return;
            }
        }

        if (e.Key == Key.Enter && sender is TextBox)
        {
            await vm.SendInputCommand.ExecuteAsync(InputBox.Text + "\r");
            InputBox.Clear();
            e.Handled = true;
        }
    }
}
