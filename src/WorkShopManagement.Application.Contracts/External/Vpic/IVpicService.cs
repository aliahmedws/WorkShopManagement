using System.Threading.Tasks;

namespace WorkShopManagement.External.Vpic;

/// <summary>
/// Provides methods for interacting with the NHTSA Vehicle API (vPIC) to decode VINs and retrieve vehicle information.
/// </summary>
/// <remarks>This service is registered as a transient dependency and is intended for use in applications that
/// require access to vehicle decoding and related data from the NHTSA vPIC API. For more information about the API, see
/// https://vpic.nhtsa.dot.gov/api/.</remarks>
public interface IVpicService
{
    /// <summary>
    /// Decodes a Vehicle Identification Number (VIN) and retrieves extended vehicle information, optionally using a
    /// specified model year.
    /// </summary>
    /// <param name="vin">The VIN to decode. Must be a non-empty string containing a valid 17-character VIN.</param>
    /// <param name="modelYear">The model year to use for decoding the VIN. Specify this value to improve decoding accuracy for vehicles where
    /// the VIN alone is ambiguous. If null, the model year is not used.</param>
    /// <returns>A <see cref="VpicVariableResultDto"/> containing the decoded vehicle information. The result includes extended
    /// details about the vehicle corresponding to the provided VIN.</returns>
    Task<VpicVariableResultDto> DecodeVinExtendedAsync(string vin, string? modelYear = null);
}
