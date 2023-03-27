﻿using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CryptoFinderLib.Extensions;
using CryptoFinderLib.Models;
using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace CryptoFinderLib.Decryptors.Wallets;

public class Metamask : Decryptor
{
    private const string pattern = @"{""vault"":""([0-9a-zA-z+.""={},'/:]+)""},""[\w]+Controller""";

    private Hash _hash { get; set; }
    
    public Metamask(InputParameter parameter) : base(parameter)
    {
        
    }


    public override string? GetEncrypted()
    {
        var walletFile = Directory
            .EnumerateFiles(base._parameter.Wallet.Directory, "*", SearchOption.AllDirectories)
            .FirstOrDefault(x => x.EndsWith(".log", StringComparison.OrdinalIgnoreCase));

        if (walletFile == null) return null;

        var readFile = File.ReadAllText(walletFile);
        var match = Regex.Match(readFile, pattern);

        if (!match.Success) return null;

        _encryptedString = match.Groups[1].Value.Replace("\\", "");
        return _encryptedString;
    }

    public override string? GetHashcat()
    {
        try
        {
            _hash = JsonConvert.DeserializeObject<Hash>(_encryptedString);
            return JsonConvert
                .DeserializeObject<HashCat>(_encryptedString)
                ?.ToString();
        }
        catch
        {
            return null;
        }
    }

    public override bool TryDecrypt()
    {
        if (base._parameter.Password == null)
            return false;
        
        CancellationTokenSource cts = new CancellationTokenSource();
        object lockObject = new object();

        try
        {
            Parallel.ForEach(base._parameter.Password, new ParallelOptions { CancellationToken = cts.Token }, password =>
            {
                try
                {
                    byte[] key = PBKDF2.DeriveKey(Encoding.UTF8.GetBytes(password), _hash.salt, 10000, 256, HMAC.Create("HMACSHA256"));
                    GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
                    AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, _hash.iv, null);
                    gcmBlockCipher.Init(false, parameters);
                    byte[] plainBytes = new byte[gcmBlockCipher.GetOutputSize(_hash.data.Length)];
                    int retLen = gcmBlockCipher.ProcessBytes(_hash.data, 0, _hash.data.Length, plainBytes, 0);
                    gcmBlockCipher.DoFinal(plainBytes, retLen);

                    lock(lockObject)
                    {
                        _password = password;
                        _decrypted = Encoding.UTF8.GetString(plainBytes);
                    }

                    cts.Cancel();
                }
                catch { }
            });
        }
        catch{ }
        
        return _decrypted == null ? false : true;
    }

    public override string? GetDecrypted()
    {
        return _decrypted;
    }

    public override string? GetPassword()
    {
        return _password;
    }

    public override string? GetMnemonic()
    {
        try
        {
            var jObject = JArray.Parse(_decrypted);
            var mnemonicObject = jObject.FindTokens("mnemonic")
                .FirstOrDefault();
            if (mnemonicObject?.Type == JTokenType.Array)
            {
                byte[] bytes = mnemonicObject
                    .Select(x => (int)x)
                    .ToArray()
                    .SelectMany(BitConverter.GetBytes)
                    .Where(x => x != 0)
                    .ToArray();
                
                var mnemonic = Encoding.UTF8.GetString(bytes);
                _ = mnemonic;
                return mnemonic;
            }
            else
            {
                var mnemonic = mnemonicObject?.Value<string>();
                _mnemonic = mnemonic;
                return mnemonic;
            }
        }
        catch
        {
            return null;
        }
    }

    public override string[] GetAddress(int depth)
    {
        List<string> Addresses = new();
        try
        {
            var addressesFromFile = GetAddressFromFile();
            if (addressesFromFile != null)
            {
                Addresses.AddRange(addressesFromFile);
            }
            
            var generatingWallet = new Nethereum.HdWallet.Wallet(_mnemonic, null);
            var addresses = generatingWallet.GetAddresses(depth);
            Addresses.AddRange(addresses);
        }
        catch { }
        return Addresses
            .Distinct()
            .ToArray();
    }
    
    private List<string>? GetAddressFromFile()
    {
        try
        {
            if (_parameter.Wallet.Directory == null) return null;

            var walletFile = Directory
                .EnumerateFiles(_parameter.Wallet.Directory, "*", SearchOption.AllDirectories)
                .FirstOrDefault(x => x.EndsWith(".log", StringComparison.OrdinalIgnoreCase));
            
            var fullJson = File.ReadAllText(walletFile);

            var cachedBalanceText = fullJson.Substring(fullJson.IndexOf("{\"cachedBalances\""), 5000);

            for (int a = 1; a < 1000; a++)
            {
                try
                {
                    var jsonString = cachedBalanceText.Substring(0, a);
                    var obj = JObject.Parse(jsonString);

                    var regexAddresses = Regex.Matches(obj.ToString(), "(0x[a-zA-Z0-9]{40})")
                        .Select(x => x.ToString())
                        .ToList();

                    return regexAddresses;
                }
                catch { }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    
    private class Hash
    {
        public byte[]? iv { get; set; }
        public byte[]? salt { get; set; }
        public byte[]? data { get; set; }
    }
    private class HashCat
    {
        public string? data { get; set; }
        public string? iv { get; set; }
        public string? salt { get; set; }
        
        public override string? ToString()
        {
            return $"$metamask${salt}${iv}${data}";
        }
    }
}