using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace OmegaSSH.Infrastructure;

public class SyntaxHighlightRule
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
    public string? Foreground { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
}

public class SyntaxHighlightSchema
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public List<SyntaxHighlightRule> Rules { get; set; } = new();
}

public class SyntaxHighlighter
{
    private SyntaxHighlightSchema? _schema;
    private readonly List<(Regex Regex, SyntaxHighlightRule Rule)> _compiledRules = new();

    public void LoadSchema(string jsonPath)
    {
        try
        {
            var json = File.ReadAllText(jsonPath);
            _schema = JsonSerializer.Deserialize<SyntaxHighlightSchema>(json);
            
            _compiledRules.Clear();
            if (_schema?.Rules != null)
            {
                foreach (var rule in _schema.Rules)
                {
                    try
                    {
                        var regex = new Regex(rule.Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        _compiledRules.Add((regex, rule));
                    }
                    catch
                    {
                        // Skip invalid regex
                    }
                }
            }
        }
        catch
        {
            // Fallback to no highlighting
        }
    }

    public void ApplyHighlighting(Paragraph paragraph, string text)
    {
        if (_compiledRules.Count == 0)
        {
            paragraph.Inlines.Add(new Run(text));
            return;
        }

        var matches = new List<(int Start, int Length, SyntaxHighlightRule Rule)>();

        // Find all matches
        foreach (var (regex, rule) in _compiledRules)
        {
            foreach (Match match in regex.Matches(text))
            {
                matches.Add((match.Index, match.Length, rule));
            }
        }

        // Sort by position
        matches.Sort((a, b) => a.Start.CompareTo(b.Start));

        int currentPos = 0;
        foreach (var (start, length, rule) in matches)
        {
            // Add text before match
            if (start > currentPos)
            {
                paragraph.Inlines.Add(new Run(text.Substring(currentPos, start - currentPos)));
            }

            // Add highlighted match
            var run = new Run(text.Substring(start, length));
            
            if (!string.IsNullOrEmpty(rule.Foreground))
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(rule.Foreground);
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();
                    run.Foreground = brush;
                }
                catch { }
            }

            if (rule.Bold) run.FontWeight = System.Windows.FontWeights.Bold;
            if (rule.Italic) run.FontStyle = System.Windows.FontStyles.Italic;
            if (rule.Underline) run.TextDecorations = System.Windows.TextDecorations.Underline;

            paragraph.Inlines.Add(run);
            currentPos = start + length;
        }

        // Add remaining text
        if (currentPos < text.Length)
        {
            paragraph.Inlines.Add(new Run(text.Substring(currentPos)));
        }
    }
}
