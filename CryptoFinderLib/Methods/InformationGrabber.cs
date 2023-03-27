using CryptoFinderLib.Models;

namespace CryptoFinderLib.Methods;

public static class InformationGrabber
{
    public static Information? Grab(string[] files)
    {
        var informationFile = files.FirstOrDefault(x => x.Contains("info", StringComparison.OrdinalIgnoreCase));
        if (informationFile == null)
        {
            return null;
        }
        
        var infoText = File.ReadAllLines(informationFile);
        
        return new Information
        {
            IpAddress = infoText.GetDataSubString(new[] { "ip:" }),
            Country = infoText.GetDataSubString(new[] { "country:" }),
            Location = infoText.GetDataSubString(new []{ "location:" }),
            Language = infoText.GetDataSubString(new []{ "Current Language:", "Keyboard Languages:" }),
            OperationSystem = infoText.GetDataSubString(new []{ "Operation System:", "Windows:", "OS:" }),
            ZipCode = infoText.GetDataSubString(new[] { "Zip Code:", "ZIP:" }),
            LogDate = infoText.GetDataSubString(new [] { "Local Time:", "Log date:", "DateTime:" })
        };
    }

    private static string? GetDataSubString(this string[] str, string[] inner)
    {
        var foundString = str
            .FirstOrDefault(x => inner
                .Select(i => i.ToLower())
                .Any(x.ToLower().StartsWith));

        if (foundString == null || !foundString.Contains(":"))
            return null;

        return foundString?.Split(":").Last();
    }
}
