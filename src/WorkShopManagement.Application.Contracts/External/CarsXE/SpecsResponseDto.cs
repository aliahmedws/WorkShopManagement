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
        public SpecEquipmentDto? Equipment { get; set; }         

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

    public sealed class SpecEquipmentDto
    {
        [JsonPropertyName("4wd_awd")]
        public string? FourWdAwd { get; set; }

        [JsonPropertyName("abs_brakes")]
        public string? AbsBrakes { get; set; }

        [JsonPropertyName("adjustable_foot_pedals")]
        public string? AdjustableFootPedals { get; set; }

        [JsonPropertyName("air_conditioning")]
        public string? AirConditioning { get; set; }

        [JsonPropertyName("alloy_wheels")]
        public string? AlloyWheels { get; set; }

        [JsonPropertyName("am_fm_radio")]
        public string? AmFmRadio { get; set; }

        [JsonPropertyName("automatic_headlights")]
        public string? AutomaticHeadlights { get; set; }

        [JsonPropertyName("automatic_load_leveling")]
        public string? AutomaticLoadLeveling { get; set; }

        [JsonPropertyName("cargo_area_cover")]
        public string? CargoAreaCover { get; set; }

        [JsonPropertyName("cargo_area_tiedowns")]
        public string? CargoAreaTiedowns { get; set; }

        [JsonPropertyName("cargo_net")]
        public string? CargoNet { get; set; }

        [JsonPropertyName("cassette_player")]
        public string? CassettePlayer { get; set; }

        [JsonPropertyName("cd_changer")]
        public string? CdChanger { get; set; }

        [JsonPropertyName("cd_player")]
        public string? CdPlayer { get; set; }

        [JsonPropertyName("child_safety_door_locks")]
        public string? ChildSafetyDoorLocks { get; set; }

        [JsonPropertyName("chrome_wheels")]
        public string? ChromeWheels { get; set; }

        [JsonPropertyName("cruise_control")]
        public string? CruiseControl { get; set; }

        [JsonPropertyName("daytime_running_lights")]
        public string? DaytimeRunningLights { get; set; }

        [JsonPropertyName("deep_tinted_glass")]
        public string? DeepTintedGlass { get; set; }

        [JsonPropertyName("driver_airbag")]
        public string? DriverAirbag { get; set; }

        [JsonPropertyName("driver_multi_adjustable_power_seat")]
        public string? DriverMultiAdjustablePowerSeat { get; set; }

        [JsonPropertyName("dvd_player")]
        public string? DvdPlayer { get; set; }

        [JsonPropertyName("electrochromic_exterior_rearview_mirror")]
        public string? ElectrochromicExteriorRearviewMirror { get; set; }

        [JsonPropertyName("electrochromic_interior_rearview_mirror")]
        public string? ElectrochromicInteriorRearviewMirror { get; set; }

        [JsonPropertyName("electronic_brake_assistance")]
        public string? ElectronicBrakeAssistance { get; set; }

        [JsonPropertyName("electronic_parking_aid")]
        public string? ElectronicParkingAid { get; set; }

        [JsonPropertyName("first_aid_kit")]
        public string? FirstAidKit { get; set; }

        [JsonPropertyName("fog_lights")]
        public string? FogLights { get; set; }

        [JsonPropertyName("front_air_dam")]
        public string? FrontAirDam { get; set; }

        [JsonPropertyName("front_cooled_seat")]
        public string? FrontCooledSeat { get; set; }

        [JsonPropertyName("front_heated_seat")]
        public string? FrontHeatedSeat { get; set; }

        [JsonPropertyName("front_power_lumbar_support")]
        public string? FrontPowerLumbarSupport { get; set; }

        [JsonPropertyName("front_power_memory_seat")]
        public string? FrontPowerMemorySeat { get; set; }

        [JsonPropertyName("front_side_airbag")]
        public string? FrontSideAirbag { get; set; }

        [JsonPropertyName("front_side_airbag_with_head_protection")]
        public string? FrontSideAirbagWithHeadProtection { get; set; }

        [JsonPropertyName("front_split_bench_seat")]
        public string? FrontSplitBenchSeat { get; set; }

        [JsonPropertyName("full_size_spare_tire")]
        public string? FullSizeSpareTire { get; set; }

        [JsonPropertyName("genuine_wood_trim")]
        public string? GenuineWoodTrim { get; set; }

        [JsonPropertyName("glass_rear_window_on_convertible")]
        public string? GlassRearWindowOnConvertible { get; set; }

        [JsonPropertyName("heated_exterior_mirror")]
        public string? HeatedExteriorMirror { get; set; }

        [JsonPropertyName("heated_steering_wheel")]
        public string? HeatedSteeringWheel { get; set; }

        [JsonPropertyName("high_intensity_discharge_headlights")]
        public string? HighIntensityDischargeHeadlights { get; set; }

        [JsonPropertyName("interval_wipers")]
        public string? IntervalWipers { get; set; }

        [JsonPropertyName("keyless_entry")]
        public string? KeylessEntry { get; set; }

        [JsonPropertyName("leather_seat")]
        public string? LeatherSeat { get; set; }

        [JsonPropertyName("leather_steering_wheel")]
        public string? LeatherSteeringWheel { get; set; }

        [JsonPropertyName("limited_slip_differential")]
        public string? LimitedSlipDifferential { get; set; }

        [JsonPropertyName("load_bearing_exterior_rack")]
        public string? LoadBearingExteriorRack { get; set; }

        [JsonPropertyName("locking_differential")]
        public string? LockingDifferential { get; set; }

        [JsonPropertyName("locking_pickup_truck_tailgate")]
        public string? LockingPickupTruckTailgate { get; set; }

        [JsonPropertyName("manual_sunroof")]
        public string? ManualSunroof { get; set; }

        [JsonPropertyName("navigation_aid")]
        public string? NavigationAid { get; set; }

        [JsonPropertyName("passenger_airbag")]
        public string? PassengerAirbag { get; set; }

        [JsonPropertyName("passenger_multi_adjustable_power_seat")]
        public string? PassengerMultiAdjustablePowerSeat { get; set; }

        [JsonPropertyName("pickup_truck_bed_liner")]
        public string? PickupTruckBedLiner { get; set; }

        [JsonPropertyName("pickup_truck_cargo_box_light")]
        public string? PickupTruckCargoBoxLight { get; set; }

        [JsonPropertyName("power_adjustable_exterior_mirror")]
        public string? PowerAdjustableExteriorMirror { get; set; }

        [JsonPropertyName("power_door_locks")]
        public string? PowerDoorLocks { get; set; }

        [JsonPropertyName("power_sliding_side_van_door")]
        public string? PowerSlidingSideVanDoor { get; set; }

        [JsonPropertyName("power_sunroof")]
        public string? PowerSunroof { get; set; }

        [JsonPropertyName("power_trunk_lid")]
        public string? PowerTrunkLid { get; set; }

        [JsonPropertyName("power_windows")]
        public string? PowerWindows { get; set; }

        [JsonPropertyName("rain_sensing_wipers")]
        public string? RainSensingWipers { get; set; }

        [JsonPropertyName("rear_spoiler")]
        public string? RearSpoiler { get; set; }

        [JsonPropertyName("rear_window_defogger")]
        public string? RearWindowDefogger { get; set; }

        [JsonPropertyName("rear_wiper")]
        public string? RearWiper { get; set; }

        [JsonPropertyName("remote_ignition")]
        public string? RemoteIgnition { get; set; }

        [JsonPropertyName("removable_top")]
        public string? RemovableTop { get; set; }

        [JsonPropertyName("run_flat_tires")]
        public string? RunFlatTires { get; set; }

        [JsonPropertyName("running_boards")]
        public string? RunningBoards { get; set; }

        [JsonPropertyName("second_row_folding_seat")]
        public string? SecondRowFoldingSeat { get; set; }

        [JsonPropertyName("second_row_heated_seat")]
        public string? SecondRowHeatedSeat { get; set; }

        [JsonPropertyName("second_row_multi_adjustable_power_seat")]
        public string? SecondRowMultiAdjustablePowerSeat { get; set; }

        [JsonPropertyName("second_row_removable_seat")]
        public string? SecondRowRemovableSeat { get; set; }

        [JsonPropertyName("second_row_side_airbag")]
        public string? SecondRowSideAirbag { get; set; }

        [JsonPropertyName("second_row_side_airbag_with_head_protection")]
        public string? SecondRowSideAirbagWithHeadProtection { get; set; }

        [JsonPropertyName("second_row_sound_controls")]
        public string? SecondRowSoundControls { get; set; }

        [JsonPropertyName("separate_driver_front_passenger_climate_controls")]
        public string? SeparateDriverFrontPassengerClimateControls { get; set; }

        [JsonPropertyName("side_head_curtain_airbag")]
        public string? SideHeadCurtainAirbag { get; set; }

        [JsonPropertyName("skid_plate")]
        public string? SkidPlate { get; set; }

        [JsonPropertyName("sliding_rear_pickup_truck_window")]
        public string? SlidingRearPickupTruckWindow { get; set; }

        [JsonPropertyName("splash_guards")]
        public string? SplashGuards { get; set; }

        [JsonPropertyName("steel_wheels")]
        public string? SteelWheels { get; set; }

        [JsonPropertyName("steering_wheel_mounted_controls")]
        public string? SteeringWheelMountedControls { get; set; }

        [JsonPropertyName("subwoofer")]
        public string? Subwoofer { get; set; }

        [JsonPropertyName("tachometer")]
        public string? Tachometer { get; set; }

        [JsonPropertyName("telematics_system")]
        public string? TelematicsSystem { get; set; }

        [JsonPropertyName("telescopic_steering_column")]
        public string? TelescopicSteeringColumn { get; set; }

        [JsonPropertyName("third_row_removable_seat")]
        public string? ThirdRowRemovableSeat { get; set; }

        [JsonPropertyName("tilt_steering")]
        public string? TiltSteering { get; set; }

        [JsonPropertyName("tilt_steering_column")]
        public string? TiltSteeringColumn { get; set; }

        [JsonPropertyName("tire_pressure_monitor")]
        public string? TirePressureMonitor { get; set; }

        [JsonPropertyName("tow_hitch_receiver")]
        public string? TowHitchReceiver { get; set; }

        [JsonPropertyName("towing_preparation_package")]
        public string? TowingPreparationPackage { get; set; }

        [JsonPropertyName("traction_control")]
        public string? TractionControl { get; set; }

        [JsonPropertyName("trip_computer")]
        public string? TripComputer { get; set; }

        [JsonPropertyName("trunk_anti_trap_device")]
        public string? TrunkAntiTrapDevice { get; set; }

        [JsonPropertyName("vehicle_anti_theft")]
        public string? VehicleAntiTheft { get; set; }

        [JsonPropertyName("vehicle_stability_control_system")]
        public string? VehicleStabilityControlSystem { get; set; }

        [JsonPropertyName("voice_activated_telephone")]
        public string? VoiceActivatedTelephone { get; set; }

        [JsonPropertyName("wind_deflector_for_convertibles")]
        public string? WindDeflectorForConvertibles { get; set; }

        // Catches new fields added by the API in the future
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraEquipment { get; set; }
    }
}


