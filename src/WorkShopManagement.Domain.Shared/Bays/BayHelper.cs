using System.Text.RegularExpressions;

namespace WorkShopManagement.Bays;

public static class BayHelper
{
    private static readonly Regex TrailingNumberRegex = new(@"^(?<prefix>.*?)(?<num>\d+)\s*$", RegexOptions.Compiled);

    public static string GetNamePrefix(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;

        var m = TrailingNumberRegex.Match(name.Trim());
        return m.Success ? m.Groups["prefix"].Value.Trim() : name.Trim();
    }

    public static int GetTrailingNumberOrMax(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return int.MaxValue;

        var m = TrailingNumberRegex.Match(name.Trim());
        return m.Success && int.TryParse(m.Groups["num"].Value, out var n) ? n : int.MaxValue;
    }
}
