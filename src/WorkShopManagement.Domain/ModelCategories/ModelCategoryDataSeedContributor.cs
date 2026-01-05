using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        if (await _modelCategoryRepository.AnyAsync())
        {
            _logger.LogInformation("Model data is already added. Skipping.");
            return;
        }

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

        _logger.LogInformation("Started.");

        foreach (var (name, fileName) in seeds)
        {
            var filePath = Path.Combine(contentPath, fileName);

            var attachment = new FileAttachment(
                name: fileName,
                blobName: filePath,
                path: filePath
            );

            var model = new ModelCategory(
                _guidGenerator.Create(),
                name,
                attachment
            );

            await _modelCategoryRepository.InsertAsync(model, autoSave: true);

        }

        _logger.LogInformation("Added {0} model records", seeds.Count);
    }
}
