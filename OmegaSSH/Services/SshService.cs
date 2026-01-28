using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public class SshService : ISshService, IDisposable
{
    private SshClient? _client;
    private SftpClient? _sftpClient;
    private ShellStream? _shellStream;
    public event Action<string>? DataReceived;
    public event Action? Disconnected;

    public bool IsConnected => _client != null && _client.IsConnected;

    public async Task ConnectAsync(SessionModel session)
    {
        await Task.Run(() =>
        {
            var connectionInfo = string.IsNullOrEmpty(session.PrivateKeyPath)
                ? new ConnectionInfo(session.Host, session.Port, session.Username,
                    new PasswordAuthenticationMethod(session.Username, session.Password ?? ""))
                : new ConnectionInfo(session.Host, session.Port, session.Username,
                    new PrivateKeyAuthenticationMethod(session.Username, new PrivateKeyFile(session.PrivateKeyPath)));

            _client = new SshClient(connectionInfo);
            _client.ErrorOccurred += (s, e) => Disconnected?.Invoke();
            _client.Connect();

            _sftpClient = new SftpClient(connectionInfo);
            _sftpClient.Connect();

            _shellStream = _client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
            _shellStream.DataReceived += (sender, e) =>
            {
                var text = Encoding.UTF8.GetString(e.Data);
                DataReceived?.Invoke(text);
            };
        });
    }

    public async Task<List<SftpEntryModel>> ListDirectoryAsync(string path)
    {
        if (_sftpClient == null || !_sftpClient.IsConnected) return new List<SftpEntryModel>();

        return await Task.Run(() =>
        {
            var files = _sftpClient.ListDirectory(path);
            return files.Select(f => new SftpEntryModel
            {
                Name = f.Name,
                FullPath = f.FullName,
                IsDirectory = f.IsDirectory,
                Size = f.Length,
                LastWriteTime = f.LastWriteTime,
                Permissions = f.UserId.ToString() // Simplification
            }).ToList();
        });
    }

    public async Task DownloadFileAsync(string remotePath, string localPath)
    {
        if (_sftpClient == null || !_sftpClient.IsConnected) return;

        await Task.Run(() =>
        {
            using var file = File.OpenWrite(localPath);
            _sftpClient.DownloadFile(remotePath, file);
        });
    }

    public async Task UploadFileAsync(string localPath, string remotePath)
    {
        if (_sftpClient == null || !_sftpClient.IsConnected) return;

        await Task.Run(() =>
        {
            using var file = File.OpenRead(localPath);
            _sftpClient.UploadFile(file, remotePath);
        });
    }

    public void CreateTunnel(TunnelModel tunnel)
    {
        if (_client == null || !_client.IsConnected) return;

        var port = new ForwardedPortLocal("127.0.0.1", (uint)tunnel.LocalPort, tunnel.RemoteHost, (uint)tunnel.RemotePort);
        _client.AddForwardedPort(port);
        port.Start();
    }

    public async Task SendCommandAsync(string command)
    {
        if (_shellStream != null && _shellStream.CanWrite)
        {
            await Task.Run(() =>
            {
                _shellStream.Write(command);
                _shellStream.Flush();
            });
        }
    }

    public async Task DisconnectAsync()
    {
        await Task.Run(() =>
        {
            _shellStream?.Dispose();
            _sftpClient?.Disconnect();
            _sftpClient?.Dispose();
            _client?.Disconnect();
            _client?.Dispose();
        });
    }

    public void Dispose()
    {
        _shellStream?.Dispose();
        _sftpClient?.Dispose();
        _client?.Dispose();
    }
}
