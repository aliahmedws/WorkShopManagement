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

namespace WorkShopManagement.ListItems;

public class ListItemDataSeedContributor : ITransientDependency
{
    private const string Station0Name = "Station 0 - Receiving Compliance Audit";
    private const string Station1AName = "Station 1A";
    private const string Station1BName = "Station 1B";
    private const string Station3AName = "Station 3A";

    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IRepository<ListItem, Guid> _listItemRepository;
    private readonly ILogger<ListItemDataSeedContributor> _logger;
    private readonly IGuidGenerator _guidGenerator;

    public ListItemDataSeedContributor(
        IRepository<CarModel, Guid> carModelRepository,
        IRepository<CheckList, Guid> checkListRepository,
        IRepository<ListItem, Guid> listItemRepository,
        ILogger<ListItemDataSeedContributor> logger,
        IGuidGenerator guidGenerator)
    {
        _carModelRepository = carModelRepository;
        _checkListRepository = checkListRepository;
        _listItemRepository = listItemRepository;
        _logger = logger;
        _guidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var carModels = await _carModelRepository.GetListAsync();
        if (carModels == null || carModels.Count == 0)
        {
            _logger.LogInformation("No CarModels found. Skipping ListItem seeding.");
            return;
        }

        var totalInsertedItems = 0;

        foreach (var carModel in carModels)
        {
            totalInsertedItems += await SeedCheckListItemsAsync(
                carModelId: carModel.Id,
                checkListName: Station0Name,
                listItemSeeds: GetStation0_ListItems()
            );

            totalInsertedItems += await SeedCheckListItemsAsync(
                carModelId: carModel.Id,
                checkListName: Station1AName,
                listItemSeeds: GetStation1A_ListItems()
            );

            totalInsertedItems += await SeedCheckListItemsAsync(
                carModelId: carModel.Id,
                checkListName: Station1BName,
                listItemSeeds: Station1B_ListItems()
                );

            totalInsertedItems += await SeedCheckListItemsAsync(
                carModelId: carModel.Id,
                checkListName: Station3AName,
                listItemSeeds: Station3A_ListItems()
                );
        }

        _logger.LogInformation("Done. Inserted ListItems: {Items}.", totalInsertedItems);
    }

