using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface ISessionService
{
    Task<List<SessionModel>> GetAllSessionsAsync();
    Task SaveSessionAsync(SessionModel session);
    Task DeleteSessionAsync(Guid sessionId);
}
