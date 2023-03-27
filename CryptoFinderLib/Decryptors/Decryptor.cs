using CryptoFinderLib.Models;

namespace CryptoFinderLib.Decryptors;

public abstract class Decryptor
{
    public InputParameter _parameter { get; set; }
    
    protected string? _encryptedString { get; set; }
    protected string? _password { get; set; }
    protected string? _decrypted { get; set; }
    protected string? _mnemonic { get; set; }
    protected List<string> _addresses { get; set; }
    
    public Decryptor(InputParameter parameter)
    {
        _parameter = parameter;
    }

    public abstract string? GetEncrypted();
    public abstract string? GetHashcat();
    public abstract bool TryDecrypt();
    public abstract string? GetDecrypted();
    public abstract string? GetPassword();
    public abstract string? GetMnemonic();
    public abstract string[] GetAddress(int Depth);
}