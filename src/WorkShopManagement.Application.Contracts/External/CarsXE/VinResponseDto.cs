using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkShopManagement.External.CarsXE
{
    public sealed class VinResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("input")]
        public CarsXeVinInputDto? Input { get; set; }

        // Docs show this as "attributes" object containing vin/make/model/year/... :contentReference[oaicite:2]{index=2}
        [JsonPropertyName("attributes")]
        public VinAttributesDto? Attributes { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }
    }

    public sealed class CarsXeVinInputDto
    {
        [JsonPropertyName("vin")]
        public string? Vin { get; set; }
    }

    public sealed class VinAttributesDto
    {
        [JsonPropertyName("vin")]
        public string? Vin { get; set; }

        [JsonPropertyName("vid")]
        public string? Vid { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("product_type")]
        public string? ProductType { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("series")]
        public string? Series { get; set; }

        [JsonPropertyName("fuel_type")]
        public string? FuelType { get; set; }

        [JsonPropertyName("gears")]
        public string? Gears { get; set; }

        [JsonPropertyName("emission_standard")]
        public string? EmissionStandard { get; set; }

        [JsonPropertyName("manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("manufacturer_address")]
        public string? ManufacturerAddress { get; set; }

        [JsonPropertyName("plant_country")]
        public string? PlantCountry { get; set; }

        [JsonPropertyName("engine_manufacturer")]
        public string? EngineManufacturer { get; set; }

        [JsonPropertyName("avg_co2_emission_g_km")]
        public string? AvgCo2EmissionGKm { get; set; }

        [JsonPropertyName("no_of_axels")]
        public string? NoOfAxels { get; set; }

        [JsonPropertyName("no_of_doors")]
        public string? NoOfDoors { get; set; }

        [JsonPropertyName("no_of_seats")]
        public string? NoOfSeats { get; set; }

        [JsonPropertyName("rear_brakes")]
        public string? RearBrakes { get; set; }

        [JsonPropertyName("steering_type")]
        public string? SteeringType { get; set; }

        [JsonPropertyName("rear_suspension")]
        public string? RearSuspension { get; set; }

        [JsonPropertyName("front_suspension")]
        public string? FrontSuspension { get; set; }

        [JsonPropertyName("wheel_size")]
        public string? WheelSize { get; set; }

        [JsonPropertyName("wheel_size_array")]
        public string? WheelSizeArray { get; set; }

        [JsonPropertyName("wheelbase_mm")]
        public string? WheelbaseMm { get; set; }

        [JsonPropertyName("wheelbase_array_mm")]
        public string? WheelbaseArrayMm { get; set; }

        [JsonPropertyName("height_mm")]
        public string? HeightMm { get; set; }

        [JsonPropertyName("length_mm")]
        public string? LengthMm { get; set; }

        [JsonPropertyName("width_mm")]
        public string? WidthMm { get; set; }

        [JsonPropertyName("track_front_mm")]
        public string? TrackFrontMm { get; set; }

        [JsonPropertyName("track_rear_mm")]
        public string? TrackRearMm { get; set; }

        [JsonPropertyName("max_speed_kmh")]
        public string? MaxSpeedKmh { get; set; }

        [JsonPropertyName("max_trunk_capacity_liters")]
        public string? MaxTrunkCapacityLiters { get; set; }

        [JsonPropertyName("min_trunk_capacity_liters")]
        public string? MinTrunkCapacityLiters { get; set; }

        [JsonPropertyName("weight_empty_kg")]
        public string? WeightEmptyKg { get; set; }

        [JsonPropertyName("max_weight_kg")]
        public string? MaxWeightKg { get; set; }

        [JsonPropertyName("max_roof_load_kg")]
        public string? MaxRoofLoadKg { get; set; }

        [JsonPropertyName("permitted_trailer_load_without_brakes_kg")]
        public string? PermittedTrailerLoadWithoutBrakesKg { get; set; }

        [JsonPropertyName("abs")]
        public string? Abs { get; set; }

        [JsonPropertyName("check_digit")]
        public string? CheckDigit { get; set; }

        [JsonPropertyName("sequential_number")]
        public string? SequentialNumber { get; set; }

        // Important: CarsXE may add more fields over time; this keeps you forward-compatible.
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extra { get; set; }
    }
}
