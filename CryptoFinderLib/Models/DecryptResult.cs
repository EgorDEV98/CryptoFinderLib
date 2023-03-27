namespace CryptoFinderLib.Models;

public class DecryptResult
{
    public string? Encrypted { get; set; }
    public string? Hashcat { get; set; }
    public bool IsDecrypted { get; set; }
    public string? Decrypted { get; set; }
    public string? Password { get; set; }
    public string? Mnemonic { get; set; }
    
    public string[] Addresses = Array.Empty<string>();
}