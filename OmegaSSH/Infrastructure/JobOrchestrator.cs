using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmegaSSH.Models;
using OmegaSSH.Services;

namespace OmegaSSH.Infrastructure;

public class JobResult
{
    public SessionModel Session { get; set; } = null!;
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}

public interface IJobOrchestrator
{
    Task<List<JobResult>> ExecuteParallelAsync(IEnumerable<SessionModel> sessions, string command, Action<JobResult>? onProgress = null, CancellationToken ct = default);
}

public class JobOrchestrator : IJobOrchestrator
{
    private readonly ISshService _sshService;

    public JobOrchestrator(ISshService sshService)
    {
        _sshService = sshService;
    }

    public async Task<List<JobResult>> ExecuteParallelAsync(IEnumerable<SessionModel> sessions, string command, Action<JobResult>? onProgress = null, CancellationToken ct = default)
    {
        var tasks = new List<Task<JobResult>>();

        foreach (var session in sessions)
        {
            tasks.Add(Task.Run(async () =>
            {
                var result = new JobResult { Session = session };
                try
                {
                    // For parallel execution, we might need separate ssh service instances or a stateless execution method
                    // Assuming ISshService can handle concurrent requests if implemented correctly or using a temporary connection
                    // For now, let's simulate with a new connection since it's cleaner for parallel jobs
                    
                    // In a real scenario, we'd use a factory to create a light-weight ssh client
                    // For now, we'll try to use the shared ssh logic but connect separately
                    
                    var output = await ExecuteOneOffCommand(session, command, ct);
                    result.Success = true;
                    result.Output = output;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Error = ex.Message;
                }

                onProgress?.Invoke(result);
                return result;
            }, ct));
        }

        var results = await Task.WhenAll(tasks);
        return new List<JobResult>(results);
    }

    private async Task<string> ExecuteOneOffCommand(SessionModel session, string command, CancellationToken ct)
    {
        // Simple one-off connection using Renci.SshNet
        return await Task.Run(() =>
        {
            using var client = new Renci.SshNet.SshClient(session.Host, session.Port, session.Username, session.Password ?? "");
            client.Connect();
            
            var cmd = client.CreateCommand(command);
            var result = cmd.Execute();
            
            client.Disconnect();
            return result;
        }, ct);
    }
}
