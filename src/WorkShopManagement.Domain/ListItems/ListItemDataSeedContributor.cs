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
                listItemSeeds: GetStation1B_ListItems()
                );

            totalInsertedItems += await SeedCheckListItemsAsync(
               carModelId: carModel.Id,
               checkListName: Station2Name,
               listItemSeeds: Station2_ListItems()
               );

            totalInsertedItems += await SeedCheckListItemsAsync(
                carModelId: carModel.Id,
                checkListName: Station3AName,
                listItemSeeds: Station3A_ListItems()
                );

            totalInsertedItems += await SeedCheckListItemsAsync(
              carModelId: carModel.Id,
              checkListName: Station3BName,
              listItemSeeds: Station3B_ListItems()
              );

            totalInsertedItems += await SeedCheckListItemsAsync(
               carModelId: carModel.Id,
               checkListName: Station4Name,
               listItemSeeds: Station4_ListItems()
               );

            totalInsertedItems += await SeedCheckListItemsAsync(
            carModelId: carModel.Id,
            checkListName: Station5QCName,
            listItemSeeds: Station5QC_ListItems()
            );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: WheelAlignmentName,
             listItemSeeds: WheelAlignment_ListItems()
             );


            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: QualityControlName,
             listItemSeeds: QualityControl_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: QualityReleaseName,
             listItemSeeds: QualityRelease_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: DashRemanufactureName,
             listItemSeeds: DashRemanufacture_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: HVACName,
             listItemSeeds: HVAC_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
              carModelId: carModel.Id,
              checkListName: CentreConsoleName,
              listItemSeeds: CentreConsole_ListItems()
              );

            totalInsertedItems += await SeedCheckListItemsAsync(
               carModelId: carModel.Id,
               checkListName: SeatsConversionName,
               listItemSeeds: SeatsConversion_ListItems()
               );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: LeatherSeatKitName,
             listItemSeeds: LeatherSeatKit_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: SubAssemblyElectricalName,
             listItemSeeds: SubAssemblyElectrical_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: InvoiceName,
             listItemSeeds: Invoice_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: PreDeliveryInspectioneName,
             listItemSeeds: GetPreDeliveryInspection_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
             carModelId: carModel.Id,
             checkListName: ProcurementName,
             listItemSeeds: Procurement_ListItems()
             );

            totalInsertedItems += await SeedCheckListItemsAsync(
               carModelId: carModel.Id,
               checkListName: AVVPackageName,
               listItemSeeds: AVVPackage_ListItems()
               );


            totalInsertedItems += await SeedCheckListItemsAsync(
               carModelId: carModel.Id,
               checkListName: QualityName,
               listItemSeeds: Quality_ListItems()
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
            new(33, "RHS Headlight", "RHS Headlight", CommentType.String, true, false),
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

    private static List<ListItemSeed> GetStation1A_ListItems()
    => new()
    {
        new(1,  "Disassemble Frunk", "Disassemble Frunk", CommentType.String, true, false),
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
        new(14, "Disassemble the Centre Console", "Disassemble the Centre Console", CommentType.String, true, false),
        new(15, "Disassemble Glovebox & Undertray", "Disassemble Glovebox & Undertray", CommentType.String, true, false),
        new(16, "Disassemble the Airbag & Steering Wheel", "Disassemble the Airbag & Steering Wheel", CommentType.String, true, false),
        new(17, "Disassemble the Steering Column", "Disassemble the Steering Column", CommentType.String, true, false),
        new(18, "Remove Floor Items and Carpet", "Remove Floor Items and Carpet", CommentType.String, true, false),
        new(19, "Disassemble the Dash", "Disassemble the Dash", CommentType.String, true, false),
        new(20, "Disassemble IPMA Module", "Disassemble IPMA Module", CommentType.String, true, false),
        new(21, "Disassemble the Brake Pedal", "Disassemble the Brake Pedal", CommentType.String, true, false),
        new(22, "Disassemble the Accelerator Pedal", "Disassemble the Accelerator Pedal", CommentType.String, true, false),
        new(23, "Disassemble HVAC", "Disassemble HVAC", CommentType.String, true, false),
        new(24, "Disassemble Intermediate Shaft", "Disassemble Intermediate Shaft", CommentType.String, true, false),
        new(25, "Disassemble the Brake Booster", "Disassemble the Brake Booster", CommentType.String, true, false),
        new(26, "Disassemble the Wipers", "Disassemble the Wipers", CommentType.String, true, false),
        new(27, "Disassemble Aerial Antenna", "Disassemble Aerial Antenna", CommentType.String, true, false),
        new(28, "Disassemble the LHS Wiper Scuttle", "Disassemble the LHS Wiper Scuttle", CommentType.String, true, false),
        new(29, "Disassemble the RHS Wiper Scuttle", "Disassemble the RHS Wiper Scuttle", CommentType.String, true, false),
        new(30, "Disassemble the Wiper Assembly", "Disassemble the Wiper Assembly", CommentType.String, true, false),
        new(31, "Install RHS Indicator - ADR 06/00", "Install RHS Indicator - ADR 06/00", CommentType.String, true, false),
        new(32, "Install LHS Indicator - ADR 06/00", "Install LHS Indicator - ADR 06/00", CommentType.String, true, false),
        new(33, "Cut Wiper Scuttle", "Cut Wiper Scuttle", CommentType.String, true, false),
        new(34, "Install Wipers", "Install Wipers", CommentType.String, true, false),

        // Section header
        new(35, "QC Checklist", "QC Checklist", null, false, true),

        new(36, "Confirm HVAC/Wiper Assembly Water Shield Installed", "Confirm HVAC/Wiper Assembly Water Shield Installed", CommentType.String, true, false),

        // PASS/FAIL items
        new(37, "Vehicle Electrical System has been disabled and locked out.", "Vehicle Electrical System has been disabled and locked out.", CommentType.String, true, false),
        new(38, "No Damages", "No Damages", CommentType.String, true, false),
        new(39, "All fasteners removed and placed in correct location", "All fasteners removed and placed in correct location", CommentType.String, true, false),
        new(40, "Clock spring has been removed & zip tied", "Clock spring has been removed & zip tied", CommentType.String, true, false),
        new(41, "The VIN Stickers have been checked against the VIN of the vehicle and bay number before fitment.", "The VIN Stickers have been checked against the VIN of the vehicle and bay number before fitment.", CommentType.String, true, false),
        new(42, "No Damages, no cuts of lines or looms", "No Damages, no cuts of lines or looms", CommentType.String, true, false),

        // As shown on the screen (repeated item)
        new(43, "Disassemble the Wipers", "Disassemble the Wipers", CommentType.String, true, false),
    };

    private static List<ListItemSeed> GetStation1B_ListItems()
   => new()
   {
        new(1,  "Disassemble Tail Lights", "Disassemble Tail Lights", CommentType.String, true, false),
        new(2,  "Run Wiring Front To Back", "Run Wiring Front To Back", CommentType.String, true, false),
        new(3,  "Install Trailer Wiring", "Install Trailer Wiring", CommentType.String, true, false),
        new(4,  "Install LHS Tail Light - ADR 13/00 ADR 06/00 ADR 01/00", "Install LHS Tail Light - ADR 13/00 ADR 06/00 ADR 01/00", CommentType.String, true, false),
        new(5,  "Install RHS Tail Light - ADR 13/00 ADR 06/00 ADR 01/00", "Install RHS Tail Light - ADR 13/00 ADR 06/00 ADR 01/00", CommentType.String, true, false),
        new(6,  "Install Mudflaps - ADR 42/05", "Install Mudflaps - ADR 42/05", CommentType.String, true, false),
        new(7,  "Modify Side Mirrors - ADR 14/02", "Modify Side Mirrors - ADR 14/02", CommentType.String, true, false),
        new(8,  "Install Power Access Blanks", "Install Power Access Blanks", CommentType.String, true, false),

        // Section header
        new(9,  "QC Checklist", "QC Checklist", null, false, true),

        // QC items (still normal list items)
        new(10, "Check correct side mirrors are fitted", "Check correct side mirrors are fitted", CommentType.String, true, false),
        new(11, "Wiring secured correctly down the chassis.", "Wiring secured correctly down the chassis.", CommentType.String, true, false),
   };
    private static List<ListItemSeed> Station3A_ListItems()
   => new()
   {
        new(1,  "RHS Door Loom Installation", "RHS Door Loom Installation", CommentType.String, true, false),
        new(2,  "LHS Door Loom Installation", "LHS Door Loom Installation", CommentType.String, true, false),
        new(3,  "Install Crossover Loom", "Install Crossover Loom", CommentType.String, true, false),
        new(4,  "Modify BCM Connector 1", "Modify BCM Connector 1", CommentType.String, true, false),
        new(5,  "Modify BCM Connector 2", "Modify BCM Connector 2", CommentType.String, true, false),
        new(6,  "Prep Relay Switch", "Prep Relay Switch", CommentType.String, true, false),
        new(7,  "Install Relay Switch", "Install Relay Switch", CommentType.String, true, false),
        new(8,  "Continuity Test", "Continuity Test", CommentType.String, true, false),
        new(9,  "Prep RHS Modules", "Prep RHS Modules", CommentType.String, true, false),
        new(10, "Install RHS Modules", "Install RHS Modules", CommentType.String, true, false),
        new(11, "Install Accelerator Pedal", "Install Accelerator Pedal", CommentType.String, true, false),
        new(12, "Install Seat Looms", "Install Seat Looms", CommentType.String, true, false),
        new(13, "Roof Mic Conversion", "Roof Mic Conversion", CommentType.String, true, false),

        new(14, "QC Checklist", "QC Checklist", null, false, true),

        new(15, "No Damages or cuts to looms", "No Damages or cuts to looms", CommentType.String, true, false),
        new(16, "All door looms are connected and continuity tested OK", "All door looms are connected and continuity tested OK", CommentType.String, true, false),
        new(17, "Perform push pull on all plugs and find OK", "Perform push pull on all plugs and find OK", CommentType.String, true, false),
        new(18, "Routing of looms completed to expected standard", "Routing of looms completed to expected standard", CommentType.String, true, false),
        new(19, "Inspect Relay wiring condition and positioning", "Inspect Relay wiring condition and positioning", CommentType.String, true, false),
   };

    private static List<ListItemSeed> Invoice_ListItems()
     => new()
     {
            new(1,  "Dealer Invoice Number","Dealer Invoice Number",CommentType.String, true, false),
            new(2,  "Dealer Invoice Paid","Dealer Invoice Paid",CommentType.Date, true, false),
            new(3,  "Customer Invoice Number","Customer Invoice Number",CommentType.String, true, false),
            new(4,  "Customer Invoice Paid","Customer Invoice Paid",CommentType.Date, true, false),
     };
    private static List<ListItemSeed> GetPreDeliveryInspection_ListItems()
        => new()
        {
        new(1,  "PRE-DELIVERY INSPECTION", "PRE-DELIVERY INSPECTION", null, false, true),
        new(2,  "VEHICLE HISTORY", "VEHICLE HISTORY", null, false, true),

        new(3,  "VIN Inspection", "VIN Inspection", CommentType.String, false, false),
        new(4,  "PMA Inspection - All Green No Issues", "PMA Inspection - All Green No Issues", CommentType.String, false, false),
        new(5,  "No Pending Recalls/TSB's", "No Pending Recalls/TSB's", CommentType.String, false, false),

        new(6,  "VEHICLE EXTERIOR", "VEHICLE EXTERIOR", null, false, true),

        new(7,  "360° Visual Inspection of Panel & Paint", "360° Visual Inspection of Panel & Paint", CommentType.String, false, false),
        new(8,  "No Evidence of Flood or Fire", "No Evidence of Flood or Fire", CommentType.String, false, false),
        new(9,  "No Major/Minor Hail Damage", "No Major/Minor Hail Damage", CommentType.String, false, false),
        new(10, "AUSEV Decals Present", "AUSEV Decals Present", CommentType.String, false, false),
        new(11, "Tailgate Operates as Expected", "Tailgate Operates as Expected", CommentType.String, false, false),
        new(12, "Charge Door Stickers & Function", "Charge Door Stickers & Function", CommentType.String, false, false),
        new(13, "Aerial Present on Vehicle", "Aerial Present on Vehicle", CommentType.String, false, false),
        new(14, "Tyres, Wheels & Valve Caps", "Tyres, Wheels & Valve Caps", CommentType.String, false, false),
        new(15, "Towbar & Towbar Sticker Present", "Towbar & Towbar Sticker Present", CommentType.String, false, false),
        new(16, "Glass & Mirrors - No Cracks or Chips", "Glass & Mirrors - No Cracks or Chips", CommentType.String, false, false),
        new(17, "Lights Functioning as Expected", "Lights Functioning as Expected", CommentType.String, false, false),
        new(18, "Tail/Head Lights Lens Free of Cracks", "Tail/Head Lights Lens Free of Cracks", CommentType.String, false, false),
        new(19, "Vehicle Tray - Free of Items", "Vehicle Tray - Free of Items", CommentType.String, false, false),

        new(20, "UNDERHOOD", "UNDERHOOD", null, false, true),

        new(21, "Frunk Lid Operates as Expected", "Frunk Lid Operates as Expected", CommentType.String, false, false),
        new(22, "Free of Items & Clean", "Free of Items & Clean", CommentType.String, false, false),
        new(23, "Washer Fluid Level - Full?", "Washer Fluid Level - Full?", CommentType.String, false, false),
        new(24, "12V Battery State of Health & Secure", "12V Battery State of Health & Secure", CommentType.String, false, false),

        new(25, "VEHICLE INTERIOR", "VEHICLE INTERIOR", null, false, true),

        new(26, "Window Function - Auto Up/Down", "Window Function - Auto Up/Down", CommentType.String, false, false),
        new(27, "Seats and Seat Functions (Heated Seats)", "Seats and Seat Functions (Heated Seats)", CommentType.String, false, false),
        new(28, "Seat Belts - Functioning as Expected", "Seat Belts - Functioning as Expected", CommentType.String, false, false),
        new(29, "A/C & Heater - Cold & Hot", "A/C & Heater - Cold & Hot", CommentType.String, false, false),
        new(30, "Tilt/Telescoping Steering Column", "Tilt/Telescoping Steering Column", CommentType.String, false, false),
        new(31, "Steering Function, Controls & Horn", "Steering Function, Controls & Horn", CommentType.String, false, false),
        new(32, "Wiper and Washers Functioning", "Wiper and Washers Functioning", CommentType.String, false, false),
        new(33, "Interior Lights - Functioning as Expected", "Interior Lights - Functioning as Expected", CommentType.String, false, false),
        new(34, "Power Outlets - Functioning as Expected", "Power Outlets - Functioning as Expected", CommentType.String, false, false),
        new(35, "CIN, Tyre Placard, Air Bag & AUSEV Stickers", "CIN, Tyre Placard, Air Bag & AUSEV Stickers", CommentType.String, false, false),
        new(36, "No Error Codes or Symbols on Cluster", "No Error Codes or Symbols on Cluster", CommentType.String, false, false),
        new(37, "Side Mirrors Function as Expected", "Side Mirrors Function as Expected", CommentType.String, false, false),
        new(38, "Carpet & Floor Matts Secure", "Carpet & Floor Matts Secure", CommentType.String, false, false),
        new(39, "Interior Condition is Clean & No Rubbish", "Interior Condition is Clean & No Rubbish", CommentType.String, false, false),

        new(40, "CONVENIENCE", "CONVENIENCE", null, false, true),

        new(41, "Owners Manual", "Owners Manual", CommentType.String, false, false),
        new(42, "2x Key FOBs & Emergency Key", "2x Key FOBs & Emergency Key", CommentType.String, false, false),
        new(43, "Radio Stations Set - No Static", "Radio Stations Set - No Static", CommentType.String, false, false),
        new(44, "Phone List Cleared", "Phone List Cleared", CommentType.String, false, false),
        new(45, "One Pedal Drive Set On", "One Pedal Drive Set On", CommentType.String, false, false),
        new(46, "Vehicle is in Km/h & °C", "Vehicle is in Km/h & °C", CommentType.String, false, false),
        new(47, "Trip Meters Reset (Trip 1 & 2)", "Trip Meters Reset (Trip 1 & 2)", CommentType.String, false, false),
        new(48, "Road Test Passed - All ok", "Road Test Passed - All ok", CommentType.String, false, false),
        new(49, "State of Charge = > 75%", "State of Charge = > 75%", CommentType.String, false, false),

        new(50, "QUALITY CONTAINMENT ACTIONS", "QUALITY CONTAINMENT ACTIONS", null, false, true),

        new(51, "VEHICLE INSPECTIONS", "VEHICLE INSPECTIONS", null, false, true),

        new(52, "Child Restraint Zip Tags Removed -ex", "Child Restraint Zip Tags Removed -ex", CommentType.String, false, false),
        new(53, "Cup Holders not cracked & Matts -ex", "Cup Holders not cracked & Matts -ex", CommentType.String, false, false),
        new(54, "Towbar Sticker has correct information -ex", "Towbar Sticker has correct information -ex", CommentType.String, false, false),
        new(55, "RHS Wiper Scuttle Position -ex", "RHS Wiper Scuttle Position -ex", CommentType.String, false, false),
        new(56, "RHS Kick Trim Weather Seal Position -ex", "RHS Kick Trim Weather Seal Position -ex", CommentType.String, false, false),
        new(57, "Reverse Camera Working -ex", "Reverse Camera Working -ex", CommentType.String, false, false),
        new(58, "HVAC Condensation Leakage -ex", "HVAC Condensation Leakage -ex", CommentType.String, false, false),

        new(59, "TECHNICAL SERVICE BULLETINS", "TECHNICAL SERVICE BULLETINS", null, false, true),

        new(60, "HVAC Stopper TSB -ex", "HVAC Stopper TSB -ex", CommentType.String, false, false),
        new(61, "Dash Stay Bolt TSB -ex", "Dash Stay Bolt TSB -ex", CommentType.String, false, false),
        new(62, "Wiper Arm TSB -ex", "Wiper Arm TSB -ex", CommentType.String, false, false),
        new(63, "HVAC Connectors TSB -ex", "HVAC Connectors TSB -ex", CommentType.String, false, false),

        new(64, "PDI ASSESSMENT", "PDI ASSESSMENT", null, false, true),

        new(65, "PDI Status", "PDI Status", CommentType.String, false, false),

        new(66, "DISPATCHMENT PHOTOS", "DISPATCHMENT PHOTOS", null, false, true),

        new(67, "! - - - Additional Photo's Must be Taken Before Vehicle Handover - - - !",
                "! - - - Additional Photo's Must be Taken Before Vehicle Handover - - - !",
                null, false, true),

        new(68, "! - - - As the vehicle is being placed on the truck for transport or handed to the Customer - - - !",
                "! - - - As the vehicle is being placed on the truck for transport or handed to the Customer - - - !",
                null, false, true),

        new(69, "Photo 1 : LHS (45 Deg. From Front)", "Photo 1 : LHS (45 Deg. From Front)", CommentType.String, true, false),
        new(70, "Photo 2 : RHS (45 Deg. From Front)", "Photo 2 : RHS (45 Deg. From Front)", CommentType.String, true, false),
        new(71, "Photo 3 : Rear", "Photo 3 : Rear", CommentType.String, true, false),
        new(72, "Photo 4 : ODO", "Photo 4 : ODO", CommentType.String, true, false),
        new(73, "Photo 5 : VIN - Build Plate", "Photo 5 : VIN - Build Plate", CommentType.String, true, false),
        };

    private static List<ListItemSeed> Procurement_ListItems()
      => new()
      {
            new(1,  "Remanufacture invoice issued to correct supplier ENTER INVOICE NUMBER IN COMMENTS -->","Enter invoice number",CommentType.String, true, false),
            new(2,  "Invoice emailed","Invoice emailed",CommentType.String, true, false),
      };

    private static List<ListItemSeed> AVVPackage_ListItems()
   => new()
   {
        new(1,  "AVV Package Review","AVV Package Review",CommentType.String, true, false),
   };

    private static List<ListItemSeed> Quality_ListItems()
     => new()
     {
        new(1,  "LIN Module Rework","LIN Module Rework",CommentType.String, true, false),
     };

    private static List<ListItemSeed> SubAssemblyElectrical_ListItems()
       => new()
       {
            new(1,  "Dash Loom (IF GB install GB wiring, splice the wiring into the instrument cluster CAN signal as per drawing)","Dash Loom (IF GB install GB wiring, splice the wiring into the instrument cluster CAN signal as per drawing)",CommentType.String, true, false),
       };
    private static List<ListItemSeed> LeatherSeatKit_ListItems()
       => new()
       {
            new(1,  "If XLT (else mark N/A), confirm leather seat kit has been installed","If XLT (else mark N/A), confirm leather seat kit has been installed",CommentType.String, true, false),
       };

    private static List<ListItemSeed> SeatsConversion_ListItems()
      => new()
      {
            new(1,  "Disassemble seats (remove side panels, leather cover, seats foam passenger presence sensor)","Disassemble seats (remove side panels, leather cover, seats foam passenger presence sensor)",CommentType.String, true, false),
            new(2,  "Mark the foam using template, cut the foam and glue them in the opposite position. (Seat A goes on Seat B and vice versa)",
                    "Mark the foam using template, cut the foam and glue them in the opposite position. (Seat A goes on Seat B and vice versa)",CommentType.String, true, false),
            new(3,  "Swap passenger presence sensor and foam cushions.","Swap passenger presence sensor and foam cushions.Mark the foam using template, cut the foam and glue them in the opposite position. (Seat A goes on Seat B and vice versa",CommentType.String, true, false),
            new(4,  "SWAP SEAT BELT BUCKLES (RHS goes to LHS and vice versa)",
                "SWAP SEAT BELT BUCKLES (RHS goes to LHS and vice versa)",CommentType.String, true, false),
            new(5,  "Assemble seats back together, make sure the leather is nice and tucked in then install side panel.",
                    "Assemble seats back together, make sure the leather is nice and tucked in then install side panel.",CommentType.String, true, false),
            new(6,  "Manager Signoff, DO NOT DELIVER THE SEATS IF THE BEFORE QC BUCKLES. (Charles Mitchell)",
                "Manager Signoff, DO NOT DELIVER THE SEATS IF THE BEFORE QC BUCKLES. (Charles Mitchell)",CommentType.String, true, false),
      };

    private static List<ListItemSeed> CentreConsole_ListItems()
     => new()
     {
        new(1,  "Disassemble centre console","Disassemble centre console",CommentType.String, true, false),
        new(2,  "Convert top housing and gear selector","Convert top housing and gear selector",CommentType.String, true, false),
        new(3,  "Assemble centre console","Assemble centre console",CommentType.String, true, false),
        new(4,  "(IF NOT UN) plug AC110V socket and tape it on the loom behind the centre console","(IF NOT UN) plug AC110V socket and tape it on the loom behind the centre console",CommentType.String, true, false),
        new(5,  "(IF NOT UN) install AC110V blank (IF UN) leave AC110V socket on centre console","(IF NOT UN) install AC110V blank (IF UN) leave AC110V socket on centre console",CommentType.String, true, false),
     };

    private static List<ListItemSeed> HVAC_ListItems()
    => new()
    {
        new(1,  "Disassemble HVAC completely.",
                "Disassemble HVAC completely.", CommentType.String, false, false),

        new(2,  "Convert foot vent",
                "Convert foot vent", CommentType.String, false, false),

        new(3,  "Cut out OEM head unit",
                "Cut out OEM head unit", CommentType.String, false, false),

        new(4,  "Assemble fresh air inlet (vane and motor).",
                "Assemble fresh air inlet (vane and motor).", CommentType.String, false, false),

        new(5,  "Assemble the top scroll and the head unit together. Use OEM screws.",
                "Assemble the top scroll and the head unit together. Use OEM screws.",
                CommentType.String, false, false),

        new(6,  "Install OEM flaps (brown flaps)",
                "Install OEM flaps (brown flaps)", CommentType.String, false, false),

        new(7,  "Install flow splitter on the clam shell and OEM flap",
                "Install flow splitter on the clam shell and OEM flap",
                CommentType.String, false, false),

        new(8,  "Install second flaps into the split section and the bottom clam shell.",
                "Install second flaps into the split section and the bottom clam shell.",
                CommentType.String, false, false),

        new(9,  "Install the new cog actuators for top and bottom motors. TOP MOTOR COG HAS 2 RIDGES.",
                "Install the new cog actuators for top and bottom motors. TOP MOTOR COG HAS 2 RIDGES.",
                CommentType.String, false, false),

        new(10, "Install bottom fan and fresh air inlet with motor",
                "Install bottom fan and fresh air inlet with motor",
                CommentType.String, false, false),

        new(11, "Install remanufactured loom. SWAP TOP AND BOTTOM PLUGS (top goes at bottom and bottom goes at top.)",
                "Install remanufactured loom. SWAP TOP AND BOTTOM PLUGS (top goes at bottom and bottom goes at top.)",
                CommentType.String, false, false),

        new(12, "Carpet the bottom area of the HVAC",
                "Carpet the bottom area of the HVAC",
                CommentType.String, false, false),
    };
    private static List<ListItemSeed> DashRemanufacture_ListItems()
    => new()
    {
        // ---------- STAGE 1 ----------
        new(1,  "STAGE 1", "STAGE 1", null, false, true),

        new(2,  "Inspect dash", "Inspect dash", CommentType.String, true, false),
        new(3,  "Disassemble dash", "Disassemble dash", CommentType.String, true, false),
        new(4,  "Remove dashtop and hat from OEM housings", "Remove dashtop and hat from OEM housings", CommentType.String, true, false),
        new(5,  "Cut OEM skin", "Cut OEM skin", CommentType.String, true, false),
        new(6,  "Swap brackets from OEM to remanufactured frame", "Swap brackets from OEM to remanufactured frame", CommentType.String, true, false),
        new(7,  "Swap modules and components from OEM plastic to remanufactured parts", "Swap modules and components from OEM plastic to remanufactured parts", CommentType.String, true, false),
        new(8,  "IF GB Install controller panel on center stack", "IF GB Install controller panel on center stack", CommentType.String, true, false),

        // ---------- STAGE 2 ----------
        new(9,  "STAGE 2", "STAGE 2", null, false, true),

        new(10, "Install Loom making sure the location of the main branches is correct.", "Install Loom making sure the location of the main branches is correct.", CommentType.String, true, false),
        new(11, "(IF NOT UN) plug AC110V socket and tape it on the loom behind the dash and blank the centre stack",
                 "(IF NOT UN) plug AC110V socket and tape it on the loom behind the dash and blank the centre stack", CommentType.String, true, false),
        new(12, "(IF UN) leave AC110V socket on centre stack", "(IF UN) leave AC110V socket on centre stack", CommentType.String, true, false),
        new(13, "Install dash skin on frame", "Install dash skin on frame", CommentType.String, true, false),
        new(14, "Install demister ducting", "Install demister ducting", CommentType.String, true, false),
        new(15, "Install Airbag and Hat housings back on dash", "Install Airbag and Hat housings back on dash", CommentType.String, true, false),
        new(16, "Send photo of the airbag bolted on frame with VIN number attached. Team leader pen mark",
                 "Send photo of the airbag bolted on frame with VIN number attached. Team leader pen mark", CommentType.String, true, false),
        new(17, "Install headlamp panel with components", "Install headlamp panel with components", CommentType.String, true, false),
        new(18, "Prep cluster surround and install components on RHS", "Prep cluster surround and install components on RHS", CommentType.String, true, false),
        new(19, "Prep and install upper glovebox and lower glovebox", "Prep and install upper glovebox and lower glovebox", CommentType.String, true, false),
        new(20, "IF XLT Install button mechanism", "IF XLT Install button mechanism", CommentType.String, true, false),
        new(21, "Install Long bar Short bar and air vent insert", "Install Long bar Short bar and air vent insert", CommentType.String, true, false),
        new(22, "QC video", "QC video", CommentType.String, true, false),

        // ---------- TEAM LEADER ONLY ----------
        new(23, "TEAM LEADER ONLY", "TEAM LEADER ONLY", null, false, true),

        new(24, "Make sure demister ducting is connected properly.", "Make sure demister ducting is connected properly.", CommentType.String, true, false),
        new(25, "Check earth bolts. Check for full contact on the frame and no loose bolt, pen mark.",
                 "Check earth bolts. Check for full contact on the frame and no loose bolt, pen mark.", CommentType.String, true, false),
        new(26, "Check top glovebox: open and close.", "Check top glovebox: open and close.", CommentType.String, true, false),
        new(27, "Inspect dash for gap or scratches", "Inspect dash for gap or scratches", CommentType.String, true, false),
        new(28, "Make sure airbag photo and VIN number has been sent on the Dashteam Chat.",
                 "Make sure airbag photo and VIN number has been sent on the Dashteam Chat.", CommentType.String, true, false),
    };
    private static List<ListItemSeed> QualityRelease_ListItems()
    => new()
    {
        new(1, "Check CIN QR Code for Functionality",
            "Check CIN QR Code for Functionality", CommentType.String, true, false),

        new(2, "Make sure the removal of label on the rear windscreen",
            "Make sure the removal of label on the rear windscreen", CommentType.String, true, false),

        new(3, "QUALITY ALERT - Inspect edges of carpet to ensure neat and tidy",
            "QUALITY ALERT - Inspect edges of carpet to ensure neat and tidy", CommentType.String, true, false),

        new(4, "Inspect and Ensure the Pro-Power Kit has been blanked off in both locations.",
            "Inspect and Ensure the Pro-Power Kit has been blanked off in both locations.", CommentType.String, true, false),

        new(5, "Completed QC Inspection sheet attached",
            "Completed QC Inspection sheet attached", CommentType.String, true, false),

        new(6, "Final Quality Release Inspection sheet attached",
            "Final Quality Release Inspection sheet attached", CommentType.String, true, false),
    };

    private static List<ListItemSeed> WheelAlignment_ListItems()
     => new()
     {
        new(1,  "Mark N/A until alignment is completed. ENTER DATE IN COMMENTS -->","Enter date",CommentType.Date, true, false),
     };
    private static List<ListItemSeed> QualityControl_ListItems()
        => new()
        {
        new(1,  "Walk around exterior and note any damage - Dents, Scratches, or anything that requires repairs",
            "Walk around exterior and note any damage - Dents, Scratches, or anything that requires repairs", CommentType.String, true, false),

        new(2,  "Check and fill windscreen washer fluid, Check and fill coolant bottle AFTER bleed procedure has been carried out. Must be on MAX line",
            "Check and fill windscreen washer fluid, Check and fill coolant bottle AFTER bleed procedure has been carried out. Must be on MAX line", CommentType.String, true, false),

        new(3,  "Perform Frunk inspection - Remove any parts such as OEM charger and yellow adapter plug - confirm Pro Power blank has been fitted",
            "Perform Frunk inspection - Remove any parts such as OEM charger and yellow adapter plug - confirm Pro Power blank has been fitted", CommentType.String, true, false),

        new(4,  "Check tyre pressures - set to 40 PSI, check wheels and tyres for damage at the same time",
            "Check tyre pressures - set to 40 PSI, check wheels and tyres for damage at the same time", CommentType.String, true, false),

        new(5,  "Confirm OEM build plate is fitted to the vehicle - ADR 61/03",
            "Confirm OEM build plate is fitted to the vehicle - ADR 61/03", CommentType.String, true, false),

        new(6,  "Ensure Tyre placard is correct - ADR 42/05",
            "Ensure Tyre placard is correct - ADR 42/05", CommentType.String, true, false),

        new(7,  "Ensure the vehicle has been mounted with wheel guards/Mudflaps ADR42/05",
            "Ensure the vehicle has been mounted with wheel guards/Mudflaps ADR42/05", CommentType.String, true, false),

        new(8,  "Ensure the vehicle has been mounted with Retroreflectors - ADR 47/00",
            "Ensure the vehicle has been mounted with Retroreflectors - ADR 47/00", CommentType.String, true, false),

        new(9,  "Check for metal swarf in door hinges and around wiper scuttle",
            "Check for metal swarf in door hinges and around wiper scuttle", CommentType.String, true, false),

        new(10, "Check Trailer plug using test board - All lights and trailer brake must be checked if fitted",
            "Check Trailer plug using test board - All lights and trailer brake must be checked if fitted", CommentType.String, true, false),

        new(11, "Check condition of dash, door cards, centre console, and all removed trims for scratches or damage",
            "Check condition of dash, door cards, centre console, and all removed trims for scratches or damage", CommentType.String, true, false),

        new(12, "Remove glovebox and check fitment of heater lines and retaining clips",
            "Remove glovebox and check fitment of heater lines and retaining clips", CommentType.String, true, false),

        new(13, "Check all seat belts including rear seats for function - pull all the way out and plug into buckle and check retract function",
            "Check all seat belts including rear seats for function - pull all the way out and plug into buckle and check retract function", CommentType.String, true, false),

        new(14, "Check the Seatbelts work correctly - ADR 04/06",
            "Check the Seatbelts work correctly - ADR 04/06", CommentType.String, true, false),

        new(15, "Moving under dash driver's side - check for paint marks on all steering components and check for loose hanging wiring",
            "Moving under dash driver's side - check for paint marks on all steering components and check for loose hanging wiring", CommentType.String, true, false),

        new(16, "Check steering function by moving wheel from lock to lock - No noises or Notches - ADR 90/00",
            "Check steering function by moving wheel from lock to lock - No noises or Notches - ADR 90/00", CommentType.String, true, false),

        new(17, "Check to ensure brake systems are functioning correctly - ADR 35/06",
            "Check to ensure brake systems are functioning correctly - ADR 35/06", CommentType.String, true, false),

        new(18, "Plug in Scan tool - Using FORSCAN carry out HCM and AFS coding",
            "Plug in Scan tool - Using FORSCAN carry out HCM and AFS coding", CommentType.String, true, false),

        new(19, "Using FDRS - Check for any fault codes and clear. If unable to clear, notify Supervisor",
            "Using FDRS - Check for any fault codes and clear. If unable to clear, notify Supervisor", CommentType.String, true, false),

        new(20, "Set Radio stations in radio, Set temperature to Celsius, turn OFF Vehicle Connectivity, Set 1 pedal drive to ON",
            "Set Radio stations in radio, Set temperature to Celsius, turn OFF Vehicle Connectivity, Set 1 pedal drive to ON", CommentType.String, true, false),

        new(21, "Check functionality of side mirrors - ADR 14/02",
            "Check functionality of side mirrors - ADR 14/02", CommentType.String, true, false),

        new(22, "Check all windows function from master switch including one touch up and down if fitted, check mirror controls and door locks",
            "Check all windows function from master switch including one touch up and down if fitted, check mirror controls and door locks", CommentType.String, true, false),

        new(23, "Check all window switches on each door",
            "Check all window switches on each door", CommentType.String, true, false),

        new(24, "Check windscreen wiper function on all speeds making sure wiper doesn’t hit A Pillar - ADR 42/05",
            "Check windscreen wiper function on all speeds making sure wiper doesn’t hit A Pillar - ADR 42/05", CommentType.String, true, false),

        new(25, "Check all HVAC controls - Vents change and air flows from corresponding vents. Check temperature changes using dual zones (hot and cold)",
            "Check all HVAC controls - Vents change and air flows from corresponding vents. Check temperature changes using dual zones (hot and cold)", CommentType.String, true, false),

        new(26, "Ensure the functionality of the demister for the windscreen - ADR 42/05",
            "Ensure the functionality of the demister for the windscreen - ADR 42/05", CommentType.String, true, false),

        new(27, "Check all exterior lights (Headlights, Highbeam, Fogs, Brake, Parkers, Reverse) - ADR 01/00 ADR 13/00",
            "Check all exterior lights (Headlights, Highbeam, Fogs, Brake, Parkers, Reverse) - ADR 01/00 ADR 13/00", CommentType.String, true, false),

        new(28, "Adjust headlights - Bringing kick below LHS headlight as per board",
            "Adjust headlights - Bringing kick below LHS headlight as per board", CommentType.String, true, false),

        new(29, "Check functionality of the Rear and Front Indicators - ADR 06/00",
            "Check functionality of the Rear and Front Indicators - ADR 06/00", CommentType.String, true, false),

        new(30, "Check AIRBAG sticker has been fitted to LHS dash end cap",
            "Check AIRBAG sticker has been fitted to LHS dash end cap", CommentType.String, true, false),

        new(31, "Ensure bonnet is the only panel in front of the windshield that is capable of moving, and is provided with a latch system - ADR 42/05",
            "Ensure bonnet is the only panel in front of the windshield that is capable of moving, and is provided with a latch system - ADR 42/05", CommentType.String, true, false),

        new(32, "Check ProPower blank has been fitted to Tub",
            "Check ProPower blank has been fitted to Tub", CommentType.String, true, false),

        new(33, "Ensure the Bonnet DRL has been turned off if XLT",
            "Ensure the Bonnet DRL has been turned off if XLT", CommentType.String, true, false),

        new(34, "Confirm the vehicle accepts charge using the 10 Amp Charger",
            "Confirm the vehicle accepts charge using the 10 Amp Charger", CommentType.String, true, false),

        new(35, "Confirm the vehicle accepts charge using the 15 Amp Charger",
            "Confirm the vehicle accepts charge using the 15 Amp Charger", CommentType.String, true, false),

        new(36, "Confirm the vehicle accepts charge using the 3 Phase Charger",
            "Confirm the vehicle accepts charge using the 3 Phase Charger", CommentType.String, true, false),

        new(37, "Carry out Coolant bleed procedure using FDRS - check coolant level after this has been completed",
            "Carry out Coolant bleed procedure using FDRS - check coolant level after this has been completed", CommentType.String, true, false),

        new(38, "Confirm the vehicle to vehicle charging will charge another vehicle (if applicable)",
            "Confirm the vehicle to vehicle charging will charge another vehicle (if applicable)", CommentType.String, true, false),

        new(39, "Check Functionality of Fast Charging Capability ATTACH EVIDENCE",
            "Check Functionality of Fast Charging Capability ATTACH EVIDENCE", CommentType.String, true, false),

        new(40, "Final approval for Vehicle Release",
            "Final approval for Vehicle Release", CommentType.String, true, false),

        new(41, "AVV Odometer Reading ENTER ODOMETER IN COMMENTS -->",
            "AVV Odometer Reading ENTER ODOMETER IN COMMENTS -->", CommentType.String, true, false),

        new(42, "User settings for GB", "User settings for GB", null, false, true),

        new(43, "Set Speedo to km/h", "Set Speedo to km/h", CommentType.String, true, false),
        new(44, "Set Temp to Celsius", "Set Temp to Celsius", CommentType.String, true, false),
        new(45, "Set 1 pedal drive", "Set 1 pedal drive", CommentType.String, true, false),
        new(46, "Alarm enabled on Key", "Alarm enabled on Key", CommentType.String, true, false),
        new(47, "Turn off road sign recognition", "Turn off road sign recognition", CommentType.String, true, false),
        new(48, "Auto hold-on", "Auto hold-on", CommentType.String, true, false),
        new(49, "Pre-Collision Assist – warning, braking", "Pre-Collision Assist – warning, braking", CommentType.String, true, false),
        new(50, "Set BLIS – on", "Set BLIS – on", CommentType.String, true, false),
        new(51, "Cross-Traffic Alert – on", "Cross-Traffic Alert – on", CommentType.String, true, false),
        new(52, "Reverse Brake Assist – on", "Reverse Brake Assist – on", CommentType.String, true, false),
        new(53, "Vehicle Connectivity = OFF", "Vehicle Connectivity = OFF", CommentType.String, true, false),
        };
    private static List<ListItemSeed> Station5QC_ListItems()
    => new()
    {
        new(1,  "Scans & Check CSP's to be carried out.", "Scans & Check CSP's to be carried out.", CommentType.String, true, false),
        new(2,  "Test Drive", "Test Drive", CommentType.String, true, false),
        new(3,  "Radio Settings", "Radio Settings", CommentType.String, true, false),
        new(4,  "Tyre Pressure - Check & Adjust to 40 PSI", "Tyre Pressure - Check & Adjust to 40 PSI", CommentType.String, true, false),
        new(5,  "Adjust Headlights", "Adjust Headlights", CommentType.String, true, false),
        new(6,  "Perform Inspection - See \"Quality Control\" Checklist", "Perform Inspection - See \"Quality Control\" Checklist", CommentType.String, true, false),
        new(7,  "Coolant", "Coolant", CommentType.String, true, false),
        new(8,  "Wheel Alignment", "Wheel Alignment", CommentType.String, true, false),
        new(9,  "Front Motor Number", "Front Motor Number", CommentType.String, true, false),
        new(10, "Rear Motor Number", "Rear Motor Number", CommentType.String, true, false),
    };
    private static List<ListItemSeed> Station4_ListItems()
    => new()
    {
        new(1,  "Install LHS Modules", "Install LHS Modules", CommentType.String, true, false),
        new(2,  "Install HVAC", "Install HVAC", CommentType.String, true, false),
        new(3,  "Test A/C Leaks", "Test A/C Leaks", CommentType.String, true, false),
        new(4,  "A/C Gas", "A/C Gas", CommentType.String, true, false),
        new(5,  "Install Dash", "Install Dash", CommentType.String, true, false),
        new(6,  "Install Floor Items", "Install Floor Items", CommentType.String, true, false),
        new(7,  "Install Carpet", "Install Carpet", CommentType.String, true, false),
        new(8,  "LHS & RHS B Pillar Installation", "LHS & RHS B Pillar Installation", CommentType.String, true, false),
        new(9,  "Install RHS And LHS A Pillars", "Install RHS And LHS A Pillars", CommentType.String, true, false),
        new(10, "Install Dash Trims", "Install Dash Trims", CommentType.String, true, false),

        new(11, "Install Floor Mats", "Install Floor Mats", CommentType.String, true, false),
        new(12, "Install Steering Column", "Install Steering Column", CommentType.String, true, false),
        new(13, "Install Steering Wheel", "Install Steering Wheel", CommentType.String, true, false),
        new(14, "Install Centre Console", "Install Centre Console", CommentType.String, true, false),
        new(15, "Install Seats", "Install Seats", CommentType.String, true, false),
        new(16, "Install LHS Door Card", "Install LHS Door Card", CommentType.String, true, false),
        new(17, "Install RHS Door Card", "Install RHS Door Card", CommentType.String, true, false),
        new(18, "Install Glovebox", "Install Glovebox", CommentType.String, true, false),
        new(19, "Lower Vehicle", "Lower Vehicle", CommentType.String, true, false),
        new(20, "Connect The Battery", "Connect The Battery", CommentType.String, true, false),

        new(21, "Install Frunk", "Install Frunk", CommentType.String, true, false),

        new(22, "QC Checklist", "QC Checklist", null, false, true),

        new(23, "Modules for Control units are correctly installed", "Modules for Control units are correctly installed", CommentType.String, true, false),
        new(24, "Perform push pull on all plugs and connectors", "Perform push pull on all plugs and connectors", CommentType.String, true, false),
        new(25, "Wiring not crushed", "Wiring not crushed", CommentType.String, true, false),
        new(26, "Inspect Relay positioning for any damage from Dash installation", "Inspect Relay positioning for any damage from Dash installation", CommentType.String, true, false),
        new(27, "The HVAC has been inspected to ensure it has the correct VIN Sticker before it is fitted to the vehicle.", "The HVAC has been inspected to ensure it has the correct VIN Sticker before it is fitted to the vehicle.", CommentType.String, true, false),
        new(28, "Pressure test of HVAC System has been performed prior to dash installation", "Pressure test of HVAC System has been performed prior to dash installation", CommentType.String, true, false),
        new(29, "All safety critical fasteners sighted as marked", "All safety critical fasteners sighted as marked", CommentType.String, true, false),
        new(30, "Ensure no damages on the Dash.", "Ensure no damages on the Dash.", CommentType.String, true, false),

        new(31, "Ensure correct fitment of trims.", "Ensure correct fitment of trims.", CommentType.String, true, false),
        new(32, "24. QUALITY ALERT - All Installed Steering Components Checked and Inspected - Tight and Paint Marked", "24. QUALITY ALERT - All Installed Steering Components Checked and Inspected - Tight and Paint Marked", CommentType.String, true, false),
        new(33, "QUALITY ALERT - Inspect edges of carpet to ensure neat and tidy", "QUALITY ALERT - Inspect edges of carpet to ensure neat and tidy", CommentType.String, true, false),
    };
    private static string Normalize(string value)
        => (value ?? string.Empty).Trim().ToUpperInvariant();
    private static List<ListItemSeed> Station3B_ListItems()
     => new()
     {
        new(1, "Disassemble the LHS Wheel Guard", "Disassemble the LHS Wheel Guard", CommentType.String, true, false),
        new(2, "Disassemble CCS1", "Disassemble CCS1", CommentType.String, true, false),
        new(3, "Prep CCS2 Wires", "Prep CCS2 Wires", CommentType.String, true, false),
        new(4, "Install CCS2 To Wires", "Install CCS2 To Wires", CommentType.String, true, false),
        new(5, "Install CCS2 To Vehicle", "Install CCS2 To Vehicle", CommentType.String, true, false),
        new(6, "Install LHS Guard", "Install LHS Guard", CommentType.String, true, false),

        // Separator
        new(7, "QC Checklist", null, null, false, true),

        new(8, "CCS2 Wiring has been inspected for expected standard",
            "CCS2 Wiring has been inspected for expected standard",
            CommentType.String, true, false),

        new(9, "Perform push pull on all plugs and connectors",
            "Perform push pull on all plugs and connectors",
            CommentType.String, true, false),
     };

    private static List<ListItemSeed> Station2_ListItems()
    => new()
    {
        new(1,  "Prep firewall", "Prep firewall", CommentType.String, true, false),
        new(2,  "Cut Firewall", "Cut Firewall", CommentType.String, true, false),
        new(3,  "Install The LHS Plate", "Install The LHS Plate", CommentType.String, true, false),
        new(4,  "Install Centre Plate", "Install Centre Plate", CommentType.String, true, false),
        new(5,  "Install RHS Plate", "Install RHS Plate", CommentType.String, true, false),
        new(6,  "Seal All Firewall Plates", "Seal All Firewall Plates", CommentType.String, true, false),
        new(7,  "Install Accelerator Pedal Bracket", "Install Accelerator Pedal Bracket", CommentType.String, true, false),
        new(8,  "Install Fresh Air Cover", "Install Fresh Air Cover", CommentType.String, true, false),
        new(9,  "A/C Drain Mod", "A/C Drain Mod", CommentType.String, true, false),
        new(10, "Install Insulation", "Install Insulation", CommentType.String, true, false),
        new(11, "Install Brake Booster", "Install Brake Booster", CommentType.String, true, false),
        new(12, "Install A/C Lines", "Install A/C Lines", CommentType.String, true, false),
        new(13, "Install Brake Bar - ADR 35/06", "Install Brake Bar - ADR 35/06", CommentType.String, true, false),
        new(14, "Install Steering - ADR 90/00", "Install Steering - ADR 90/00", CommentType.String, true, false),
        new(15, "Install Seat Belts - ADR 04/06", "Install Seat Belts - ADR 04/06", CommentType.String, true, false),
        new(16, "A/C Lines Engine", "A/C Lines Engine", CommentType.String, true, false),

        // QC SECTION
        new(17, "QC Checklist", "QC Checklist", null, false, true),

        new(18, "No Damages, no cuts of lines or looms", "No Damages, no cuts of lines or looms", null, false, false),
        new(19, "Correct sealing & neat", "Correct sealing & neat", null, false, false),
        new(20, "Insulation is done and to expected standard", "Insulation is done and to expected standard", null, false, false),
        new(21, "All fasteners are correctly tensioned and marked", "All fasteners are correctly tensioned and marked", null, false, false),
        new(22, "Brake bar booster pin installed", "Brake bar booster pin installed", null, false, false),
        new(23, "Perform functional check on brake", "Perform functional check on brake", null, false, false),
    };


    private sealed record ListItemSeed(
        int Position,
        string Name,
        string CommentPlaceholder,
        CommentType? CommentType,
        bool IsAttachmentRequired,
        bool IsSeparator
    );
}
