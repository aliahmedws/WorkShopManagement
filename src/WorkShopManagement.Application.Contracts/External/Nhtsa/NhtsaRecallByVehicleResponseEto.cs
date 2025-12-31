using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkShopManagement.External.Nhtsa;

public sealed class NhtsaRecallByVehicleResponseEto
{
    public int Count { get; set; }

    public string? Message { get; set; }

    // JSON uses lowercase "results"
    [JsonPropertyName("results")]
    public List<NhtsaRecallByVehicleResultEto> Results { get; set; } = [];
}

public sealed class NhtsaRecallByVehicleResultEto
{
    public string? Manufacturer { get; set; }
    public string? NHTSACampaignNumber { get; set; }

    // JSON uses camelCase for these three
    [JsonPropertyName("parkIt")]
    public bool ParkIt { get; set; }

    [JsonPropertyName("parkOutSide")]
    public bool ParkOutSide { get; set; }

    [JsonPropertyName("overTheAirUpdate")]
    public bool OverTheAirUpdate { get; set; }

    public string? NHTSAActionNumber { get; set; }

    // Comes as "06/03/2019"
    public string? ReportReceivedDate { get; set; }

    public string? Component { get; set; }
    public string? Summary { get; set; }
    public string? Consequence { get; set; }
    public string? Remedy { get; set; }
    public string? Notes { get; set; }

    public string? ModelYear { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
}
