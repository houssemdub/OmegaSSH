using Newtonsoft.Json;
using OmegaSSH.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OmegaSSH.Services;

public interface IKeyStorageService
{
    Task<List<SshKeyModel>> LoadKeysAsync();
    Task SaveKeysAsync(List<SshKeyModel> keys);
}

public class KeyStorageService : IKeyStorageService
{
    private readonly string _filePath;

    public KeyStorageService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "OmegaSSH");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "keys.json");
    }

    public async Task<List<SshKeyModel>> LoadKeysAsync()
    {
        if (!File.Exists(_filePath)) return new List<SshKeyModel>();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonConvert.DeserializeObject<List<SshKeyModel>>(json) ?? new List<SshKeyModel>();
    }

    public async Task SaveKeysAsync(List<SshKeyModel> keys)
    {
        var json = JsonConvert.SerializeObject(keys, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
