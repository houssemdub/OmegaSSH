using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface ISshService : IDisposable
{
    Task ConnectAsync(SessionModel session);
    Task DisconnectAsync();
    Task SendCommandAsync(string command);
    event Action<string>? DataReceived;

    // SFTP Methods
    Task<List<SftpEntryModel>> ListDirectoryAsync(string path);
    Task DownloadFileAsync(string remotePath, string localPath);
    Task UploadFileAsync(string localPath, string remotePath);
    void CreateTunnel(TunnelModel tunnel);
}
