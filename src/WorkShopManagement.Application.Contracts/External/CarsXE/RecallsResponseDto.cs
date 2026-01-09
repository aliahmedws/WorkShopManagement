using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WorkShopManagement.External.CarsXE
{
    public sealed class RecallsResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("input")]
        public RecallsInputDto? Input { get; set; }

        [JsonPropertyName("data")]
        public RecallsDataDto? Data { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }
    }

    public sealed class RecallsInputDto
    {
        // Docs sample includes key + vin inside input :contentReference[oaicite:4]{index=4}
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("vin")]
        public string? Vin { get; set; }
    }

    public sealed class RecallsDataDto
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("vin")]
        public string? Vin { get; set; }

        [JsonPropertyName("manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("model_year")]
        public string? ModelYear { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("has_recalls")]
        public bool HasRecalls { get; set; }

        // Docs show this as a number in the example (1) :contentReference[oaicite:5]{index=5}
        [JsonPropertyName("recall_count")]
        public int RecallCount { get; set; }

        [JsonPropertyName("recalls")]
        public List<CarsXeRecallItemDto>? Recalls { get; set; }
    }

    public sealed class CarsXeRecallItemDto
    {
        [JsonPropertyName("recall_date")]
        public string? RecallDate { get; set; }

        [JsonPropertyName("expiration_date")]
        public string? ExpirationDate { get; set; }

        [JsonPropertyName("nhtsa_id")]
        public string? NhtsaId { get; set; }

        [JsonPropertyName("manufacturer_id")]
        public string? ManufacturerId { get; set; }

        [JsonPropertyName("recall_campaign_type")]
        public string? RecallCampaignType { get; set; }

        [JsonPropertyName("recall_name")]
        public string? RecallName { get; set; }

        [JsonPropertyName("component")]
        public string? Component { get; set; }

        [JsonPropertyName("recall_description")]
        public string? RecallDescription { get; set; }

        [JsonPropertyName("risk_description")]
        public string? RiskDescription { get; set; }

        // Example shows nulls; keep nullable
        [JsonPropertyName("stop_sale")]
        public bool? StopSale { get; set; }

        [JsonPropertyName("dont_drive")]
        public bool? DontDrive { get; set; }

        [JsonPropertyName("remedy_available")]
        public bool? RemedyAvailable { get; set; }

        [JsonPropertyName("recall_remedy")]
        public string? RecallRemedy { get; set; }

        [JsonPropertyName("parts_available")]
        public bool? PartsAvailable { get; set; }

        [JsonPropertyName("labor_hours_min")]
        public int? LaborHoursMin { get; set; }

        [JsonPropertyName("labor_hours_max")]
        public int? LaborHoursMax { get; set; }

        [JsonPropertyName("recall_status")]
        public string? RecallStatus { get; set; }
    }
}
