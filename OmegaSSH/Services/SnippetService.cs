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
        if (!File.Exists(_storagePath)) return new List<SnippetModel>();
        var json = await File.ReadAllTextAsync(_storagePath);
        _snippets = JsonConvert.DeserializeObject<List<SnippetModel>>(json) ?? new List<SnippetModel>();
        return _snippets;
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
