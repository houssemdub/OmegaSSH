using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace OmegaSSH.Infrastructure;

public class SshSyntaxHighlightSchema
{
    public Dictionary<string, string> palette { get; set; } = new();
    public List<SshTokenRule> tokens { get; set; } = new();
}

public class SshTokenRule
{
    public string scope { get; set; } = string.Empty;
    public string color { get; set; } = string.Empty;
    public string? pattern { get; set; }
    public List<string>? examples { get; set; }
}

public class SyntaxHighlighter
{
    private SshSyntaxHighlightSchema? _schema;
    private readonly List<(Regex Regex, Brush Brush)> _compiledRules = new();

    public void LoadSchema(string jsonPath)
    {
        try
        {
            if (!File.Exists(jsonPath)) return;
            
            var json = File.ReadAllText(jsonPath);
            _schema = JsonSerializer.Deserialize<SshSyntaxHighlightSchema>(json);
            
            _compiledRules.Clear();
            if (_schema != null)
            {
                foreach (var token in _schema.tokens)
                {
                    string? pattern = token.pattern;
                    
                    // If no pattern, create one from examples
                    if (string.IsNullOrEmpty(pattern) && token.examples != null && token.examples.Count > 0)
                    {
                        var escapedExamples = token.examples.Select(Regex.Escape);
                        pattern = $"\\b({string.Join("|", escapedExamples)})\\b";
                    }

                    if (string.IsNullOrEmpty(pattern)) continue;

                    try
                    {
                        var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        
                        // Resolve color from palette
                        if (_schema.palette.TryGetValue(token.color, out var hex))
                        {
                            var color = (Color)ColorConverter.ConvertFromString(hex);
                            var brush = new SolidColorBrush(color);
                            brush.Freeze();
                            _compiledRules.Add((regex, brush));
                        }
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    public void ApplyHighlighting(Paragraph paragraph, string text)
    {
        if (_compiledRules.Count == 0)
        {
            paragraph.Inlines.Add(new Run(text) { Foreground = Brushes.White });
            return;
        }

        // We use a simple strategy: Find the first matching rule for each segment
        // This isn't perfect for overlapping rules but works for a terminal
        
        List<TextSegment> segments = new List<TextSegment> { new TextSegment { Text = text, Foreground = Brushes.White } };

        foreach (var rule in _compiledRules)
        {
            List<TextSegment> nextSegments = new List<TextSegment>();
            foreach (var segment in segments)
            {
                if (segment.IsHighlighted)
                {
                    nextSegments.Add(segment);
                    continue;
                }

                var matches = rule.Regex.Matches(segment.Text);
                int lastIdx = 0;
                foreach (Match match in matches)
                {
                    if (match.Index > lastIdx)
                    {
                        nextSegments.Add(new TextSegment { Text = segment.Text.Substring(lastIdx, match.Index - lastIdx), Foreground = segment.Foreground });
                    }
                    nextSegments.Add(new TextSegment { Text = match.Value, Foreground = rule.Brush, IsHighlighted = true });
                    lastIdx = match.Index + match.Length;
                }

                if (lastIdx < segment.Text.Length)
                {
                    nextSegments.Add(new TextSegment { Text = segment.Text.Substring(lastIdx), Foreground = segment.Foreground });
                }
            }
            segments = nextSegments;
        }

        foreach (var segment in segments)
        {
            if (!string.IsNullOrEmpty(segment.Text))
            {
                paragraph.Inlines.Add(new Run(segment.Text) { Foreground = segment.Foreground });
            }
        }
    }

    private class TextSegment
    {
        public string Text { get; set; } = string.Empty;
        public Brush Foreground { get; set; } = Brushes.White;
        public bool IsHighlighted { get; set; }
    }
}
