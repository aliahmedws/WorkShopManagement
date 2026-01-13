using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkShopManagement.External.CarsXe
{
    public sealed class SpecsResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("input")]
        public VinInputDto? Input { get; set; }

        [JsonPropertyName("attributes")]
        public SpecAttributesDto? Attributes { get; set; }

        [JsonPropertyName("colors")]
        public List<SpecColorDto>? Colors { get; set; }

        [JsonPropertyName("equipment")]
        public Dictionary<string, string>? Equipment { get; set; }

        [JsonPropertyName("warranties")]
        public List<SpecWarrantyDto>? Warranties { get; set; }

        [JsonPropertyName("deepdata")]
        public Dictionary<string, string>? DeepData { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }
    }

    public sealed class SpecAttributesDto
    {
        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("trim")]
        public string? Trim { get; set; }

        [JsonPropertyName("style")]
        public string? Style { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("made_in")]
        public string? MadeIn { get; set; }

        [JsonPropertyName("made_in_city")]
        public string? MadeInCity { get; set; }

        [JsonPropertyName("doors")]
        public string? Doors { get; set; }

        [JsonPropertyName("fuel_type")]
        public string? FuelType { get; set; }

        [JsonPropertyName("fuel_capacity")]
        public string? FuelCapacity { get; set; }

        [JsonPropertyName("city_mileage")]
        public string? CityMileage { get; set; }

        [JsonPropertyName("highway_mileage")]
        public string? HighwayMileage { get; set; }

        [JsonPropertyName("engine")]
        public string? Engine { get; set; }

        [JsonPropertyName("engine_cylinders")]
        public string? EngineCylinders { get; set; }

        [JsonPropertyName("transmission")]
        public string? Transmission { get; set; }

        [JsonPropertyName("drivetrain")]
        public string? Drivetrain { get; set; }

        [JsonPropertyName("curb_weight")]
        public string? CurbWeight { get; set; }

        [JsonPropertyName("overall_height")]
        public string? OverallHeight { get; set; }

        [JsonPropertyName("overall_length")]
        public string? OverallLength { get; set; }

        [JsonPropertyName("overall_width")]
        public string? OverallWidth { get; set; }

        [JsonPropertyName("wheelbase_length")]
        public string? WheelbaseLength { get; set; }

        [JsonPropertyName("standard_seating")]
        public string? StandardSeating { get; set; }

        [JsonPropertyName("production_seq_number")]
        public string? ProductionSeqNumber { get; set; }

        [JsonPropertyName("interior_trim")]
        public List<string>? InteriorTrim { get; set; }

        [JsonPropertyName("exterior_color")]
        public List<string>? ExteriorColor { get; set; }

        // Catches any extra fields like "invoice_price" or new fields added by API later
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraAttributes { get; set; }
    }

    public sealed class SpecColorDto
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public sealed class SpecWarrantyDto
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("miles")]
        public string? Miles { get; set; }

        [JsonPropertyName("months")]
        public string? Months { get; set; }
    }
}


