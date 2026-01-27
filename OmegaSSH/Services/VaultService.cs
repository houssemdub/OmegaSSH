using System;

namespace OmegaSSH.Services;

public interface IVaultService
{
    bool IsUnlocked { get; }
    void Unlock(string password);
    void Lock();
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class VaultService : IVaultService
{
    private string? _masterPassword;

    public bool IsUnlocked => !string.IsNullOrEmpty(_masterPassword);

    public void Unlock(string password)
    {
        // In a real app, verify against a stored hash
        _masterPassword = password;
    }

    public void Lock()
    {
        _masterPassword = null;
    }

    public string Encrypt(string plainText)
    {
        if (!IsUnlocked) throw new InvalidOperationException("Vault is locked");
        return Infrastructure.CryptoHelper.Encrypt(plainText, _masterPassword!);
    }

    public string Decrypt(string cipherText)
    {
        if (!IsUnlocked) throw new InvalidOperationException("Vault is locked");
        return Infrastructure.CryptoHelper.Decrypt(cipherText, _masterPassword!);
    }
}
