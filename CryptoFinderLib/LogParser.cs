using CryptoFinderLib.Methods;
using CryptoFinderLib.Models;

namespace CryptoFinderLib;

public class LogParser
{
    private string _directory  { get; }
    private string[] _files { get; }
    
    
    public LogParser(string directory)
    {
        _directory = directory;
        _files = Directory
            .GetFiles(directory, "*", SearchOption.AllDirectories)
            .ToArray();
    }

    public Log Parse()
    {
        var log = new Log();
        log.Directory = _directory;
        log.Information = InformationGrabber.Grab(_files);
        log.Passwords = PasswordGrabber.Grab(_files);
        log.Wallets = WalletGrabber.Grab(_directory);

        return log;
    }
}

