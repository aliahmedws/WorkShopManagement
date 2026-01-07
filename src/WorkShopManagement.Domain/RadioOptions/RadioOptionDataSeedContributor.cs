using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

public class RadioOptionDataSeedContributor : ITransientDependency
{
    private const string Station0Name = "Station 0 - Receiving Compliance Audit";
    private const string Station1AName = "Station 1A";
    private const string Station1BName = "Station 1B";
    private const string Station3AName = "Station 3A";
    private const string QualityName = "Quality";
    private const string AVVPackageName = "AVV Package";
    private const string ProcurementName = "Procurement";
    private const string InvoiceName = "Invoice";
    private const string PreDeliveryInspectioneName = "Pre-Delivery Inspection";
    private const string SubAssemblyElectricalName = "Sub Assembly Electrical";
    private const string LeatherSeatKitName = "Leather Seat Kit";
    private const string SeatsConversionName = "Seats Conversion";
    private const string CentreConsoleName = "Centre Console";
    private const string HVACName = "HVAC";
    private const string DashRemanufactureName = "Dash Remanufacture";
    private const string QualityReleaseName = "Quality Release";
    private const string QualityControlName = "Quality Control";
    private const string WheelAlignmentName = "Wheel Alignment";
    private const string Station5QCName = "Station 5 (QC)";
    private const string Station4Name = "Station 4";
    private const string Station3BName = "Station 3B";
    private const string Station2Name = "Station 2";


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
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station1BName, GetStation1B_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station3AName, GetStation3A_RadioSeeds());


            totalInserted += await SeedForCheckListAsync(carModel.Id, Station2Name, GetStation2_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station3BName, GetStation3B_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station4Name, GetStation4_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, Station5QCName, GetStation5QC_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, WheelAlignmentName, GetWheelAlignment_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, QualityControlName, GetQualityControl_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, QualityReleaseName, GetQualityRelease_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, DashRemanufactureName, GetDashRemanufacture_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, HVACName, GetHVAC_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, CentreConsoleName, GetCentreConsole_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, SeatsConversionName, GetSeatsConversion_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, LeatherSeatKitName, GetLeatherSeatKit_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, SubAssemblyElectricalName, GetSubAssemblyElectrical_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, QualityName, GetQuality_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, AVVPackageName, GetAVVPackage_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, ProcurementName, GetProcurement_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, InvoiceName, GetInvoice_RadioSeeds());
            totalInserted += await SeedForCheckListAsync(carModel.Id, PreDeliveryInspectioneName, GetPreDeliveryInspection_RadioSeeds());
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
        // Yes / No / N/A
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
        new(22, "Yes", "No", "N/A"),
        new(23, "Yes", "No", "N/A"),
        new(24, "Yes", "No", "N/A"),
        new(25, "Yes", "No", "N/A"),
        new(26, "Yes", "No", "N/A"),
        new(27, "Yes", "No", "N/A"),
        new(28, "Yes", "No", "N/A"),
        new(29, "Yes", "No", "N/A"),
        new(30, "Yes", "No", "N/A"),
        new(31, "Yes", "No", "N/A"),
        new(32, "Yes", "No", "N/A"),
        new(33, "Yes", "No", "N/A"),
        new(34, "Yes", "No", "N/A"),
        new(36, "Yes", "No", "N/A"),
        new(43, "Yes", "No", "N/A"),

        // PASS / FAIL
        new(37, "PASS", "FAIL"),
        new(38, "PASS", "FAIL"),
        new(39, "PASS", "FAIL"),
        new(40, "PASS", "FAIL"),
        new(41, "PASS", "FAIL"),
        new(42, "PASS", "FAIL"),
     }; 
    private static IReadOnlyList<RadioSeed> GetStation1B_RadioSeeds()
     => new List<RadioSeed>
     {
        // 1–8 : Yes / No / N/A
        new(1,  "Yes", "No", "N/A"),
        new(2,  "Yes", "No", "N/A"),
        new(3,  "Yes", "No", "N/A"),
        new(4,  "Yes", "No", "N/A"),
        new(5,  "Yes", "No", "N/A"),
        new(6,  "Yes", "No", "N/A"),
        new(7,  "Yes", "No", "N/A"),
        new(8,  "Yes", "No", "N/A"),

        // 9 : QC Checklist (separator → no radios)

        // 10–11 : PASS / FAIL
        new(10, "PASS", "FAIL"),
        new(11, "PASS", "FAIL"),
     };

    private static IReadOnlyList<RadioSeed> GetStation3A_RadioSeeds()
    => new List<RadioSeed>
    {
        // Main checklist: Yes / No / N/A
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

        // QC checklist: PASS / FAIL
        new(15, "PASS", "FAIL"),
        new(16, "PASS", "FAIL"),
        new(17, "PASS", "FAIL"),
        new(18, "PASS", "FAIL"),
        new(19, "PASS", "FAIL"),
    };

    private static IReadOnlyList<RadioSeed> GetProcurement_RadioSeeds()
      => new List<RadioSeed>
      {
                new(1,  "AUSEV", "AUSMV", "Performance", "SCD-RV/External($0 invoice)"),
                new(2,  "Yes", "No", "N/A"),
      };

    private static IReadOnlyList<RadioSeed> GetQuality_RadioSeeds()
    => new List<RadioSeed>
    {
            new(1,  "Yes", "No", "N/A"),
    };

    private static IReadOnlyList<RadioSeed> GetPreDeliveryInspection_RadioSeeds()
    => new List<RadioSeed>
    {
        // 3..5 Vehicle History
        new(3,  "Pass", "Fail", "N/A"),
        new(4,  "Pass", "Fail", "N/A"),
        new(5,  "Pass", "Fail", "N/A"),

        // 7..19 Vehicle Exterior
        new(7,  "Pass", "Fail", "N/A"),
        new(8,  "Pass", "Fail", "N/A"),
        new(9,  "Pass", "Fail", "N/A"),
        new(10, "Pass", "Fail", "N/A"),
        new(11, "Pass", "Fail", "N/A"),
        new(12, "Pass", "Fail", "N/A"),
        new(13, "Pass", "Fail", "N/A"),
        new(14, "Pass", "Fail", "N/A"),
        new(15, "Pass", "Fail", "N/A"),
        new(16, "Pass", "Fail", "N/A"),
        new(17, "Pass", "Fail", "N/A"),
        new(18, "Pass", "Fail", "N/A"),
        new(19, "Pass", "Fail", "N/A"),

        // 21..24 Underhood
        new(21, "Pass", "Fail", "N/A"),
        new(22, "Pass", "Fail", "N/A"),
        new(23, "Pass", "Fail", "N/A"),
        new(24, "Pass", "Fail", "N/A"),

        // 26..39 Vehicle Interior
        new(26, "Pass", "Fail", "N/A"),
        new(27, "Pass", "Fail", "N/A"),
        new(28, "Pass", "Fail", "N/A"),
        new(29, "Pass", "Fail", "N/A"),
        new(30, "Pass", "Fail", "N/A"),
        new(31, "Pass", "Fail", "N/A"),
        new(32, "Pass", "Fail", "N/A"),
        new(33, "Pass", "Fail", "N/A"),
        new(34, "Pass", "Fail", "N/A"),
        new(35, "Pass", "Fail", "N/A"),
        new(36, "Pass", "Fail", "N/A"),
        new(37, "Pass", "Fail", "N/A"),
        new(38, "Pass", "Fail", "N/A"),
        new(39, "Pass", "Fail", "N/A"),

        // 41..49 Convenience
        new(41, "Present", "Missing", "N/A"),
        new(42, "Pass", "Fail", "N/A"),
        new(43, "Pass", "Fail", "N/A"),
        new(44, "Pass", "Fail", "N/A"),
        new(45, "Pass", "Fail", "N/A"),
        new(46, "Pass", "Fail", "N/A"),
        new(47, "Pass", "Fail", "N/A"),
        new(48, "Pass", "Fail", "N/A"),
        new(49, "Pass", "Fail", "N/A"),

        // 52..58 Vehicle Inspections
        new(52, "Pass", "Fail", "N/A"),
        new(53, "Pass", "Fail", "N/A"),
        new(54, "Pass", "Fail", "N/A"),
        new(55, "Pass", "Fail", "N/A"),
        new(56, "Pass", "Fail", "N/A"),
        new(57, "Pass", "Fail", "N/A"),
        new(58, "Pass", "Fail", "N/A"),

        // 60..63 TSBs
        new(60, "Pass", "Fail", "N/A"),
        new(61, "Pass", "Fail", "N/A"),
        new(62, "Pass", "Fail", "N/A"),
        new(63, "Pass", "Fail", "N/A"),

        // 65 PDI Assessment
        new(65, "Passed", "Failed"),

        // 69..73 Dispatchment Photos
        new(69, "Yes", "No", "N/A"),
        new(70, "Yes", "No", "N/A"),
        new(71, "Yes", "No", "N/A"),
        new(72, "Yes", "No", "N/A"),
        new(73, "Yes", "No", "N/A"),
    };

    private static IReadOnlyList<RadioSeed> GetInvoice_RadioSeeds()
    => new List<RadioSeed>
    {
            new(1,  "Yes", "No", "N/A"),
            new(2,  "Yes", "No", "N/A"),
            new(3,  "Yes", "No", "N/A"),
            new(4,  "Yes", "No", "N/A"),
    };
    private static IReadOnlyList<RadioSeed> GetAVVPackage_RadioSeeds()
    => new List<RadioSeed>
    {
            new(1,  "Pass", "Fail", "N/A"),
    };
    private static IReadOnlyList<RadioSeed> GetLeatherSeatKit_RadioSeeds()
       => new List<RadioSeed>
       {
                new(1,  "Yes", "No", "N/A"),
       };

    private static IReadOnlyList<RadioSeed> GetSubAssemblyElectrical_RadioSeeds()
    => new List<RadioSeed>
    {
            new(1,  "Yes", "No", "N/A"),
    };

    private static IReadOnlyList<RadioSeed> GetSeatsConversion_RadioSeeds()
       => new List<RadioSeed>
       {
                new(1,  "Yes", "No", "N/A"),
                new(2,  "Yes", "No", "N/A"),
                new(3,  "Yes", "No", "N/A"),
                new(4,  "Yes", "No", "N/A"),
                new(5,  "Yes", "No", "N/A"),
                new(6,  "Yes", "No", "N/A"),
       };

    private static IReadOnlyList<RadioSeed> GetCentreConsole_RadioSeeds()
      => new List<RadioSeed>
      {
                new(1,  "Yes", "No", "N/A"),
                new(2,  "Yes", "No", "N/A"),
                new(3,  "Yes", "No", "N/A"),
                new(4,  "Yes", "No", "N/A"),
                new(5,  "Yes", "No", "N/A"),
      };
    private static IReadOnlyList<RadioSeed> GetHVAC_RadioSeeds()
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
    };
    private static IReadOnlyList<RadioSeed> GetDashRemanufacture_RadioSeeds()
    => new List<RadioSeed>
    {
        // STAGE 1
        new(2,  "Yes", "No", "N/A"),
        new(3,  "Yes", "No", "N/A"),
        new(4,  "Yes", "No", "N/A"),
        new(5,  "Yes", "No", "N/A"),
        new(6,  "Yes", "No", "N/A"),
        new(7,  "Yes", "No", "N/A"),
        new(8,  "Yes", "No", "N/A"),

        // STAGE 2
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
        new(22, "Yes", "No", "N/A"),

        // TEAM LEADER ONLY
        new(24, "Yes", "No", "N/A"),
        new(25, "Yes", "No", "N/A"),
        new(26, "Yes", "No", "N/A"),
        new(27, "Yes", "No", "N/A"),
        new(28, "Yes", "No", "N/A"),
    };
    private static IReadOnlyList<RadioSeed> GetQualityRelease_RadioSeeds()
    => new List<RadioSeed>
    {
        new(1, "Yes", "No", "N/A"),
        // UI shows only Yes/No for item 2 (no N/A)
        new(2, "Yes", "No"),
        new(3, "Yes", "No", "N/A"),
        new(4, "Yes", "No", "N/A"),
        new(5, "Pass", "Fail"),
        new(6, "Pass", "Fail"),
    };

    private static IReadOnlyList<RadioSeed> GetQualityControl_RadioSeeds()
    => new List<RadioSeed>
    {
        // 1..40 => Pass/Fail
        new(1,  "Pass", "Fail"),
        new(2,  "Pass", "Fail"),
        new(3,  "Pass", "Fail"),
        new(4,  "Pass", "Fail"),
        new(5,  "Pass", "Fail"),
        new(6,  "Pass", "Fail"),
        new(7,  "Pass", "Fail"),
        new(8,  "Pass", "Fail"),
        new(9,  "Pass", "Fail"),
        new(10, "Pass", "Fail"),
        new(11, "Pass", "Fail"),
        new(12, "Pass", "Fail"),
        new(13, "Pass", "Fail"),
        new(14, "Pass", "Fail"),
        new(15, "Pass", "Fail"),
        new(16, "Pass", "Fail"),
        new(17, "Pass", "Fail"),
        new(18, "Pass", "Fail"),
        new(19, "Pass", "Fail"),
        new(20, "Pass", "Fail"),
        new(21, "Pass", "Fail"),
        new(22, "Pass", "Fail"),
        new(23, "Pass", "Fail"),
        new(24, "Pass", "Fail"),
        new(25, "Pass", "Fail"),
        new(26, "Pass", "Fail"),
        new(27, "Pass", "Fail"),
        new(28, "Pass", "Fail"),
        new(29, "Pass", "Fail"),
        new(30, "Pass", "Fail"),
        new(31, "Pass", "Fail"),
        new(32, "Pass", "Fail"),
        new(33, "Pass", "Fail"),
        new(34, "Pass", "Fail"),
        new(35, "Pass", "Fail"),
        new(36, "Pass", "Fail"),
        new(37, "Pass", "Fail"),
        new(38, "Pass", "Fail"),
        new(39, "Pass", "Fail"),
        new(40, "Pass", "Fail"),

        // 41 => Done/Not Done
        new(41, "Done", "Not Done"),

        // 43..53 => Yes/No/N/A
        new(43, "Yes", "No", "N/A"),
        new(44, "Yes", "No", "N/A"),
        new(45, "Yes", "No", "N/A"),
        new(46, "Yes", "No", "N/A"),
        new(47, "Yes", "No", "N/A"),
        new(48, "Yes", "No", "N/A"),
        new(49, "Yes", "No", "N/A"),
        new(50, "Yes", "No", "N/A"),
        new(51, "Yes", "No", "N/A"),
        new(52, "Yes", "No", "N/A"),
        new(53, "Yes", "No", "N/A"),
    };

    private static IReadOnlyList<RadioSeed> GetWheelAlignment_RadioSeeds()
   => new List<RadioSeed>
   {
                new(1,  "SCD", "Not Done", "Good Year"),
   };

    private static IReadOnlyList<RadioSeed> GetStation5QC_RadioSeeds()
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
    };

    private static IReadOnlyList<RadioSeed> GetStation4_RadioSeeds()
    => new List<RadioSeed>
    {
        // 1-21 => Yes/No/N/A
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

        // 23-32 => PASS/FAIL
        new(23, "PASS", "FAIL"),
        new(24, "PASS", "FAIL"),
        new(25, "PASS", "FAIL"),
        new(26, "PASS", "FAIL"),
        new(27, "PASS", "FAIL"),
        new(28, "PASS", "FAIL"),
        new(29, "PASS", "FAIL"),
        new(30, "PASS", "FAIL"),
        new(31, "PASS", "FAIL"),
        new(32, "PASS", "FAIL"),

        // 33 => Yes/No/N/A
        new(33, "Yes", "No", "N/A"),
    };

    private static IReadOnlyList<RadioSeed> GetStation3B_RadioSeeds()
    => new List<RadioSeed>
    {
        new(1, "Yes", "No", "N/A"),
        new(2, "Yes", "No", "N/A"),
        new(3, "Yes", "No", "N/A"),
        new(4, "Yes", "No", "N/A"),
        new(5, "Yes", "No", "N/A"),
        new(6, "Yes", "No", "N/A"),

        new(8, "PASS", "FAIL"),
        new(9, "PASS", "FAIL"),
    };

    private static IReadOnlyList<RadioSeed> GetStation2_RadioSeeds()
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

        new(18, "PASS", "FAIL"),
        new(19, "PASS", "FAIL"),
        new(20, "PASS", "FAIL"),
        new(21, "PASS", "FAIL"),
        new(22, "PASS", "FAIL"),
        new(23, "PASS", "FAIL"),
    };

    private sealed record RadioSeed(int ListItemPosition, params string[] Options);
}
