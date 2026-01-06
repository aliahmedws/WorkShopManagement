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

namespace WorkShopManagement.ModelCategories;

public class ModelCategoryDataSeedContributor : ITransientDependency
{
    private readonly IRepository<ModelCategory, Guid> _modelCategoryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ModelCategoryDataSeedContributor> _logger;


    public ModelCategoryDataSeedContributor(
        IRepository<ModelCategory, Guid> modelCategoryRepository,
        ILogger<ModelCategoryDataSeedContributor> logger,
        IGuidGenerator guidGenerator,
        IConfiguration configuration)
    {
        _modelCategoryRepository = modelCategoryRepository;
        _guidGenerator = guidGenerator;
        _configuration = configuration;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var configuredDir = _configuration["OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl"];
        if (string.IsNullOrWhiteSpace(configuredDir))
        {
            throw new Exception("Missing configuration: Seed:CarModelsDir");
        }

        var contentPath = Path.Combine(configuredDir, "images", "ModelCategories");

        _logger.LogInformation("Seeding car model data");
        var seeds = new List<(string Name, string FileName)>
            {
                ("FORD 1500", "Ford-1500.png"),
                ("FORD SUPER DUTY", "Ford-Super-duty.png"),
                ("RAM 1500", "RAM-1500.png"),
                ("RAM HEAVY DUTY", "Ram-heavy-duty.png"),

            };

        var existingName = (await _modelCategoryRepository.GetListAsync())
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => x.Name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var inserted = 0;

        _logger.LogInformation("Started.");

        foreach (var (name, fileName) in seeds)
        {
            var normalizedName = name.Trim();
            if (existingName.Contains(normalizedName))
                continue;

            var filePath = Path.Combine(contentPath, fileName);

            var attachment = new FileAttachment(
                name: fileName,
                blobName: filePath,
                path: filePath
            );

            var model = new ModelCategory(
                _guidGenerator.Create(),
                normalizedName,
                attachment
            );

            await _modelCategoryRepository.InsertAsync(model);
            existingName.Add(normalizedName);
            inserted++;
        }

        _logger.LogInformation("Added {Count} new model records", inserted);
    }
}
