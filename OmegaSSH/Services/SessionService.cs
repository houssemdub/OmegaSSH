using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public class SessionService : ISessionService
{
    private readonly string _storagePath;
    private List<SessionModel> _sessions = new();

    public SessionService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _storagePath = Path.Combine(appData, "OmegaSSH", "sessions.json");
        EnsureDirectoryExists();
    }

    private void EnsureDirectoryExists()
    {
        var dir = Path.GetDirectoryName(_storagePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir!);
        }
    }

    public async Task<List<SessionModel>> GetAllSessionsAsync()
    {
        if (!File.Exists(_storagePath)) return new List<SessionModel>();

        var json = await File.ReadAllTextAsync(_storagePath);
        _sessions = JsonConvert.DeserializeObject<List<SessionModel>>(json) ?? new List<SessionModel>();
        return _sessions;
    }

    public async Task SaveSessionAsync(SessionModel session)
    {
        var existing = _sessions.FirstOrDefault(s => s.Id == session.Id);
        if (existing != null)
        {
            _sessions.Remove(existing);
        }
        _sessions.Add(session);
        await PersistAsync();
    }

    public async Task DeleteSessionAsync(Guid sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            _sessions.Remove(session);
            await PersistAsync();
        }
    }

    private async Task PersistAsync()
    {
        var json = JsonConvert.SerializeObject(_sessions, Formatting.Indented);
        await File.WriteAllTextAsync(_storagePath, json);
    }
}
