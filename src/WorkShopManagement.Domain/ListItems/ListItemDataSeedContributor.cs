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

public class ListItemDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private const string Station0Name = "Station 0 - Receiving Compliance Audit";
    private const string Station1AName = "Station 1A";

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
        if (await _listItemRepository.AnyAsync())
        {
            _logger.LogInformation("ListItem data already exists. Skipping.");
            return;
        }

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
        var existingByPosition = existingItems.ToDictionary(x => x.Position);
        var existingNames = existingItems
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => x.Name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var insertedItems = 0;

        foreach (var s in listItemSeeds.OrderBy(x => x.Position))
        {
            if (existingByPosition.ContainsKey(s.Position) || existingNames.Contains(s.Name))
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
            insertedItems++;
        }

        return insertedItems;
    }

    // -----------------------
    // Station 0 Seeds
    // -----------------------

    private static List<ListItemSeed> GetStation0_ListItems()
        => new()
        {
            new(1,  "Initial Wash & Damage Inspection", "Initial Wash & Damage Inspection", null, false, true),
            new(2,  "Perform initial wash", "Perform initial wash", CommentType.String, true, false),
            new(3,  "Perform initial damage inspection", "Perform initial damage inspection", CommentType.String, true, false),

            new(4,  "Information Stickers Installed", "Information Stickers Installed", null, false, true),
            new(5,  "Key tag", "Key tag", CommentType.String, true, false),
            new(6,  "Customer Information Label Installed", "Customer Information Label Installed", CommentType.String, true, false),

            new(7,  "Vehicle Inspection", "Vehicle Inspection", null, false, true),
            new(8,  "Key Fobs", "Key Fobs", CommentType.String, true, false),
            new(9,  "Lock Nuts", "Lock Nuts", CommentType.String, true, false),
            new(10, "User's Guide", "User's Guide", CommentType.String, true, false),
            new(11, "Antenna", "Antenna", CommentType.String, true, false),
            new(12, "Secure Pin Card", "Secure Pin Card", CommentType.String, true, false),
            new(13, "Floor Mats", "Floor Mats", CommentType.String, true, false),

            new(14, "Vehicle Photos", "Vehicle Photos", null, false, true),
            new(15, "Dash Photo", "Dash Photo", CommentType.String, true, false),
            new(16, "Use WiTech to scan vehicle and upload photo of general summary report",
                "Use WiTech to scan vehicle and upload photo of general summary report", CommentType.String, true, false),
            new(17, "Use WiTech to scan vehicle and upload photo of recall summary report tab",
                "Use WiTech to scan vehicle and upload photo of recall summary report tab", CommentType.String, true, false),

            new(18, "Sticker Installation", "Sticker Installation", null, false, true),
            new(19, "Tyre placard replaced - ADR 42/05", "Tyre placard replaced - ADR 42/05", CommentType.String, true, false),
            new(20, "Tow Bar Label", "Tow Bar Label", CommentType.String, true, false),
            new(21, "OEM build plate sighted – ADR 61/03", "OEM build plate sighted – ADR 61/03", CommentType.String, true, false),

            new(22, "Photos & Review", "Photos & Review", null, false, true),
            new(23, "Vehicle Photos", "Vehicle Photos", CommentType.String, true, false),
            new(24, "Photos Uploaded", "Photos Uploaded", CommentType.String, true, false),

            new(25, "Lighting", "Lighting", null, false, true),
            new(26, "LHS Headlight", "LHS Headlight", CommentType.String, true, false),
            new(27, "RHS Headlight", "RHS Headlight", CommentType.String, true, false),
            new(28, "LHS Taillight", "LHS Taillight", CommentType.String, true, false),
            new(29, "RHS Taillight", "RHS Taillight", CommentType.String, true, false),
            new(30, "CHMSL", "CHMSL", CommentType.String, true, false),
            new(31, "Left Mirror Indicator", "Left Mirror Indicator", CommentType.String, true, false),
            new(32, "Right Mirror Indicator", "Right Mirror Indicator", CommentType.String, true, false),

            new(33, "Glassing", "Glassing", null, false, true),
            new(34, "Windscreen", "Windscreen", CommentType.String, true, false),
            new(35, "LHS Front Window", "LHS Front Window", CommentType.String, true, false),
            new(36, "RHS Front Window", "RHS Front Window", CommentType.String, true, false),
            new(37, "LHS Rear Window", "LHS Rear Window", CommentType.String, true, false),
            new(38, "RHS Rear Window", "RHS Rear Window", CommentType.String, true, false),
            new(39, "Rear Window", "Rear Window", CommentType.String, true, false),

            new(40, "Sunroof Front", "Sunroof Front", CommentType.String, true, false),
            new(41, "Sunroof Rear", "Sunroof Rear", CommentType.String, true, false),
            new(42, "Interior Rear Mirror", "Interior Rear Mirror", CommentType.String, true, false),

            new(43, "Tyres", "Tyres", null, false, true),
            new(44, "Front & Rear Tyres", "Front & Rear Tyres", CommentType.String, true, false),

            new(45, "Damage and Corrosion Check", "Damage and Corrosion Check", null, false, true),
            new(46, "Structural Damage Detected", "Structural Damage Detected", CommentType.String, true, false),
            new(47, "Levels of Damage", "Levels of Damage", CommentType.String, true, false),
            new(48, "Body Alignment Check", "Body Alignment Check", CommentType.String, true, false),
            new(49, "Deterioration check", "Deterioration check", CommentType.String, true, false),
            new(50, "Signs of odometer tampering", "Signs of odometer tampering", CommentType.String, true, false),
            new(51, "Vehicle ready for Production", "Vehicle ready for Production", CommentType.String, true, false),
        };

    // -----------------------
    // Station 1A Seeds
    // -----------------------

    private static List<ListItemSeed> GetStation1A_ListItems()
        => new()
        {
            new(1,  "Prepare Vehicle - Wash Vehicle, Remove Dealer Plates & Stickers, Apply Deflectors & AUSMV Sticker - ADR 47/00",
                "Prepare Vehicle - Wash Vehicle, Remove Dealer Plates & Stickers, Apply Deflectors & AUSMV Sticker - ADR 47/00",
                CommentType.String, true, false),

            new(2,  "Disassemble Trims", "Disassemble Trims", CommentType.String, true, false),
            new(3,  "Disassemble Door Cards", "Disassemble Door Cards", CommentType.String, true, false),
            new(4,  "Disassemble Seats", "Disassemble Seats", CommentType.String, true, false),
            new(5,  "Disassemble B-Pillar Trims & Seat Belt Assemblies", "Disassemble B-Pillar Trims & Seat Belt Assemblies", CommentType.String, true, false),
            new(6,  "Disassemble the Glovebox and Undertray", "Disassemble the Glovebox and Undertray", CommentType.String, true, false),
            new(7,  "Disassemble Neutral Release & Shifter Cable", "Disassemble Neutral Release & Shifter Cable", CommentType.String, true, false),
            new(8,  "Disassemble Centre Console, Carpet, Ducting and Centre Looms", "Disassemble Centre Console, Carpet, Ducting and Centre Looms", CommentType.String, true, false),
            new(9,  "Disassemble Airbag, Steering Wheel & Steering Column", "Disassemble Airbag, Steering Wheel & Steering Column", CommentType.String, true, false),
            new(10, "Remove Child Restraints - Place in Storage Box @ Bay 1", "Remove Child Restraints - Place in Storage Box @ Bay 1", CommentType.String, true, false),

            new(11, "Disassemble Dash Looms, Related Components & Remove Dash Assembly", "Disassemble Dash Looms, Related Components & Remove Dash Assembly", CommentType.String, true, false),
            new(12, "Disassemble HVAC", "Disassemble HVAC", CommentType.String, true, false),
            new(13, "Disassemble the Accelerator Pedal", "Disassemble the Accelerator Pedal", CommentType.String, true, false),
            new(14, "Disassemble Brake Pedal", "Disassemble Brake Pedal", CommentType.String, true, false),
            new(15, "Degas Vehicle A/C System using the correct A/C machine", "Degas Vehicle A/C System using the correct A/C machine", CommentType.String, true, false),
            new(16, "Disassemble Wipers & Wiper Trims", "Disassemble Wipers & Wiper Trims", CommentType.String, true, false),
            new(17, "Disassemble Vehicle Side Mirrors - Place in Storage Box @ Bay 1", "Disassemble Vehicle Side Mirrors - Place in Storage Box @ Bay 1", CommentType.String, true, false),
            new(18, "Disconnect Battery - Rear Battery if Applicable", "Disconnect Battery - Rear Battery if Applicable", CommentType.String, true, false),
            new(19, "Raise the Vehicle", "Raise the Vehicle", CommentType.String, true, false),
            new(20, "Remove the Wheel Guards", "Remove the Wheel Guards", CommentType.String, true, false),
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

    // -----------------------
    // Seed Records
    // -----------------------

    private sealed record ListItemSeed(
        int Position,
        string Name,
        string CommentPlaceholder,
        CommentType? CommentType,
        bool IsAttachmentRequired,
        bool IsSeparator
    );
}
