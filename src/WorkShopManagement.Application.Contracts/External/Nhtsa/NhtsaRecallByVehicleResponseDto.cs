using System.Collections.Generic;

namespace WorkShopManagement.External.Nhtsa;

public class NhtsaRecallByVehicleResponseDto
{
    public int Count { get; set; }

    public string? Message { get; set; }

    public List<NhtsaRecallByVehicleResultDto> Results { get; set; } = [];
}

public sealed class NhtsaRecallByVehicleResultDto
{
    public string? Manufacturer { get; set; }
    public string? NHTSACampaignNumber { get; set; }

    public bool ParkIt { get; set; }

    public bool ParkOutSide { get; set; }
    
    public bool OverTheAirUpdate { get; set; }

    public string? NHTSAActionNumber { get; set; }

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
