
export interface CarsXeRecallItemDto {
  recall_date?: string;
  expiration_date?: string;
  nhtsa_id?: string;
  manufacturer_id?: string;
  recall_campaign_type?: string;
  recall_name?: string;
  component?: string;
  recall_description?: string;
  risk_description?: string;
  stop_sale?: boolean;
  dont_drive?: boolean;
  remedy_available?: boolean;
  recall_remedy?: string;
  parts_available?: boolean;
  labor_hours_min?: number;
  labor_hours_max?: number;
  recall_status?: string;
}

export interface CarsXeVinInputDto {
  vin?: string;
}

export interface RecallsDataDto {
  uuid?: string;
  vin?: string;
  manufacturer?: string;
  model_year?: string;
  make?: string;
  model?: string;
  has_recalls: boolean;
  recall_count: number;
  recalls: CarsXeRecallItemDto[];
}

export interface RecallsInputDto {
  key?: string;
  vin?: string;
}

export interface RecallsResponseDto {
  success: boolean;
  input: RecallsInputDto;
  data: RecallsDataDto;
  timestamp?: string;
}

export interface VinAttributesDto {
  vin?: string;
  vid?: string;
  make?: string;
  model?: string;
  year?: string;
  product_type?: string;
  body?: string;
  series?: string;
  fuel_type?: string;
  gears?: string;
  emission_standard?: string;
  manufacturer?: string;
  manufacturer_address?: string;
  plant_country?: string;
  engine_manufacturer?: string;
  avg_co2_emission_g_km?: string;
  no_of_axels?: string;
  no_of_doors?: string;
  no_of_seats?: string;
  rear_brakes?: string;
  steering_type?: string;
  rear_suspension?: string;
  front_suspension?: string;
  wheel_size?: string;
  wheel_size_array?: string;
  wheelbase_mm?: string;
  wheelbase_array_mm?: string;
  height_mm?: string;
  length_mm?: string;
  width_mm?: string;
  track_front_mm?: string;
  track_rear_mm?: string;
  max_speed_kmh?: string;
  max_trunk_capacity_liters?: string;
  min_trunk_capacity_liters?: string;
  weight_empty_kg?: string;
  max_weight_kg?: string;
  max_roof_load_kg?: string;
  permitted_trailer_load_without_brakes_kg?: string;
  abs?: string;
  check_digit?: string;
  sequential_number?: string;
  extra: Record<string, any>;
}

export interface VinResponseDto {
  success: boolean;
  input: CarsXeVinInputDto;
  attributes: VinAttributesDto;
  timestamp?: string;
}
