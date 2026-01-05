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
        if (await _carModelRepository.AnyAsync())
        {
            _logger.LogInformation("CarModel data already exists. Skipping.");
            return;
        }

        var rootUrl = _configuration["OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl"];
        if (string.IsNullOrWhiteSpace(rootUrl))
            throw new Exception("Missing configuration: OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl");

        var carModelsContentPath = Path.Combine(rootUrl, "images", "ModelCategories", "CarModels");

        var seeds = new List<(string CategoryName, string Name, string FileName)>
        {
            // FORD 150
            ("FORD 1500", "Ford F-1500", "Ford F-150.jpg"),
            ("FORD 1500", "Ford F-150 Lightning", "Ford F-150 Lightning.jpg"),
            ("FORD 1500", "F-150 Lightning Pro EXT", "F-150 Lightning Pro EXT.jpg"),
            ("FORD 1500", "F-150 Lightning Pro STD", "F-150 Lightning Pro STD.jpg"),
            ("FORD 1500", "F-150LightningLariatExt", "F-150LightningLariatExt.avif"),

            // FORD SUPER DUTY
            ("FORD SUPER DUTY", "Ford Super Duty", "Ford Super Duty.jpg"),

            // RAM 150
            ("RAM 1500", "DT RAM", "DT RAM.jpg"),
            ("RAM 1500", "Ram 1500 DEPRECATED", "Ram 1500 DEPRECATED.jpg"),

            // RAM HEAVY DUTY
            ("RAM HEAVY DUTY", "HD RAM", "HD RAM.jpg"),
            ("RAM HEAVY DUTY", "Ram 2500 DEPRECATED", "Ram 2500 DEPRECATED.jpg"),
            ("RAM HEAVY DUTY", "Ram 3500 DEPRECATED", "Ram 3500 DEPRECATED.jpg"),
        };

        var categories = await _modelCategoryRepository.GetListAsync();
        var categoryByName = categories
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToDictionary(x => Normalize(x.Name), x => x);

        foreach (var (categoryName, name, fileName) in seeds)
        {
            var normalizedCategoryName = Normalize(categoryName);

            if (!categoryByName.TryGetValue(normalizedCategoryName, out var category))
            {
                // If you DON'T want auto-create, replace this block with "throw new BusinessException(...)".
                var categoryFilePath = Path.Combine(rootUrl, "images", "ModelCategories", "CarModels", $"{categoryName}.png");

                var catAttachment = new FileAttachment(
                    name: Path.GetFileName(categoryFilePath),
                    blobName: categoryFilePath,
                    path: categoryFilePath
                );

                category = new ModelCategory(
                    _guidGenerator.Create(),
                    categoryName.Trim(),
                    catAttachment
                );

                category = await _modelCategoryRepository.InsertAsync(category, autoSave: true);
                categoryByName[normalizedCategoryName] = category;

                _logger.LogInformation("Created missing ModelCategory: {CategoryName}", categoryName);
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
        }

        _logger.LogInformation("Added {Count} car model records", seeds.Count);
    }

    private static string Normalize(string value)
        => (value ?? string.Empty).Trim().ToUpperInvariant();
}
