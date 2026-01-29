using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace OmegaSSH.Infrastructure;

public class AnsiParser
{
    private static readonly Regex AnsiRegex = new Regex(@"(\x1B\[[0-9;?]*[a-zA-Z])", RegexOptions.Compiled);
    private Brush _currentForeground = Brushes.LightGray;
    private readonly SyntaxHighlighter? _highlighter;

    private const int MaxLines = 1000;

    public AnsiParser(SyntaxHighlighter? highlighter = null)
    {
        _highlighter = highlighter;
    }

    public void ParseAndAppend(RichTextBox richTextBox, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        // Use BeginInvoke to prevent blocking the network thread
        richTextBox.Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                var document = richTextBox.Document;
                Paragraph? paragraph = document.Blocks.LastBlock as Paragraph;
                
                var parts = AnsiRegex.Split(text);
                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part)) continue;

                    if (paragraph == null)
                    {
                        paragraph = new Paragraph { Margin = new Thickness(0) };
                        document.Blocks.Add(paragraph);
                    }

                    if (part.StartsWith("\x1B["))
                    {
                        if (part.EndsWith("m"))
                        {
                            _currentForeground = GetColorFromAnsi(part, _currentForeground);
                        }
                        else if (part.EndsWith("J") || (part.EndsWith("H") && part.Length < 5)) // ESC[2J or ESC[H
                        {
                            document.Blocks.Clear();
                            paragraph = null;
                        }
                        // Ignore other sequences like [6n (status) or [?25h (cursor)
                    }
                        else
                        {
                            // Clean up control characters that might confuse RichTextBox
                            string cleanPart = part.Replace("\r", "");
                            if (cleanPart.Contains("\n"))
                            {
                                var lines = cleanPart.Split('\n');
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lines[i]))
                                    {
                                        if (_highlighter != null)
                                        {
                                            _highlighter.ApplyHighlighting(paragraph, lines[i]);
                                        }
                                        else
                                        {
                                            paragraph.Inlines.Add(new Run(lines[i]) { Foreground = _currentForeground });
                                        }
                                    }
                                    if (i < lines.Length - 1)
                                    {
                                        paragraph = new Paragraph { Margin = new Thickness(0) };
                                        document.Blocks.Add(paragraph);
                                    }
                                }
                            }
                            else
                            {
                                if (_highlighter != null)
                                {
                                    _highlighter.ApplyHighlighting(paragraph, cleanPart);
                                }
                                else
                                {
                                    paragraph.Inlines.Add(new Run(cleanPart) { Foreground = _currentForeground });
                                }
                            }
                        }
                }

                // Line Limiting: Keep only the last MaxLines
                while (document.Blocks.Count > MaxLines)
                {
                    document.Blocks.Remove(document.Blocks.FirstBlock);
                }

                richTextBox.ScrollToEnd();
            }
            catch { }
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    private Brush GetColorFromAnsi(string ansiCode, Brush current)
    {
        // Simple 8-color implementation
        if (ansiCode.Contains("30")) return Brushes.Black;
        if (ansiCode.Contains("31")) return Brushes.Red;
        if (ansiCode.Contains("32")) return Brushes.LimeGreen;
        if (ansiCode.Contains("33")) return Brushes.Yellow;
        if (ansiCode.Contains("34")) return Brushes.DodgerBlue;
        if (ansiCode.Contains("35")) return Brushes.Magenta;
        if (ansiCode.Contains("36")) return Brushes.Cyan;
        if (ansiCode.Contains("37")) return Brushes.White;
        if (ansiCode.Contains("0")) return Brushes.LightGray; // Reset (actually default terminal color)
        
        return current;
    }

    private SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
