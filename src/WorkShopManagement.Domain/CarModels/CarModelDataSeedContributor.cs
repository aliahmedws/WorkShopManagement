using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModelDataSeedContributor : ITransientDependency
{
    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IConfiguration _configuration;

    public CarModelDataSeedContributor(
        IRepository<CarModel, Guid> carModelRepository,
        ICurrentTenant currentTenant,
        IGuidGenerator guidGenerator,
        IConfiguration configuration)
    {
        _carModelRepository = carModelRepository;
        _currentTenant = currentTenant;
        _guidGenerator = guidGenerator;
        _configuration = configuration;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            if (await _carModelRepository.AnyAsync())
            {
                return;
            }

            var configuredDir = _configuration["OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl"];
            if (string.IsNullOrWhiteSpace(configuredDir))
            {
                throw new Exception("Missing configuration: Seed:CarModelsDir");
            }

            var contentPath = Path.Combine(configuredDir, "images", "CarModels");

            var seeds = new List<(string Name, string FileName)>
            {
                ("Challenger Charger DEPRECATED", "Challenger Charger DEPRECATED.jpg"),
                ("Challenger Demon DEPRECATED", "Challenger Demon DEPRECATED.jpg"),
                ("Challenger Hellcat DEPRECATED", "Challenger Hellcat DEPRECATED.jpg"),
                ("Challenger RT DEPRECATED", "Challenger RT DEPRECATED.jpg"),
                ("Challenger/Charger", "Challenger-Charger.jpg"),
                ("Cruiser DEPRECATED", "Cruiser DEPRECATED.jpg"),
                ("DT RAM", "DT RAM.jpg"),
                ("F-150 Lightning Pro EXT (2)", "F-150 Lightning Pro EXT (2).jpg"),
                ("F-150 Lightning Pro EXT", "F-150 Lightning Pro EXT.jpg"),
                ("F-150 Lightning Pro STD", "F-150 Lightning Pro STD.jpg"),
                ("F-150LightningLariatExt", "F-150LightningLariatExt.avif"),
                ("Ford F-150 LEGACY Lightning", "Ford F-150 LEGACY Lightning.jpg"),
                ("Ford F-150 Lightning", "Ford F-150 Lightning.jpg"),
                ("Ford F-150", "Ford F-150.jpg"),
                ("Ford Super Duty", "Ford Super Duty.jpg"),
                ("HD RAM", "HD RAM.jpg"),
                ("Ram 1500 DEPRECATED", "Ram 1500 DEPRECATED.jpg"),
                ("Ram 2500 DEPRECATED", "Ram 2500 DEPRECATED.jpg"),
                ("Ram 3500 DEPRECATED", "Ram 3500 DEPRECATED.jpg"),
            };

            foreach (var (name, fileName) in seeds)
            {
                var filePath = Path.Combine(contentPath, fileName);

                var attachment = new FileAttachment(
                    name: fileName,
                    path: filePath
                );

                var carModel = new CarModel(
                    _guidGenerator.Create(),
                    name,
                    attachment
                );

                await _carModelRepository.InsertAsync(carModel, autoSave: true);
            }
        }
    }
}
