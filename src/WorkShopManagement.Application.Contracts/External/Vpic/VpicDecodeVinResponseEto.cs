using System.Collections.Generic;

namespace WorkShopManagement.External.Vpic;

public sealed class VpicDecodeVinResponseEto
{
    public int Count { get; set; }
    public string? Message { get; set; }
    public string? SearchCriteria { get; set; }
    public List<VpicVariableResultEto> Results { get; set; } = [];
}

public sealed class VpicVariableResultEto
{
    public string? Value { get; set; }
    public string? ValueId { get; set; }
    public string? Variable { get; set; }
    public int VariableId { get; set; }
}
