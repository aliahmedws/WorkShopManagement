namespace WorkShopManagement.Cars;

public static class CarConsts
{
    public const int VinLength = 17;
    public const int MaxColorLength = 64;

    public const int MaxCncLength = 64;
    public const int MaxCncFirewallLength = 64;
    public const int MaxCncColumnLength = 64;

    public const int MaxNotesLength = 4000;
    public const int MaxMissingPartsLength = 4000;

    public const int MinModelYear = 1886;
    public const int MaxModelYear = 2100;

    public const string DefaultSorting = "CreationTime DESC";

    public static string GetDefaultSorting(string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting)) return DefaultSorting;
        return sorting;
    }
}
