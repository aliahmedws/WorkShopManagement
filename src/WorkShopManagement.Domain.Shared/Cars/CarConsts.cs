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
    public const int MaxLocationStatusLength = 128;
    public const int MaxBookingNumberLength = 64;
    public const int MaxClearingAgentLength = 128;
    public const int MaxBuildMaterialNumberLength = 64;
    public const int MaxPdiStatusLength = 64;

    public const string DefaultSorting = "CreationTime DESC";

    public static string GetNormalizedSorting(string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting)) return DefaultSorting;
        if (sorting.Contains("ownerName")) sorting = sorting.Replace("ownerName", "owner.name");
        if (sorting.Contains("modelName")) sorting = sorting.Replace("modelName", "model.name");
        return sorting;
    }
}
