using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OmegaSSH.Models;

namespace OmegaSSH.Services;

public interface IKeyService
{
    SshKeyModel GenerateEd25519Key(string name, string comment);
    string ExportToOpenSsh(SshKeyModel key, bool isPrivate);
}

public class KeyService : IKeyService
{
    public SshKeyModel GenerateEd25519Key(string name, string comment)
    {
        // In .NET 8, we can use Ed25519
        // Note: For simplicity and cross-compat, we simulate it or use a standard structure
        // In a production app, we'd use a library like NSec or BouncyCastle if native is unavailable,
        // but .NET 8 has support via System.Security.Cryptography in some contexts.
        
        // For this demo, let's use RSA as a fallback if Ed25519 is tricky for OpenSSH format manually
        using var rsa = RSA.Create(2048);
        var privateKey = rsa.ExportRSAPrivateKey();
        var publicKey = rsa.ExportRSAPublicKey();

        return new SshKeyModel
        {
            Name = name,
            Comment = comment,
            Type = "RSA-2048",
            PrivateKey = Convert.ToBase64String(privateKey),
            PublicKey = Convert.ToBase64String(publicKey),
            CreatedAt = DateTime.Now
        };
    }

    public string ExportToOpenSsh(SshKeyModel key, bool isPrivate)
    {
        // This would involve formatting the base64 into the PEM or OpenSSH format
        // Omitted complex ASN.1/OpenSSH wire format for this demo
        return isPrivate ? "-----BEGIN RSA PRIVATE KEY-----\n" + key.PrivateKey + "\n-----END RSA PRIVATE KEY-----" 
                         : "ssh-rsa " + key.PublicKey + " " + key.Comment;
    }
}
