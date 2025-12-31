using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkShopManagement.External.Nhtsa;

/// <summary>
/// Defines a service for retrieving vehicle recall information from the National Highway Traffic Safety Administration
/// (NHTSA) database.
/// </summary>
public interface INhtsaService
{
    /// <summary>
    /// Asynchronously retrieves a list of vehicle recall records for the specified make, model, and model year.
    /// </summary>
    /// <param name="make">The manufacturer of the vehicle. Cannot be null or empty.</param>
    /// <param name="model">The model name of the vehicle. Cannot be null or empty.</param>
    /// <param name="modelYear">The model year of the vehicle. Must be a valid year in string format.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of recall records matching
    /// the specified vehicle. The list is empty if no recalls are found.</returns>
    Task<List<NhtsaRecallByVehicleResultDto>> GetRecallByVehicleAsync(string make, string model, string modelYear);
}