    private async Task<int> SeedCheckListItemsAsync(
        Guid carModelId,
        string checkListName,
        List<ListItemSeed> listItemSeeds)
    {
        var checkList = await _checkListRepository.FirstOrDefaultAsync(x =>
            x.CarModelId == carModelId && x.Name == checkListName);

        if (checkList == null)
        {
            _logger.LogWarning("Checklist not found. CarModelId={CarModelId}, CheckListName={CheckListName}", carModelId, checkListName);
            return 0;
        }

        var existingItems = await _listItemRepository.GetListAsync(x => x.CheckListId == checkList.Id);
        var existingByPosition = existingItems
          .GroupBy(x => x.Position)
          .ToDictionary(g => g.Key, g => g.First());

        var existingNames = existingItems
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => Normalize(x.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var insertedItems = 0;

        foreach (var s in listItemSeeds.OrderBy(x => x.Position))
        {
            var seedName = Normalize(s.Name);

            if (existingByPosition.ContainsKey(s.Position) || existingNames.Contains(seedName))
            {
                continue;
            }

            var entity = new ListItem(
                id: _guidGenerator.Create(),
                checkListId: checkList.Id,
                position: s.Position,
                name: s.Name,
                commentPlaceholder: s.IsSeparator ? null : s.CommentPlaceholder,
                commentType: s.IsSeparator ? null : s.CommentType,
                isAttachmentRequired: s.IsSeparator ? false : s.IsAttachmentRequired,
                isSeparator: s.IsSeparator
            );

            await _listItemRepository.InsertAsync(entity, autoSave: false);
            existingByPosition[s.Position] = entity;
            existingNames.Add(seedName);
            insertedItems++;
        }

        return insertedItems;
    }

    // Station 0 Seeds
    private static List<ListItemSeed> GetStation0_ListItems()
        => new()
        {
            new(1,  "Odometer <= 200Km", "Odometer <= 200Km", null, false, true),
            new(2,  "Initial Wash & Damage Inspection", "Initial Wash & Damage Inspection", null, false, true),
            new(3,  "Perform initial wash", "Perform initial wash", CommentType.String, true, false),
            new(4,  "Perform initial damage inspection", "Perform initial damage inspection", null, false, true),

            new(5,  "Information Stickers Installed", "Information Stickers Installed", null, false, true),

            new(6,  "Key tag", "Key tag", CommentType.String, true, false),
            new(7,  "Customer Information Label Installed", "Customer Information Label Installed", null, false, true),

            new(8,  "Vehicle Inspection", "Vehicle Inspection", null, false, true),
            
            new(9,  "Key Fobs", "Key Fobs", CommentType.String, true, false),
            new(10, "Lock Nuts", "Lock Nuts", CommentType.String, true, false),
            new(11, "User's Guide", "User's Guide", CommentType.String, true, false),
            new(12, "Antenna", "Antenna", CommentType.String, true, false),
            new(13, "Secure Pin Card", "Secure Pin Card", CommentType.String, true, false),
            new(14, "Floor Mats", "Floor Mats", CommentType.String, true, false),

            new(15, "Vehicle Photos", "Vehicle Photos", null, false, true),
            new(16, "Dash Photo", "Dash Photo", CommentType.String, true, false),
            new(17, "Use WiTech to scan vehicle and upload photo of general summary report",
                "Use WiTech to scan vehicle and upload photo of general summary report", CommentType.String, true, false),
            new(18, "Use WiTech to scan vehicle and upload photo of recall summary report tab", "Use WiTech to scan vehicle and upload photo of recall summary report tab", null, false, true),
            
            new(19, "Sticker Installation", "Sticker Installation", null, false, true),
            
            new(20, "Tyre placard replaced - ADR 42/05", "Tyre placard replaced - ADR 42/05", CommentType.String, true, false),
            new(21, "ow Bar Label", "ow Bar Label", CommentType.String, true, false),
            new(22, "OEM build plate sighted – ADR 61/03", "OEM build plate sighted – ADR 61/03", CommentType.String, true, false),
            new(23, "Install (2) Sun Visor and (1) LHS Dash Airbag Labels", "Install (2) Sun Visor and (1) LHS Dash Airbag Labels", CommentType.String, true, false),
            new(24, "Customer Information Notice", "Photos Uploaded", CommentType.String, true, false),
            new(25, "Owner's Manual", "Owner's Manual", CommentType.String, true, false),
            new(26, "Charging Port Labels (3)", "Charging Port Labels (3)", CommentType.String, true, false),
            new(27, "Sales/Support Label", "Sales/Support Label", CommentType.String, true, false),

            new(28, "Photos & Review", "Photos & Review", null, false, true),
            
            new(29, "Vehicle Photos", "Vehicle Photos", CommentType.String, true, false),
            new(30, "Photos Uploaded", "Photos Uploaded", CommentType.String, true, false),

            new(31, "Lighting", "Lighting", null, false, true),

            new(32, "LHS Headlight", "LHS Headlight", CommentType.String, true, false),
            new(33, "RHS Headlight", "RHS Headlight", null, false, true),
            new(34, "LHS Taillight", "LHS Taillight", CommentType.String, true, false),
            new(35, "RHS Taillight", "RHS Taillight", CommentType.String, true, false),
            new(36, "CHMSL", "CHMSL", CommentType.String, true, false),
            new(37, "Left Mirror Indicator", "Left Mirror Indicator", CommentType.String, true, false),
            new(38, "Right Mirror Indicator", "Right Mirror Indicator", CommentType.String, true, false),

            new(39, "Glassing", "Glassing", null, false, true),

            new(40, "Windscreen", "Windscreen", CommentType.String, true, false),
            new(41, "LHS Front Window", "LHS Front Window", CommentType.String, true, false),
            new(42, "RHS Front Window", "RHS Front Window", CommentType.String, true, false),
            new(43, "LHS Rear Window", "LHS Rear Window", CommentType.String, true, false),
            new(44, "RHS Rear Window", "RHS Rear Window", CommentType.String, true, false),
            new(45, "Rear Window", "Rear Window", CommentType.String, true, false),
            new(46, "Sunroof Front", "Sunroof Front", CommentType.String, true, false),
            new(47, "Sunroof Rear", "Sunroof Rear", CommentType.String, true, false),
            new(48, "Interior Rear Mirror", "Interior Rear Mirror", CommentType.String, true, false),

            new(49, "Tyres", "Tyres", null, false, true),

            new(50, "Front & Rear Tyres", "Front & Rear Tyres", CommentType.String, true, false),
          
            new(51, "Damage and Corrosion Check", "Damage and Corrosion Check", CommentType.String, true, false),
            new(52, "Structural Damage Detected", "Structural Damage Detected", CommentType.String, true, false),
            new(53, "Levels of Damage", "Levels of Damage", CommentType.String, true, false),
            new(54, "Body Alignment Check", "Body Alignment Check", CommentType.String, true, false),
            new(55, "Deterioration check", "Deterioration check", CommentType.String, true, false),
            new(56, "Signs of odometer tampering", "Signs of odometer tampering", CommentType.String, true, false),
            new(57, "Vehicle ready for Production", "Vehicle ready for Production", CommentType.String, true, false)
        };

    // Station 1A Seeds
    private static List<ListItemSeed> GetStation1A_ListItems()
        => new()
        {
            new(1,  "Disassemble Frunk", "Disassemble Frunk",  CommentType.String, true, false),
            new(2,  "Disconnect the Battery", "Disconnect the Battery", CommentType.String, true, false),
            new(3,  "Degas A/C", "Degas A/C", CommentType.String, true, false),
            new(4,  "Disassemble RHS Door Card", "Disassemble RHS Door Card", CommentType.String, true, false),
            new(5,  "Disassemble RHS Door Loom", "Disassemble RHS Door Loom", CommentType.String, true, false),
            new(6,  "Disassemble RHS Seats", "Disassemble RHS Seats", CommentType.String, true, false),
            new(7,  "Disassemble RHS Trims", "Disassemble RHS Trims", CommentType.String, true, false),
            new(8,  "RHS B Pillar and Pretensioner removal", "RHS B Pillar and Pretensioner removal", CommentType.String, true, false),
            new(9,  "Disassemble LHS Door Card", "Disassemble LHS Door Card", CommentType.String, true, false),
            new(10, "Disassemble LHS Door Loom", "Disassemble LHS Door Loom", CommentType.String, true, false),
            new(11, "Disassemble LHS Seats", "Disassemble LHS Seats", CommentType.String, true, false),
            new(12, "Disassemble the LHS Door Seat Trims", "Disassemble the LHS Door Seat Trims", CommentType.String, true, false),
            new(13, "LHS B Pillar and Pretensioner Removal", "LHS B Pillar and Pretensioner Removal", CommentType.String, true, false),
            new(14, "DIsassemble the Centre Console", "DIsassemble the Centre Console", CommentType.String, true, false),
            new(15, "Disassemble Glovebox & Undertray", "Disassemble Glovebox & Undertray", CommentType.String, true, false),
            new(16, "Disassemble the Airbag & Steering Wheel", "Disassemble the Airbag & Steering Wheel", CommentType.String, true, false),
            new(17, "Disassemble the Steering Column", "Disassemble the Steering Column", CommentType.String, true, false),
            new(18, "Remove Floor Items and Carpet", "Remove Floor Items and Carpet", CommentType.String, true, false),
            new(19, "Disassemble the Dash", "Disassemble the Dash", CommentType.String, true, false),
            new(20, "Disassemble IPMA Module", "Disassemble IPMA Module", CommentType.String, true, false),
            new(21, "Remove Wheels", "Remove Wheels", CommentType.String, true, false),

            new(22, "QC Checklist", "QC Checklist", null, false, true),

            new(23, "No Damages", "No Damages", CommentType.String, true, false),
            new(24, "All fasteners removed and placed in correct location", "All fasteners removed and placed in correct location", CommentType.String, true, false),
            new(25, "Correct placement of green wrap", "Correct placement of green wrap", CommentType.String, true, false),
            new(26, "Clock spring has been removed zip tied", "Clock spring has been removed zip tied", CommentType.String, true, false),
            new(27, "The VIN Stickers have been checked against the VIN of the vehicle and bay number before fitment.",
                "The VIN Stickers have been checked against the VIN of the vehicle and bay number before fitment.",
                CommentType.String, true, false),
        };

    private static List<ListItemSeed> Station1B_ListItems()
      => new()
      {
            new(1,  "Remove Centre Firewall",
                "Remove Centre Firewall",
                CommentType.String, true, false),

            new(2,  "LHS Firewall - Cut TX Valve, heater hose access, and install LHS Firewall Plates", "LHS Firewall - Cut TX Valve, heater hose access, and install LHS Firewall Plates", CommentType.String, true, false),
            new(3,  "RHS Firewall - Install the RHS Firewall Plates and Accelerator Plate", "RHS Firewall - Install the RHS Firewall Plates and Accelerator Plate", CommentType.String, true, false),
            new(4,  "LHS A-Pillar - Using the template mark out and remove the required material.", "LHS A-Pillar - Using the template mark out and remove the required material.", CommentType.String, true, false),
            new(5,  "Steering Box Plates - Install the Steering Box Sandwhich Plates.", "Steering Box Plates - Install the Steering Box Sandwhich Plates.", CommentType.String, true, false),
            new(6,  "Centre Plate - Install the centre firewall bracing plate.", "Centre Plate - Install the centre firewall bracing plate.", CommentType.String, true, false),
            new(7,  "RHS Wiper Foot Cutout.", "RHS Wiper Foot Cutout.", CommentType.String, true, false),
            new(8,  "Firewall Insulation", "Firewall Insulation", CommentType.String, true, false),
            new(9,  "QC Checklist", "QC Checklist", null, false, true),
            new(10,  "No Damages, no cuts of lines or looms", "No Damages, no cuts of lines or looms", CommentType.String, true, false),
            new(11,  "All firewall plates are fitted", "All firewall plates are fitted", CommentType.String, true, false),
            new(12,  "Correct sealing", "Correct sealing", CommentType.String, true, false),
            new(13,  "Insulation is done and correct", "Insulation is done and correct", CommentType.String, true, false),
            new(14,  "No burrs, no left over materials", "No burrs, no left over materials", CommentType.String, true, false),


      };

    private static List<ListItemSeed> Station3A_ListItems()
     => new()
     {
            new(1,  "Install Door Wiring Harness","Install Door Wiring Harness",CommentType.String, true, false),

            new(2,  "Gaurd Indicators and Wiring installation - ADR 06/00", "Gaurd Indicators and Wiring installation - ADR 06/00", CommentType.String, true, false),
            new(3,  "Install Crossover Looms", "Install Crossover Looms", CommentType.String, true, false),
            new(4,  "Modify LHS Loom", "Modify LHS Loom", CommentType.String, true, false),
            new(5,  "Modify and Install rear vision Mirrors - ADR 14/02", "Modify and Install rear vision Mirrors - ADR 14/02", CommentType.String, true, false),
            new(6,  "Install Trailer Wiring", "Install Trailer Wiring", CommentType.String, true, false),
            new(7,  "QC Checklist", "QC Checklist", null, false, true),
            new(8,  "No Damages or cuts to looms", "No Damages or cuts to looms", CommentType.String, true, false),
            new(9,  "All door looms are connected and tested OK", "All door looms are connected and tested OK", null, false, true),
            new(10,  "Perform push pull on all plugs and find OK", "Perform push pull on all plugs and find OK", CommentType.String, true, false),
            new(11,  "Correct routing of looms", "Correct routing of looms", CommentType.String, true, false),
            new(12,  "Inspect Relay wiring condition and positioning", "Inspect Relay wiring condition and positioning", CommentType.String, true, false),
            new(13,  "Check correct side mirrors are fitted", "Check correct side mirrors are fitted", CommentType.String, true, false),
     };

    // Seed Records
    private static string Normalize(string value)
        => (value ?? string.Empty).Trim().ToUpperInvariant();
    private sealed record ListItemSeed(
        int Position,
        string Name,
        string CommentPlaceholder,
        CommentType? CommentType,
        bool IsAttachmentRequired,
        bool IsSeparator
    );
}
