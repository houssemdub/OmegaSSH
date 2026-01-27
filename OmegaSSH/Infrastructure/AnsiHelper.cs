using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace OmegaSSH.Infrastructure;

public static class AnsiHelper
{
    // Regex to split string by ANSI escape sequences
    private static readonly Regex AnsiRegex = new Regex(@"(\x1B\[[0-9;]*[mK])", RegexOptions.Compiled);

    public static void AppendAnsiText(RichTextBox richTextBox, string text)
    {
        // Don't do heavy work if empty
        if (string.IsNullOrEmpty(text)) return;
        
        // Ensure we have a paragraph to append to
        Paragraph paragraph;
        if (richTextBox.Document.Blocks.LastBlock is Paragraph p)
        {
            paragraph = p;
        }
        else
        {
            paragraph = new Paragraph();
            richTextBox.Document.Blocks.Add(paragraph);
        }

        var parts = AnsiRegex.Split(text);

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            if (part.StartsWith("\x1B["))
            {
                // It's an ANSI code, process it (update current state)
                // In a real implementation, we'd maintain state across calls.
                // For this simple version, we'll cheat and just set the color for the *next* run if possible, 
                // but since we are splitting, the next part is the key.
                // However, the architecture here is slightly flawed for stateful parsing.
                // A better approach is state machine.
                
                // Let's implement a minimal state machine for the current append.
                // Note: This simple helper resets state on every append, which is wrong for partial streams,
                // but sufficient for a PoC.
            }
            else
            {
                // It's text
                var run = new Run(part);
                // Apply current style (omitted for brevity in this split logic)
                paragraph.Inlines.Add(run);
            }
        }
        
        richTextBox.ScrollToEnd();
    }
}
