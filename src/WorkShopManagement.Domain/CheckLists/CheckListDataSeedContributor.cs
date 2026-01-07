using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using WorkShopManagement.CarModels;

namespace WorkShopManagement.CheckLists;

public class CheckListDataSeedContributor : ITransientDependency
{
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly ILogger<CheckListDataSeedContributor> _logger;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUnitOfWorkManager _uowManager;

    public CheckListDataSeedContributor(
        IRepository<CheckList, Guid> checkListRepository,
        IRepository<CarModel, Guid> carModelRepository,
        ILogger<CheckListDataSeedContributor> logger,
        IGuidGenerator guidGenerator,
        IUnitOfWorkManager uowManager)
    {
        _checkListRepository = checkListRepository;
        _carModelRepository = carModelRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
        _uowManager = uowManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var seeds = GetDefaultCheckLists();

        var carModels = await _carModelRepository.GetListAsync();
        if (carModels == null || carModels.Count == 0)
        {
            _logger.LogInformation("No CarModels found. Skipping checklist seeding.");
            return;
        }

        _logger.LogInformation("Seeding default CheckLists for {Count} CarModels...", carModels.Count);

        var totalInserted = 0;

        foreach (var carModel in carModels)
        {
            var existing = await _checkListRepository.GetListAsync(x => x.CarModelId == carModel.Id);

            var existingNames = existing
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => x.Name.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var insertedForModel = 0;

            foreach (var s in seeds.OrderBy(x => x.Position))
            {
                if (existingNames.Contains(s.Name))
                {
                    continue;
                }

                var entity = new CheckList(
                    id: _guidGenerator.Create(),
                    name: s.Name,
                    position: s.Position,
                    carModelId: carModel.Id
                );

                await _checkListRepository.InsertAsync(entity, autoSave: false);

                insertedForModel++;
                totalInserted++;
            }

            if (insertedForModel > 0)
            {
                _logger.LogInformation(
                    "Inserted {Inserted} CheckLists for CarModel: {Name} ({Id})",
                    insertedForModel, carModel.Name, carModel.Id
                );
            }
        }

        await _uowManager.Current!.SaveChangesAsync();

        _logger.LogInformation("Done. Total inserted CheckLists: {TotalInserted}", totalInserted);
    }

    private static List<CheckListSeed> GetDefaultCheckLists()
        => new()
        {
            new(1,  "Station 0 - Receiving Compliance Audit", true),
            new(2,  "Station 1A", true),
            new(3,  "Station 1B", true),
            new(4,  "Station 2", true),
            new(5,  "Station 3A", true),
            new(6,  "Station 3B", true),
            new(7,  "Station 4", true),
            new(8,  "Station 5 (QC)", true),
            new(9,  "Wheel Alignment", true),
            new(10, "Quality Control", true),
            new(11, "Quality Release", true),
            new(12, "Dash Remanufacture", true),
            new(13, "HVAC", true),
            new(14, "Centre Console", true),
            new(15, "Seats Conversion", true),
            new(16, "Leather Seat Kit", true),
            new(17, "Sub Assembly Electrical", true),
            new(18, "Invoice", true),
            new(19, "Procurement", true),
            new(20, "AVV Package", true),
            new(21, "Pre-Delivery Inspection", true),
            new(22, "Quality", true)
        };

    private sealed record CheckListSeed(int Position, string Name, bool IsEnabled);
}
