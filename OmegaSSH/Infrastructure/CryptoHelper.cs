using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OmegaSSH.Infrastructure;

public static class CryptoHelper
{
    private static readonly byte[] Salt = Encoding.ASCII.GetBytes("OmegaSSH_Secret_Salt_123!");

    public static string Encrypt(string plainText, string masterPassword)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(masterPassword, Salt, 10000, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                plainText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return plainText;
    }

    public static string Decrypt(string cipherText, string masterPassword)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        byte[] cipherBytes = Convert.ToBase64String(Encoding.Unicode.GetBytes(cipherText)).Length > 0 ? Convert.FromBase64String(cipherText) : Array.Empty<byte>();
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(masterPassword, Salt, 10000, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
}
