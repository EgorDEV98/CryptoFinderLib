using CryptoFinderLib.Enums;

namespace CryptoFinderLib.Models;

public class Wallet
{
    /// <summary>
    /// Путь до папки
    /// </summary>
    public string Directory { get; set; }
    
    /// <summary>
    /// Провайдер кошелька
    /// </summary>
    public WalletClient? WalletClient { get; set; }
    
    /// <summary>
    /// Строковое представление
    /// </summary>
    public string? WalletClientDisplay { get; set; }
}