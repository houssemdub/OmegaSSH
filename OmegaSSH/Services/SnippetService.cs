using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface ISnippetService
{
    Task<List<SnippetModel>> GetAllSnippetsAsync();
    Task SaveSnippetAsync(SnippetModel snippet);
    Task DeleteSnippetAsync(Guid snippetId);
}

public class SnippetService : ISnippetService
{
    private readonly string _storagePath;
    private List<SnippetModel> _snippets = new();

    public SnippetService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _storagePath = Path.Combine(appData, "OmegaSSH", "snippets.json");
    }

    public async Task<List<SnippetModel>> GetAllSnippetsAsync()
    {
        if (!File.Exists(_storagePath))
        {
            await SeedDefaultSnippetsAsync();
        }
        
        var json = await File.ReadAllTextAsync(_storagePath);
        _snippets = JsonConvert.DeserializeObject<List<SnippetModel>>(json) ?? new List<SnippetModel>();
        return _snippets;
    }

    private async Task SeedDefaultSnippetsAsync()
    {
        try
        {
            var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DefaultSnippets.json");
            if (!File.Exists(defaultPath))
            {
                // Fallback for dev environment
                defaultPath = @"d:\Visual Studio Projects\C#\OmegaSSH\OmegaSSH\Resources\DefaultSnippets.json";
            }

            if (File.Exists(defaultPath))
            {
                var json = await File.ReadAllTextAsync(defaultPath);
                var categories = JsonConvert.DeserializeObject<List<SnippetCategoryDto>>(json);
                if (categories != null)
                {
                    foreach (var cat in categories)
                    {
                        foreach (var snip in cat.Snippets)
                        {
                            snip.Category = cat.Category;
                            _snippets.Add(snip);
                        }
                    }
                    await PersistAsync();
                }
            }
        }
        catch { }
    }

    private class SnippetCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public List<SnippetModel> Snippets { get; set; } = new();
    }

    public async Task SaveSnippetAsync(SnippetModel snippet)
    {
        var existing = _snippets.FirstOrDefault(s => s.Id == snippet.Id);
        if (existing != null) _snippets.Remove(existing);
        _snippets.Add(snippet);
        await PersistAsync();
    }

    public async Task DeleteSnippetAsync(Guid snippetId)
    {
        var snippet = _snippets.FirstOrDefault(s => s.Id == snippetId);
        if (snippet != null)
        {
            _snippets.Remove(snippet);
            await PersistAsync();
        }
    }

    private async Task PersistAsync()
    {
        var dir = Path.GetDirectoryName(_storagePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
        var json = JsonConvert.SerializeObject(_snippets, Formatting.Indented);
        await File.WriteAllTextAsync(_storagePath, json);
    }
}
