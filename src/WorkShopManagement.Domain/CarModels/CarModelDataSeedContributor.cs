using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.ModelCategories;

namespace WorkShopManagement.CarModels;

public class CarModelDataSeedContributor : ITransientDependency
{
    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly IRepository<ModelCategory, Guid> _modelCategoryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CarModelDataSeedContributor> _logger;

    public CarModelDataSeedContributor(
        IRepository<CarModel, Guid> carModelRepository,
        IRepository<ModelCategory, Guid> modelCategoryRepository,
        ILogger<CarModelDataSeedContributor> logger,
        IGuidGenerator guidGenerator,
        IConfiguration configuration)
    {
        _carModelRepository = carModelRepository;
        _modelCategoryRepository = modelCategoryRepository;
        _guidGenerator = guidGenerator;
        _configuration = configuration;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var rootUrl = _configuration["OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl"];
        if (string.IsNullOrWhiteSpace(rootUrl))
            throw new Exception("Missing configuration: OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl");

        var carModelsContentPath = Path.Combine(rootUrl, "images", "ModelCategories", "CarModels");

        var seeds = new List<(string CategoryName, string Name, string FileName)>
        {
            // FORD 150
            ("FORD 150", "Raptor", "ford-raptor.jpg"),
            ("FORD 150", "Raptor R", "ford-raptor-r.jpg"),

            // FORD SUPER DUTY
            ("FORD SUPER DUTY", "Ford Superduty Pickup", "ford-super-duty-pickup.jpg"),
            ("FORD SUPER DUTY", "Ford Super Duty Cab Chassis", "ford-super-duty-cab-chassis.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® XL", "super-duty-f-250-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® XL", "super-duty-f-250-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® XL", "super-duty-f-450-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® XLT", "super-duty-f-250-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® XLT", "super-duty-f-250-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® XLT", "super-duty-f-450-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® LARIAT", "super-duty-f-250-lariat.jpg"),
            ("FORD SUPER DUTY", "2025 Super Duty® F-350® LARIAT", "super-duty-f-250-lariat.jpg"),
            ("FORD SUPER DUTY", "2025 Super Duty® F-450® LARIAT", "super-duty-f-450-lariat.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® King Ranch", "super-duty-f-250-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® King Ranch", "super-duty-f-250-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® King Ranch", "super-duty-f-450-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® Platinum", "super-duty-f-250-platinum.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® Platinum", "super-duty-f-250-platinum.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® Platinum", "super-duty-f-450-king-ranch.jpg"),

            ("FORD SUPER DUTY", "Chassis Cab F-350® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-350® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-350® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-600® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-600® XLT", "chassis-cab-f-350-xl.jpg"),


            // RAM 150
            ("RAM 1500", "RAM 1500 RHO", "ram-1500-rho.jpg"),
            ("RAM 1500", "RAM 1500 Tungsten", "ram-1500-tungsten.jpg"),

            // RAM HEAVY DUTY
            ("RAM HEAVY DUTY", "RAM Heavy Duty Pickup", "ram-heavy-duty-pickup.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 REBEL", "ram-heavy-duty-2500-rebel.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 LIMITED LONGHORN", "ram-heavy-duty-2500-limited-longhorn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 LIMITED", "ram-heavy-duty-2500-limited.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LIMITED", "ram-heavy-duty-3500-limited.png"),
            ("RAM HEAVY DUTY", "RAM heavy Duty 3500 LARAMIE", "ram-heavy-duty-3500-laramie.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 TRADESMAN", "ram-heavy-duty-35000tradesman.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LARAMIE", "ram-heavy-duty-3500-laramie.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LIMITED LONGHORN", "ram-heavy-duty-3500-limited-longhorn.png"),

            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 Cab Chassis TRADESMAN", "ram-heavy-duty-3500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 Cab Chassis BIG HORN", "ram-heavy-duty-3500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 4500 Cab Chassis TRADESMAN", "ram-heavy-duty-4500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 4500 Cab Chassis BIG HORN", "ram-heavy-duty-4500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 5500 Cab Chassis TRADESMAN", "ram-heavy-duty-5500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 5500 Cab Chassis BIG HORN", "ram-heavy-duty-5500-cab-chassis-tradesman.png")
        };

        var categories = await _modelCategoryRepository.GetListAsync();
        var categoryByName = categories
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToDictionary(x => Normalize(x.Name), x => x);

        // Incremental: track existing car models by (CategoryId + ModelName)
        var existingCarModelKeys = (await _carModelRepository.GetListAsync())
            .Select(x => $"{x.ModelCategoryId:N}|{Normalize(x.Name)}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var inserted = 0;

        foreach (var (categoryName, name, fileName) in seeds)
        {
            var normalizedCategoryName = Normalize(categoryName);

            if (!categoryByName.TryGetValue(normalizedCategoryName, out var category))
            {
                _logger.LogWarning("Missing ModelCategory '{CategoryName}'. Skipping car model '{CarModelName}'.", categoryName, name);
                continue;
            }

            var carModelKey = $"{category.Id:N}|{Normalize(name)}";
            if (existingCarModelKeys.Contains(carModelKey))
            {
                continue;
            }

            var filePath = Path.Combine(carModelsContentPath, fileName);

                var attachment = new FileAttachment(
                    name: fileName,
                    blobName: fileName,
                    path: filePath
                );

            var carModel = new CarModel(
                _guidGenerator.Create(),
                category.Id,
                name,
                attachment
            );

            await _carModelRepository.InsertAsync(carModel, autoSave: true);
            existingCarModelKeys.Add(carModelKey);
            inserted++;
        }

        _logger.LogInformation("Added {Count} car model records", inserted);
    }

    private static string Normalize(string value)
        => (value ?? string.Empty).Trim().ToUpperInvariant();
}
