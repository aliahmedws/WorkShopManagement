namespace WorkShopManagement.External.Vpic;

public class VpicVariableResultDto
{
    public string? Model { get; set; }
    public string? ModelYear { get; set; }
    public string? SuggestedVin { get; set; }
    public string? Error { get; set; }
    public bool Success => string.IsNullOrWhiteSpace(Error);
}
