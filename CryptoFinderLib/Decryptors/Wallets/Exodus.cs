using System.Buffers.Binary;
using CryptoFinderLib.Models;

namespace CryptoFinderLib.Decryptors.Wallets;

public class Exodus : Decryptor
{
    private byte[] _seedBuffer;
    
    public Exodus(InputParameter parameter) : base(parameter)
    {
    }

    public override string? GetEncrypted()
    {
        return null;
    }

    public override string? GetHashcat()
    {
        var secoFile = Directory
            .GetFiles(base._parameter.Wallet.Directory, "seed.seco", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (secoFile == null)
            return null;
        
        _seedBuffer = File.ReadAllBytes(secoFile);
        var salt = _seedBuffer.Skip(0x100).Take(0x20).ToArray();

        var n = BinaryPrimitives.ReadInt32BigEndian(_seedBuffer.AsSpan(0x120, 4));
        var r = BinaryPrimitives.ReadInt32BigEndian(_seedBuffer.AsSpan(0x124, 4));
        var p = BinaryPrimitives.ReadInt32BigEndian(_seedBuffer.AsSpan(0x128, 4));

        if (n != 16384 || r != 8 || p != 1)
            return null;
        
        byte[] updatingBytes = _seedBuffer.Skip(256).Take(_seedBuffer.Length).ToArray();
        byte[] afterSHA = ComputeSha256Hash(updatingBytes);
        if (afterSHA.Length != 32)
            return null;
 
        var iv = _seedBuffer.Skip(0x14c).Take(0xc).ToArray();
        var authTag = _seedBuffer.Skip(0x158).Take(0x10).ToArray();
        var key = _seedBuffer.Skip(0x168).Take(0x20).ToArray();

        var ivBase64 = Convert.ToBase64String(iv);
        var authTagBase64 = Convert.ToBase64String(authTag);
        var keyBase64 = Convert.ToBase64String(key);
        var saltBase64 = Convert.ToBase64String(salt);
        
        return $"EXODUS:{n}:{r}:{p}:{saltBase64}:{ivBase64}:{keyBase64}:{authTagBase64}";
    }

    public override bool TryDecrypt()
    {
        return false;
    }

    public override string? GetDecrypted()
    {
        return null;
    }

    public override string? GetPassword()
    {
        return null;
    }

    public override string? GetMnemonic()
    {
        return null;
    }

    public override string[] GetAddress(int Depth)
    {
        return Array.Empty<string>();
    }
    
    
    private byte[] ComputeSha256Hash(byte[] data)
    {
        using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
        {
            return  sha256Hash.ComputeHash(data);
        }
    }
}