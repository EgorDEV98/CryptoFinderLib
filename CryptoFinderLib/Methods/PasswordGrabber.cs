namespace CryptoFinderLib.Methods;

public class PasswordGrabber
{
    public static string[] Grab(IEnumerable<string> _files)
    {
        var passwordFile = _files.FirstOrDefault(x => x.Contains("assword", StringComparison.OrdinalIgnoreCase));
        if (passwordFile == null)
        {
            return null;
        }

        var passLines = File.ReadAllLines(passwordFile)
            .Where(x => x.StartsWith("pass", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Split(' ').Last())
            .Distinct()
            .ToArray();

        return passLines;
    }
}