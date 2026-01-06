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
using WorkShopManagement.CheckLists;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.RadioOptions;

public class RadioOptionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string Station0Name = "Station 0 - Receiving Compliance Audit";
    private const string Station1AName = "Station 1A";

    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IRepository<ListItem, Guid> _listItemRepository;
    private readonly IRepository<RadioOption, Guid> _radioOptionRepository;
    private readonly ILogger<RadioOptionDataSeedContributor> _logger;
    private readonly IGuidGenerator _guidGenerator;

    public RadioOptionDataSeedContributor(
        IRepository<CarModel, Guid> carModelRepository,
        IRepository<CheckList, Guid> checkListRepository,
        IRepository<ListItem, Guid> listItemRepository,
        IRepository<RadioOption, Guid> radioOptionRepository,
        ILogger<RadioOptionDataSeedContributor> logger,
        IGuidGenerator guidGenerator)
    {
        _carModelRepository = carModelRepository;
        _checkListRepository = checkListRepository;
        _listItemRepository = listItemRepository;
        _radioOptionRepository = radioOptionRepository;
        _logger = logger;
        _guidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var carModels = await _carModelRepository.GetListAsync();
        if (carModels == null || carModels.Count == 0)
        {
            _logger.LogInformation("No CarModels found. Skipping RadioOption seeding.");
            return;
        }

        var totalInserted = 0;

        foreach (var carModel in carModels)
        {
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station0Name, GetStation0_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station1AName, GetStation1A_RadioSeeds());
        }

        _logger.LogInformation("Done. Inserted RadioOptions: {Count}.", totalInserted);
    }

    private async Task<int> SeedForCheckListAsync(Guid carModelId, string checkListName, IReadOnlyList<RadioSeed> seeds)
    {
        var checkList = await _checkListRepository.FirstOrDefaultAsync(x =>
            x.CarModelId == carModelId && x.Name == checkListName);

        if (checkList == null)
        {
            _logger.LogWarning("Checklist not found. CarModelId={CarModelId}, CheckListName={CheckListName}", carModelId, checkListName);
            return 0;
        }

        var listItems = await _listItemRepository.GetListAsync(x => x.CheckListId == checkList.Id);
        if (listItems.Count == 0)
        {
            _logger.LogWarning("No ListItems found. Skipping RadioOption seeding. CheckListId={CheckListId}, Name={CheckListName}", checkList.Id, checkListName);
            return 0;
        }

        // IMPORTANT: Handle duplicate positions safely.
        var duplicates = listItems
            .GroupBy(x => x.Position)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .OrderBy(x => x)
            .ToList();

        if (duplicates.Count > 0)
        {
            _logger.LogWarning(
                "Duplicate ListItem positions found. CheckListId={CheckListId}, Positions={Positions}. Using first ListItem per position.",
                checkList.Id,
                string.Join(", ", duplicates)
            );
        }

        var itemByPos = listItems
            .GroupBy(x => x.Position)
            .ToDictionary(g => g.Key, g => g.First());

        var inserted = 0;

        foreach (var seed in seeds)
        {
            if (!itemByPos.TryGetValue(seed.ListItemPosition, out var listItem))
            {
                _logger.LogWarning(
                    "ListItem not found for RadioOptions. CarModelId={CarModelId}, CheckList={CheckListName}, Position={Position}",
                    carModelId, checkListName, seed.ListItemPosition);
                continue;
            }

            var existing = await _radioOptionRepository.GetListAsync(x => x.ListItemId == listItem.Id);
            var existingNames = existing
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => x.Name.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var opt in seed.Options.Select(o => (o ?? string.Empty).Trim()).Where(o => !string.IsNullOrWhiteSpace(o)))
            {
                if (existingNames.Contains(opt))
                {
                    continue;
                }

                await _radioOptionRepository.InsertAsync(
                    new RadioOption(_guidGenerator.Create(), listItem.Id, opt),
                    autoSave: false
                );

                existingNames.Add(opt);
                inserted++;
            }
        }

        return inserted;
    }

    private static IReadOnlyList<RadioSeed> GetStation0_RadioSeeds()
        => new List<RadioSeed>
        {
            new(2,  "Yes", "No", "N/A"),
            new(3,  "Yes", "No", "N/A"),
            new(5,  "Yes", "No", "N/A"),
            new(6,  "Yes", "No", "N/A"),

            new(8,  "2", "1", "0"),
            new(9,  "No Lock Nuts", "Missing Key", "Lock Nuts with Key"),

            new(10, "Present", "Missing"),
            new(11, "Present", "Missing"),
            new(12, "Present", "Missing"),
            new(13, "Present", "Missing"),

            new(15, "Yes", "No", "N/A"),
            new(16, "Yes", "No", "N/A"),
            new(17, "Yes", "No", "N/A"),

            new(19, "Yes", "No", "N/A"),
            new(20, "Yes", "No", "N/A"),
            new(21, "Yes", "No", "N/A"),

            new(23, "Yes", "No", "N/A"),
            new(24, "Yes", "No", "N/A"),

            new(26, "SAE HL A I5 P P2 22TK", "Other (Write In Comments)"),
            new(27, "SAE HL A I5 P P2 22TK", "Other (Write In Comments)"),

            new(28, "SAE AIP2RST 22TK", "Other (Write In Comments)"),
            new(29, "SAE AIP2RST 22TK", "Other (Write In Comments)"),

            new(30, "SAE U3 (2)G 17TK", "SAE U3 (2)G 15TK", "Other (Write In Comments)"),

            new(31, "SAE E220", "N/A", "Other (Write In Comments)"),
            new(32, "SAE E220", "N/A", "Other (Write In Comments)"),

            new(34, "DOT-22 M50L1 AS1", "Other (Write In Comments)"),

            new(35, "E11 43R-000257", "Other (Write In Comments)"),
            new(36, "E11 43R-000257", "Other (Write In Comments)"),

            new(37, "DOT-467 M40T3 AS3", "Other (Write In Comments)"),
            new(38, "DOT-467 M40T3 AS3", "Other (Write In Comments)"),

            new(39, "E11 43R-000147", "Other (Write In Comments)"),

            new(40, "N/A", "E2 43R 0115131", "Other (Write In Comments)"),
            new(41, "N/A", "E2 43R 0115131", "Other (Write In Comments)"),

            new(42, "E11 026533", "Other (Write In Comments)"),

            new(44, "Michelin", "General", "Hankook", "Goodyear", "Other (Write In Comments)"),

            new(46, "No", "Yes"),
            new(47, "Pass", "Fail", "N/A"),
            new(48, "Pass", "Fail", "N/A"),
            new(49, "Complete"),
            new(50, "No", "Yes"),
            new(51, "Yes", "No", "N/A"),
        };

    private static IReadOnlyList<RadioSeed> GetStation1A_RadioSeeds()
        => new List<RadioSeed>
        {
            new(1,  "Yes", "No", "N/A"),
            new(2,  "Yes", "No", "N/A"),
            new(3,  "Yes", "No", "N/A"),
            new(4,  "Yes", "No", "N/A"),
            new(5,  "Yes", "No", "N/A"),
            new(6,  "Yes", "No", "N/A"),
            new(7,  "Yes", "No", "N/A"),
            new(8,  "Yes", "No", "N/A"),
            new(9,  "Yes", "No", "N/A"),
            new(10, "Yes", "No", "N/A"),
            new(11, "Yes", "No", "N/A"),
            new(12, "Yes", "No", "N/A"),
            new(13, "Yes", "No", "N/A"),
            new(14, "Yes", "No", "N/A"),
            new(15, "Yes", "No", "N/A"),
            new(16, "Yes", "No", "N/A"),
            new(17, "Yes", "No", "N/A"),
            new(18, "Yes", "No", "N/A"),
            new(19, "Yes", "No", "N/A"),
            new(20, "Yes", "No", "N/A"),
            new(21, "Yes", "No", "N/A"),

            new(23, "PASS", "FAIL"),
            new(24, "PASS", "FAIL"),
            new(25, "PASS", "FAIL"),
            new(26, "PASS", "FAIL"),
            new(27, "PASS", "FAIL"),
        };

    private sealed record RadioSeed(int ListItemPosition, params string[] Options);
}
