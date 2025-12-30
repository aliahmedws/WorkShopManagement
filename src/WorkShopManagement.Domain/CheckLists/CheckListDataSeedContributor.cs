using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using WorkShopManagement.CarModels;

namespace WorkShopManagement.CheckLists;

public class CheckListDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IRepository<CarModel, Guid> _carModelRepository;

    public CheckListDataSeedContributor(
        IRepository<CheckList, Guid> checkListRepository,
        IRepository<CarModel, Guid> carModelRepository)
    {
        _checkListRepository = checkListRepository;
        _carModelRepository = carModelRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var templates = GetModelTemplates();

        var carModels = await _carModelRepository.GetListAsync();
        var modelByName = carModels
            .Where(m => !string.IsNullOrWhiteSpace(m.Name))
            .ToDictionary(m => Normalize(m.Name), m => m);

        foreach (var template in templates)
        {
            if (!modelByName.TryGetValue(Normalize(template.ModelName), out var model))
            {
                throw new Exception($"CarModel not found for checklist seeding: '{template.ModelName}'.");
            }

            foreach (var set in template.Sets)
            {
                if (set.Steps == null || set.Steps.Count == 0)
                {
                    throw new Exception(
                        $"Checklist steps are empty. Model='{template.ModelName}', Type='{set.CheckListType}'.");
                }

                await SeedForModelAndTypeIfMissingAsync(model.Id, set.CheckListType, set.Steps);
            }
        }
    }

    private static string Normalize(string value)
        => value.Trim().ToUpperInvariant();

    private async Task SeedForModelAndTypeIfMissingAsync(
        Guid modelId,
        CheckListType checkListType,
        List<(int Position, string Name)> steps)
    {
        var exists = await _checkListRepository.AnyAsync(x =>
            x.CarModelId == modelId &&
            x.CheckListType == checkListType);

        if (exists)
        {
            return;
        }

        foreach (var (position, name) in steps.OrderBy(x => x.Position))
        {
            var entity = new CheckList(
                id: Guid.NewGuid(),
                name: name,
                position: position,
                carModelId: modelId,
                checkListType: checkListType
            );

            await _checkListRepository.InsertAsync(entity, autoSave: true);
        }
    }


    private static List<ModelTemplate> GetModelTemplates()
        => new()
        {
            new ModelTemplate(
                ModelName: "Ford F-150 Lightning",
                Sets: new List<CheckListTypeSet>
                {
                    new(CheckListType.Production,    GetFordLightning_Production()),
                    new(CheckListType.SubProduction, GetFordLightning_SubProduction()),
                    new(CheckListType.AncillaryTask, GetFordLightning_Ancillary()),
                }
            ),

            new ModelTemplate(
                ModelName: "F-150 Lightning Pro EXT",
                Sets: new List<CheckListTypeSet>
                {
                    new(CheckListType.Production,    GetLightningProExt_Production()),
                    new(CheckListType.SubProduction, GetLightningProExt_SubProduction()),
                    new(CheckListType.AncillaryTask, GetLightningProExt_Ancillary()),
                }
            ),
        };

    // Ford F-150 Lightning (CURRENT)

    private static List<(int Position, string Name)> GetFordLightning_Production()
        => new()
        {
            (1,  "Station 0 - Receiving Compliance Audit"),
            (2,  "Station 1A"),
            (3,  "Station 1B"),
            (4,  "Station 2"),
            (5,  "Station 3A"),
            (6,  "Station 3B"),
            (7,  "Station 4"),
            (8,  "Station 5 (QC)"),
            (9,  "Wheel Alignment"),
            (10, "Quality Control"),
            (11, "Quality Release"),
        };

    private static List<(int Position, string Name)> GetFordLightning_SubProduction()
        => new()
        {
            (1, "Dash Remanufacture"),
            (2, "HVAC"),
            (3, "Centre Console"),
            (4, "Seats Conversion"),
            (5, "Leather Seat Kit"),
            (6, "Sub Assembly Electrical"),
        };

    private static List<(int Position, string Name)> GetFordLightning_Ancillary()
        => new()
        {
            (1, "Invoice"),
            (2, "Procurement"),
            (3, "AVV Package"),
            (4, "Pre-Delivery Inspection"),
            (5, "Quality"),
        };

    // F-150 Lightning Pro EXT (TEMP = same as Ford Lightning)
    private static List<(int Position, string Name)> GetLightningProExt_Production()
        => GetFordLightning_Production();

    private static List<(int Position, string Name)> GetLightningProExt_SubProduction()
        => GetFordLightning_SubProduction();

    private static List<(int Position, string Name)> GetLightningProExt_Ancillary()
        => GetFordLightning_Ancillary();

    // Helper records
    private sealed record ModelTemplate(string ModelName, List<CheckListTypeSet> Sets);

    private sealed record CheckListTypeSet(CheckListType CheckListType, List<(int Position, string Name)> Steps);
}
