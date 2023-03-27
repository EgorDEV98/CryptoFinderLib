using CryptoFinderLib.Decryptors;
using CryptoFinderLib.Decryptors.Wallets;
using CryptoFinderLib.Enums;
using CryptoFinderLib.Models;


namespace CryptoFinderLib;

public class WalletDecryptor
{
    private InputParameter _parameter { get; }
    
    public WalletDecryptor(InputParameter parameter)
    {
        _parameter = parameter;
    }
    
    public DecryptResult? Decrypt()
    {
        Decryptor decryptor;
        switch (_parameter.Wallet.WalletClient)
        {
            case WalletClient.Metamask:
                decryptor = new Metamask(_parameter);
                break;
            // case WalletClient.Ronin:
            //     decryptor = new Ronin(_wallet.Directory, _passwords);
            //     break;
            // case WalletClient.Brave:
            //     decryptor = new Brave(_wallet.Directory, _passwords);
            //     break;
            
            default:
                return null;
        }

        return new DecryptResult()
        {
            Encrypted = decryptor?.GetEncrypted(),
            Hashcat = decryptor?.GetHashcat(),
            IsDecrypted = _parameter.TryBrute ? decryptor.TryDecrypt() : false,
            Decrypted = decryptor?.GetDecrypted(),
            Password = decryptor?.GetPassword(),
            Mnemonic = decryptor?.GetMnemonic(),
            Addresses = decryptor?.GetAddress(_parameter.Depth)
        };
    }
}




