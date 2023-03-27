using CryptoFinderLib.Enums;
using CryptoFinderLib.Models;

namespace CryptoFinderLib.Methods;

public class WalletGrabber
{
    private static object lockObject = new object();

    public static Wallet[] Grab(string directory)
    {
        if (directory == null)
        {
            return null;
        }

        var wallets = Directory.GetDirectories(directory);
        if (wallets.Length == 0)
        {
            return null;
        }

        var walletProviders = Enum.GetNames<WalletClient>();

        var result = new List<Wallet>();

        Parallel.ForEach(wallets, wallet =>
        {
            var walletName = Path.GetFileName(wallet);
            var walletProvider = walletProviders.FirstOrDefault(x => walletName.Contains(x, StringComparison.OrdinalIgnoreCase));
            if (walletProvider != null && Enum.TryParse<WalletClient>(walletProvider, out var walletClient))
            {
                lock (lockObject)
                {
                    result.Add(new Wallet
                    {
                        Directory = wallet,
                        WalletClient = walletClient,
                        WalletClientDisplay = walletProvider
                    });
                }
            }
        });

        return result.ToArray();
    }
}