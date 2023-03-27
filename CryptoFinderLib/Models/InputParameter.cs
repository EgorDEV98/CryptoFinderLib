namespace CryptoFinderLib.Models;

public class InputParameter
{
    public string[] Password { get; set; }
    public Wallet Wallet { get; set; }
    public bool TryBrute { get; set; }
    public int Depth { get; set; }
}