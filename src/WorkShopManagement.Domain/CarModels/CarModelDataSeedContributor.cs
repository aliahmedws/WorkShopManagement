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
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModelDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly FileManager _fileManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly IGuidGenerator _guidGenerator;

    public CarModelDataSeedContributor(
        IRepository<CarModel, Guid> carModelRepository,
        FileManager fileManager,
        ICurrentTenant currentTenant,
        IGuidGenerator guidGenerator)
    {
        _carModelRepository = carModelRepository;
        _fileManager = fileManager;
        _currentTenant = currentTenant;
        _guidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            //if (await _carModelRepository.AnyAsync())
            //{
            //    return;
            //}

            var seedDir = Path.Combine(AppContext.BaseDirectory, "seed", "car-models");

            var seeds = new List<(string Name, string FileName)>
            {
                ("F-150 Lightning Pro EXT",  "F-150 Lightning Pro EXT.jpg"),
                ("F-150 Lightning Pro STD", "F-150 Lightning Pro STD.jpg"),
            };

            foreach (var (name, fileName) in seeds)
            {
                var filePath = Path.Combine(seedDir, fileName);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Seed image not found: {filePath}");
                }

                await using var stream = File.OpenRead(filePath);

                var attachment = await _fileManager.SaveAsync(stream, fileName, "car-models");

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
