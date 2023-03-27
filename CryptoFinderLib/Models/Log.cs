namespace CryptoFinderLib.Models;

public class Log
{
    /// <summary>
    /// Папка с логом
    /// </summary>
    public string? Directory { get; set; }
    
    /// <summary>
    /// Информация о логе
    /// </summary>
    public Information? Information { get; set; }
    /// <summary>
    /// Пароли
    /// </summary>
    public string[]? Passwords { get; set; }
    /// <summary>
    /// Кошельки
    /// </summary>
    public Wallet[]? Wallets { get; set; }
}