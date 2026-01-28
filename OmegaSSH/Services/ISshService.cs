using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface ISshService : IShellEngine
{
    Task ConnectAsync(SessionModel session);
    bool IsConnected { get; }
    
    // SFTP Methods
    Task<List<SftpEntryModel>> ListDirectoryAsync(string path);
    Task DownloadFileAsync(string remotePath, string localPath);
    Task UploadFileAsync(string localPath, string remotePath);
    void CreateTunnel(TunnelModel tunnel);
    
    // IShellEngine compatibility
    Task IShellEngine.ConnectAsync() => Task.CompletedTask; // Should use the Session version
    Task IShellEngine.WriteAsync(string input) => SendCommandAsync(input);
    Task SendCommandAsync(string command);
}
