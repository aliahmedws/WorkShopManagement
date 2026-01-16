using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.Uow;
using WorkShopManagement.Bays;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.ListItems;
using WorkShopManagement.ModelCategories;
using WorkShopManagement.RadioOptions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WorkShopManagement.Seeder;

public partial class WorkShopManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ILogger<WorkShopManagementDataSeedContributor> _logger;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    private readonly IUnitOfWorkManager _uowManager;
    private readonly IGuidGenerator _guid;
    private readonly IConfiguration _configuration;

    private readonly IRepository<Bay, Guid> _bayRepo;
    private readonly IRepository<ModelCategory, Guid> _categoryRepo;
    private readonly IRepository<CarModel, Guid> _carModelRepo;
    private readonly IRepository<CheckList, Guid> _checkListRepo;
    private readonly IRepository<ListItem, Guid> _listItemRepo;
    private readonly IRepository<RadioOption, Guid> _radioOptionRepo;

    // Batch sizes (tune if needed)
    private const int ListItemBatchSize = 2000;
    private const int RadioOptionBatchSize = 5000;
    private const int TransactionBatchSize = 10000;

    // Checklist names
    private const string Station0Name = "Station 0 - Receiving Compliance Audit";
    private const string Station1AName = "Station 1A";
    private const string Station1BName = "Station 1B";
    private const string Station2Name = "Station 2";
    private const string Station3AName = "Station 3A";
    private const string Station3BName = "Station 3B";
    private const string Station4Name = "Station 4";
    private const string Station5QCName = "Station 5 (QC)";
    private const string WheelAlignmentName = "Wheel Alignment";
    private const string QualityControlName = "Quality Control";
    private const string QualityReleaseName = "Quality Release";
    private const string DashRemanufactureName = "Dash Remanufacture";
    private const string HVACName = "HVAC";
    private const string CentreConsoleName = "Centre Console";
    private const string SeatsConversionName = "Seats Conversion";
    private const string LeatherSeatKitName = "Leather Seat Kit";
    private const string SubAssemblyElectricalName = "Sub Assembly Electrical";
    private const string InvoiceName = "Invoice";
    private const string PreDeliveryInspectionName = "Pre-Delivery Inspection";
    private const string ProcurementName = "Procurement";
    private const string AVVPackageName = "AVV Package";
    private const string QualityName = "Quality";

    public WorkShopManagementDataSeedContributor(
        ILogger<WorkShopManagementDataSeedContributor> logger,
        IAsyncQueryableExecuter asyncExecuter,
        IUnitOfWorkManager uowManager,
        IGuidGenerator guid,
        IConfiguration configuration,
        IRepository<Bay, Guid> bayRepo,
        IRepository<ModelCategory, Guid> categoryRepo,
        IRepository<CarModel, Guid> carModelRepo,
        IRepository<CheckList, Guid> checkListRepo,
        IRepository<ListItem, Guid> listItemRepo,
        IRepository<RadioOption, Guid> radioOptionRepo)
    {
        _logger = logger;
        _asyncExecuter = asyncExecuter;
        _uowManager = uowManager;
        _guid = guid;
        _configuration = configuration;

        _bayRepo = bayRepo;
        _categoryRepo = categoryRepo;
        _carModelRepo = carModelRepo;
        _checkListRepo = checkListRepo;
        _listItemRepo = listItemRepo;
        _radioOptionRepo = radioOptionRepo;
    }

    private async Task<T> TimedAsync<T>(string stage, Func<Task<T>> action)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("[SEED][START] {Stage}", stage);

        try
        {
            var result = await action();
            sw.Stop();
            _logger.LogInformation("[SEED][END] {Stage} in {ElapsedMs} ms", stage, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "[SEED][FAIL] {Stage} after {ElapsedMs} ms", stage, sw.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task TimedAsync(string stage, Func<Task> action)
    {
        await TimedAsync(stage, async () => { await action(); return true; });
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        try
        {
            _logger.LogInformation("WorkShopManagementDataSeedContributor started.");

            var totalInserted = 0;

            var rootUrl = _configuration["OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl"];
            if (string.IsNullOrWhiteSpace(rootUrl))
                throw new Exception("Missing configuration: OpenIddict:Applications:WorkShopManagement_Swagger:RootUrl");

            var modelCategoriesPath = Path.Combine(rootUrl, "images", "ModelCategories");
            var carModelsPath = Path.Combine(modelCategoriesPath, "CarModels");

            // ----------------------------
            // TRANSACTION 1: Basic Setup (Bays, ModelCategories, CarModels)
            // These need to be committed together before CheckLists can reference them
            // ----------------------------
            using (var uow = _uowManager.Begin(requiresNew: true, isTransactional: true))
            {
                // Bays
                totalInserted += await TimedAsync("Bays", SeedBaysAsync);

                // Model Categories
                if (!await AnyAsync(_categoryRepo))
                {
                    totalInserted += await TimedAsync("ModelCategories", () => SeedModelCategoriesAsync(modelCategoriesPath));
                    // Must save changes here so CarModels can see them
                    await TimedAsync("ModelCategories: SaveChanges", () => _uowManager.Current!.SaveChangesAsync());
                }
                else
                {
                    _logger.LogInformation("Seed skip: ModelCategories already exist.");
                }

                // Car Models
                if (!await AnyAsync(_carModelRepo))
                {
                    totalInserted += await TimedAsync("CarModels", () => SeedCarModelsAsync(carModelsPath));
                    await TimedAsync("CarModels: SaveChanges", () => _uowManager.Current!.SaveChangesAsync());
                }
                else
                {
                    _logger.LogInformation("Seed skip: CarModels already exist.");
                }

                await TimedAsync("Transaction 1: CompleteAsync", () => uow.CompleteAsync());
            }

            // Check if we should continue
            if (await AnyAsync(_checkListRepo))
            {
                _logger.LogInformation("Seed skip: CheckLists already exist -> skipping CheckLists/ListItems/RadioOptions seeding.");
                _logger.LogInformation("WorkShopManagementDataSeedContributor done. Inserted {Count} records.", totalInserted);
                return;
            }

            // ----------------------------
            // TRANSACTION 2: CheckLists
            // ----------------------------
            using (var uow = _uowManager.Begin(requiresNew: true, isTransactional: true))
            {
                totalInserted += await TimedAsync("CheckLists", SeedCheckListsAsync);
                await TimedAsync("Transaction 2: CompleteAsync", () => uow.CompleteAsync());
            }

            // ----------------------------
            // TRANSACTION 3: ListItems (in batches)
            // ----------------------------
            totalInserted += await TimedAsync("ListItems (Batched)", SeedListItemsBatched);

            // ----------------------------
            // TRANSACTION 4: RadioOptions (in batches)
            // ----------------------------
            totalInserted += await TimedAsync("RadioOptions (Batched)", SeedRadioOptionsBatched);

            _logger.LogInformation("WorkShopManagementDataSeedContributor done. Inserted {Count} records.", totalInserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seeding failed");
            throw;
        }
    }

    private async Task<int> SeedListItemsBatched()
    {
        var clQ = await _checkListRepo.GetQueryableAsync();
        var checkLists = await clQ
            .AsNoTracking()
            .Select(x => new { x.Id, x.CarModelId, x.Name })
            .ToListAsync();

        if (checkLists.Count == 0)
        {
            _logger.LogInformation("ListItems: no CheckLists found. Skipping.");
            return 0;
        }

        var checkListByCarAndName = checkLists
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToDictionary(
                x => $"{x.CarModelId:N}|{Normalize(x.Name!)}",
                x => x.Id,
                StringComparer.OrdinalIgnoreCase
            );

        var carModelIds = checkLists.Select(x => x.CarModelId).Distinct().ToList();

        // Build all items first
        var toInsert = new List<ListItem>(capacity: 20000);

        // Pre-sort seed lists
        var station0 = GetStation0_ListItems().OrderBy(x => x.Position).ToList();
        var station1a = GetStation1A_ListItems().OrderBy(x => x.Position).ToList();
        var station1b = GetStation1B_ListItems().OrderBy(x => x.Position).ToList();
        var station2 = Station2_ListItems().OrderBy(x => x.Position).ToList();
        var station3a = Station3A_ListItems().OrderBy(x => x.Position).ToList();
        var station3b = Station3B_ListItems().OrderBy(x => x.Position).ToList();
        var station4 = Station4_ListItems().OrderBy(x => x.Position).ToList();
        var station5qc = Station5QC_ListItems().OrderBy(x => x.Position).ToList();
        var wheel = WheelAlignment_ListItems().OrderBy(x => x.Position).ToList();
        var qc = QualityControl_ListItems().OrderBy(x => x.Position).ToList();
        var qr = QualityRelease_ListItems().OrderBy(x => x.Position).ToList();
        var dash = DashRemanufacture_ListItems().OrderBy(x => x.Position).ToList();
        var hvac = HVAC_ListItems().OrderBy(x => x.Position).ToList();
        var console = CentreConsole_ListItems().OrderBy(x => x.Position).ToList();
        var seats = SeatsConversion_ListItems().OrderBy(x => x.Position).ToList();
        var leather = LeatherSeatKit_ListItems().OrderBy(x => x.Position).ToList();
        var subElec = SubAssemblyElectrical_ListItems().OrderBy(x => x.Position).ToList();
        var invoice = Invoice_ListItems().OrderBy(x => x.Position).ToList();
        var pdi = GetPreDeliveryInspection_ListItems().OrderBy(x => x.Position).ToList();
        var procurement = Procurement_ListItems().OrderBy(x => x.Position).ToList();
        var avv = AVVPackage_ListItems().OrderBy(x => x.Position).ToList();
        var quality = Quality_ListItems().OrderBy(x => x.Position).ToList();

        foreach (var carModelId in carModelIds)
        {
            AddListItemsForChecklist(carModelId, Station0Name, station0);
            AddListItemsForChecklist(carModelId, Station1AName, station1a);
            AddListItemsForChecklist(carModelId, Station1BName, station1b);
            AddListItemsForChecklist(carModelId, Station2Name, station2);
            AddListItemsForChecklist(carModelId, Station3AName, station3a);
            AddListItemsForChecklist(carModelId, Station3BName, station3b);
            AddListItemsForChecklist(carModelId, Station4Name, station4);
            AddListItemsForChecklist(carModelId, Station5QCName, station5qc);
            AddListItemsForChecklist(carModelId, WheelAlignmentName, wheel);
            AddListItemsForChecklist(carModelId, QualityControlName, qc);
            AddListItemsForChecklist(carModelId, QualityReleaseName, qr);
            AddListItemsForChecklist(carModelId, DashRemanufactureName, dash);
            AddListItemsForChecklist(carModelId, HVACName, hvac);
            AddListItemsForChecklist(carModelId, CentreConsoleName, console);
            AddListItemsForChecklist(carModelId, SeatsConversionName, seats);
            AddListItemsForChecklist(carModelId, LeatherSeatKitName, leather);
            AddListItemsForChecklist(carModelId, SubAssemblyElectricalName, subElec);
            AddListItemsForChecklist(carModelId, InvoiceName, invoice);
            AddListItemsForChecklist(carModelId, PreDeliveryInspectionName, pdi);
            AddListItemsForChecklist(carModelId, ProcurementName, procurement);
            AddListItemsForChecklist(carModelId, AVVPackageName, avv);
            AddListItemsForChecklist(carModelId, QualityName, quality);
        }

        void AddListItemsForChecklist(Guid carModelId, string checkListName, IReadOnlyList<ListItemSeed> seeds)
        {
            var key = $"{carModelId:N}|{Normalize(checkListName)}";
            if (!checkListByCarAndName.TryGetValue(key, out var checkListId))
            {
                _logger.LogWarning(
                    "ListItems: checklist missing. CarModelId={CarModelId}, CheckList={CheckList}",
                    carModelId, checkListName);
                return;
            }

            foreach (var s in seeds)
            {
                var isSeparator = s.IsSeparator;

                toInsert.Add(new ListItem(
                    id: _guid.Create(),
                    checkListId: checkListId,
                    position: s.Position,
                    name: s.Name,
                    commentPlaceholder: isSeparator ? null : s.CommentPlaceholder,
                    commentType: isSeparator ? null : s.CommentType,
                    isAttachmentRequired: isSeparator ? false : s.IsAttachmentRequired,
                    isSeparator: isSeparator
                ));
            }
        }

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("ListItems: nothing to insert.");
            return 0;
        }

        // Insert in transaction batches
        var inserted = 0;
        var batchNo = 0;

        foreach (var batch in Batch(toInsert, TransactionBatchSize))
        {
            batchNo++;
            using (var uow = _uowManager.Begin(requiresNew: true, isTransactional: true))
            {
                var sw = Stopwatch.StartNew();

                // Insert in smaller chunks within the transaction
                foreach (var chunk in Batch(batch, ListItemBatchSize))
                {
                    await _listItemRepo.InsertManyAsync(chunk, autoSave: false);
                }

                await uow.CompleteAsync();
                inserted += batch.Count;

                sw.Stop();
                _logger.LogInformation(
                    "ListItems: transaction batch {BatchNo} committed {BatchCount} in {ElapsedMs} ms",
                    batchNo, batch.Count, sw.ElapsedMilliseconds);
            }
        }

        _logger.LogInformation("ListItems: inserted total {Count}.", inserted);
        return inserted;
    }

    private async Task<int> SeedRadioOptionsBatched()
    {
        var liQ = await _listItemRepo.GetQueryableAsync();
        var listItems = await liQ
            .AsNoTracking()
            .Select(x => new { x.Id, x.CheckListId, x.Position, x.IsSeparator })
            .ToListAsync();

        if (listItems.Count == 0)
        {
            _logger.LogInformation("RadioOptions: no ListItems found. Skipping.");
            return 0;
        }

        var clQ = await _checkListRepo.GetQueryableAsync();
        var checkLists = await clQ
            .AsNoTracking()
            .Select(x => new { x.Id, x.CarModelId, x.Name })
            .ToListAsync();

        var checkListByCarAndName = checkLists
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToDictionary(
                x => $"{x.CarModelId:N}|{Normalize(x.Name!)}",
                x => x.Id,
                StringComparer.OrdinalIgnoreCase
            );

        var listItemByCheckListAndPos = listItems
            .Where(x => x.IsSeparator != true)
            .GroupBy(x => new { x.CheckListId, x.Position })
            .ToDictionary(
                g => $"{g.Key.CheckListId:N}|{g.Key.Position}",
                g => g.First().Id,
                StringComparer.OrdinalIgnoreCase
            );

        var carQ = await _carModelRepo.GetQueryableAsync();
        var carModels = await carQ
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync();

        // Build all radio options
        var toInsert = new List<RadioOption>(capacity: 50000);

        foreach (var carModelId in carModels)
        {
            AddRadioSeeds(carModelId, Station0Name, GetStation0_RadioSeeds());
            AddRadioSeeds(carModelId, Station1AName, GetStation1A_RadioSeeds());
            AddRadioSeeds(carModelId, Station1BName, GetStation1B_RadioSeeds());
            AddRadioSeeds(carModelId, Station2Name, GetStation2_RadioSeeds());
            AddRadioSeeds(carModelId, Station3AName, GetStation3A_RadioSeeds());
            AddRadioSeeds(carModelId, Station3BName, GetStation3B_RadioSeeds());
            AddRadioSeeds(carModelId, Station4Name, GetStation4_RadioSeeds());
            AddRadioSeeds(carModelId, Station5QCName, GetStation5QC_RadioSeeds());
            AddRadioSeeds(carModelId, WheelAlignmentName, GetWheelAlignment_RadioSeeds());
            AddRadioSeeds(carModelId, QualityControlName, GetQualityControl_RadioSeeds());
            AddRadioSeeds(carModelId, QualityReleaseName, GetQualityRelease_RadioSeeds());
            AddRadioSeeds(carModelId, DashRemanufactureName, GetDashRemanufacture_RadioSeeds());
            AddRadioSeeds(carModelId, HVACName, GetHVAC_RadioSeeds());
            AddRadioSeeds(carModelId, CentreConsoleName, GetCentreConsole_RadioSeeds());
            AddRadioSeeds(carModelId, SeatsConversionName, GetSeatsConversion_RadioSeeds());
            AddRadioSeeds(carModelId, LeatherSeatKitName, GetLeatherSeatKit_RadioSeeds());
            AddRadioSeeds(carModelId, SubAssemblyElectricalName, GetSubAssemblyElectrical_RadioSeeds());
            AddRadioSeeds(carModelId, QualityName, GetQuality_RadioSeeds());
            AddRadioSeeds(carModelId, AVVPackageName, GetAVVPackage_RadioSeeds());
            AddRadioSeeds(carModelId, ProcurementName, GetProcurement_RadioSeeds());
            AddRadioSeeds(carModelId, InvoiceName, GetInvoice_RadioSeeds());
            AddRadioSeeds(carModelId, PreDeliveryInspectionName, GetPreDeliveryInspection_RadioSeeds());
        }

        void AddRadioSeeds(Guid carModelId, string checkListName, IReadOnlyList<RadioSeed> seeds)
        {
            var clKey = $"{carModelId:N}|{Normalize(checkListName)}";
            if (!checkListByCarAndName.TryGetValue(clKey, out var checkListId))
            {
                _logger.LogWarning(
                    "RadioOptions: checklist missing. CarModelId={CarModelId}, CheckList={CheckList}",
                    carModelId, checkListName);
                return;
            }

            foreach (var seed in seeds)
            {
                var liKey = $"{checkListId:N}|{seed.ListItemPosition}";
                if (!listItemByCheckListAndPos.TryGetValue(liKey, out var listItemId))
                    continue;

                foreach (var opt in seed.Options.Select(x => (x ?? string.Empty).Trim()).Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    toInsert.Add(new RadioOption(_guid.Create(), listItemId, opt));
                }
            }
        }

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("RadioOptions: nothing to insert.");
            return 0;
        }

        // Insert in transaction batches
        var inserted = 0;
        var batchNo = 0;

        foreach (var batch in Batch(toInsert, TransactionBatchSize * 2)) // Larger batches for RadioOptions
        {
            batchNo++;
            using (var uow = _uowManager.Begin(requiresNew: true, isTransactional: true))
            {
                var sw = Stopwatch.StartNew();

                // Insert in chunks within the transaction
                foreach (var chunk in Batch(batch, RadioOptionBatchSize))
                {
                    await _radioOptionRepo.InsertManyAsync(chunk, autoSave: false);
                }

                await uow.CompleteAsync();
                inserted += batch.Count;

                sw.Stop();
                _logger.LogInformation(
                    "RadioOptions: transaction batch {BatchNo} committed {BatchCount} in {ElapsedMs} ms",
                    batchNo, batch.Count, sw.ElapsedMilliseconds);
            }
        }

        _logger.LogInformation("RadioOptions: inserted total {Count}.", inserted);
        return inserted;
    }

    private async Task<bool> AnyAsync<TEntity>(IRepository<TEntity, Guid> repo)
        where TEntity : class, IEntity<Guid>
    {
        var q = await repo.GetQueryableAsync();
        return await _asyncExecuter.AnyAsync(q);
    }

    // Bays
    private async Task<int> SeedBaysAsync()
    {
        var desired = Enumerable.Range(1, 20)
            .Select(i => new Bay(_guid.Create(), $"Bay {i}", true))
            .ToList();

        var q = await _bayRepo.GetQueryableAsync();
        var existingNames = await q
            .Select(x => x.Name)
            .Where(x => x != null && x != "")
            .ToListAsync();

        var existing = existingNames.Select(Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toInsert = desired
            .Where(x => !existing.Contains(Normalize(x.Name)))
            .ToList();

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("Bays: nothing to insert.");
            return 0;
        }

        await _bayRepo.InsertManyAsync(toInsert, autoSave: false);
        _logger.LogInformation("Bays: inserted {Count}.", toInsert.Count);
        return toInsert.Count;
    }

    // Model Categories
    private async Task<int> SeedModelCategoriesAsync(string modelCategoriesPath)
    {
        var seeds = new List<(string Name, string FileName)>
        {
            ("FORD 150", "Ford-1500.png"),
            ("FORD SUPER DUTY", "Ford-Super-duty.png"),
            ("RAM 1500", "RAM-1500.png"),
            ("RAM HEAVY DUTY", "Ram-heavy-duty.png"),
        };

        var q = await _categoryRepo.GetQueryableAsync();
        var existingNames = await q
            .Select(x => x.Name)
            .Where(x => x != null && x != "")
            .ToListAsync();

        var existing = existingNames.Select(Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toInsert = new List<ModelCategory>();

        foreach (var (name, fileName) in seeds)
        {
            if (existing.Contains(Normalize(name)))
                continue;

            var filePath = Path.Combine(modelCategoriesPath, fileName);

            var attachment = new FileAttachment(
                name: fileName,
                blobName: fileName,
                path: filePath
            );

            toInsert.Add(new ModelCategory(_guid.Create(), name, attachment));
            existing.Add(Normalize(name));
        }

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("ModelCategories: nothing to insert.");
            return 0;
        }

        await _categoryRepo.InsertManyAsync(toInsert, autoSave: false);
        _logger.LogInformation("ModelCategories: inserted {Count}.", toInsert.Count);
        return toInsert.Count;
    }

    // Car Models
    private async Task<int> SeedCarModelsAsync(string carModelsPath)
    {
        var seeds = GetCarModelSeeds();

        var catQ = await _categoryRepo.GetQueryableAsync();
        var categories = await catQ.Select(x => new { x.Id, x.Name }).ToListAsync();

        var categoryByName = categories
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .ToDictionary(x => Normalize(x.Name!), x => x.Id);

        var carQ = await _carModelRepo.GetQueryableAsync();
        var existingKeys = (await carQ
                .Select(x => new { x.ModelCategoryId, x.Name })
                .Where(x => x.Name != null && x.Name != "")
                .ToListAsync())
            .Select(x => $"{x.ModelCategoryId:N}|{Normalize(x.Name!)}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toInsert = new List<CarModel>();

        foreach (var (categoryName, name, fileName) in seeds)
        {
            var categoryKey = Normalize(categoryName);

            if (!categoryByName.TryGetValue(categoryKey, out var categoryId))
            {
                _logger.LogWarning("Missing ModelCategory '{CategoryName}'. Skipping CarModel '{CarModelName}'.", categoryName, name);
                continue;
            }

            var key = $"{categoryId:N}|{Normalize(name)}";
            if (existingKeys.Contains(key))
                continue;

            var filePath = Path.Combine(carModelsPath, fileName);

            var attachment = new FileAttachment(
                name: fileName,
                blobName: fileName,
                path: filePath
            );

            toInsert.Add(new CarModel(
                _guid.Create(),
                categoryId,
                name,
                attachment
            ));

            existingKeys.Add(key);
        }

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("CarModels: nothing to insert.");
            return 0;
        }

        await _carModelRepo.InsertManyAsync(toInsert, autoSave: false);
        _logger.LogInformation("CarModels: inserted {Count}.", toInsert.Count);
        return toInsert.Count;
    }

    // CheckLists
    private async Task<int> SeedCheckListsAsync()
    {
        var defaultCheckLists = GetDefaultCheckLists();

        var carQ = await _carModelRepo.GetQueryableAsync();
        var carModels = await carQ.Select(x => new { x.Id, x.Name }).ToListAsync();

        if (carModels.Count == 0)
        {
            _logger.LogInformation("CheckLists: no CarModels found. Skipping.");
            return 0;
        }

        var clQ = await _checkListRepo.GetQueryableAsync();
        var existingKeys = (await clQ
                .Select(x => new { x.CarModelId, x.Name })
                .Where(x => x.Name != null && x.Name != "")
                .ToListAsync())
            .Select(x => $"{x.CarModelId:N}|{Normalize(x.Name!)}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toInsert = new List<CheckList>();

        foreach (var carModel in carModels)
        {
            foreach (var seed in defaultCheckLists.OrderBy(x => x.Position))
            {
                var key = $"{carModel.Id:N}|{Normalize(seed.Name)}";
                if (existingKeys.Contains(key))
                    continue;

                toInsert.Add(new CheckList(
                    id: _guid.Create(),
                    name: seed.Name,
                    position: seed.Position,
                    carModelId: carModel.Id
                ));

                existingKeys.Add(key);
            }
        }

        if (toInsert.Count == 0)
        {
            _logger.LogInformation("CheckLists: nothing to insert.");
            return 0;
        }

        await _checkListRepo.InsertManyAsync(toInsert, autoSave: false);
        _logger.LogInformation("CheckLists: inserted {Count}.", toInsert.Count);
        return toInsert.Count;
    }

    // Helpers
    private static string Normalize(string value)
        => (value ?? string.Empty).Trim().ToUpperInvariant();

    private static IEnumerable<List<T>> Batch<T>(List<T> source, int size)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

        for (int i = 0; i < source.Count; i += size)
        {
            var count = Math.Min(size, source.Count - i);
            yield return source.GetRange(i, count);
        }
    }



    private sealed record CheckListSeed(int Position, string Name, bool IsEnabled);

    private sealed record ListItemSeed(
        int Position,
        string Name,
        string CommentPlaceholder,
        CommentType? CommentType,
        bool IsAttachmentRequired,
        bool IsSeparator
    );

    private sealed record RadioSeed(int ListItemPosition, params string[] Options);

    // ---- Checklists (FULL) ----
    private static IReadOnlyList<CheckListSeed> GetDefaultCheckLists()
        => new List<CheckListSeed>
        {
            new(1,  Station0Name, true),
            new(2,  Station1AName, true),
            new(3,  Station1BName, true),
            new(4,  Station2Name, true),
            new(5,  Station3AName, true),
            new(6,  Station3BName, true),
            new(7,  Station4Name, true),
            new(8,  Station5QCName, true),
            new(9,  WheelAlignmentName, true),
            new(10, QualityControlName, true),
            new(11, QualityReleaseName, true),
            new(12, DashRemanufactureName, true),
            new(13, HVACName, true),
            new(14, CentreConsoleName, true),
            new(15, SeatsConversionName, true),
            new(16, LeatherSeatKitName, true),
            new(17, SubAssemblyElectricalName, true),
            new(18, InvoiceName, true),
            new(19, ProcurementName, true),
            new(20, AVVPackageName, true),
            new(21, PreDeliveryInspectionName, true),
            new(22, QualityName, true),
        };

    // ---- Car models (FULL - as provided) ----
    private static IReadOnlyList<(string CategoryName, string Name, string FileName)> GetCarModelSeeds()
        => new List<(string, string, string)>
        {
            // FORD 150
            ("FORD 150", "Raptor", "ford-raptor.jpg"),
            ("FORD 150", "Raptor R", "ford-raptor-r.jpg"),

            // FORD SUPER DUTY
            ("FORD SUPER DUTY", "Ford Superduty Pickup", "ford-super-duty-pickup.jpg"),
            ("FORD SUPER DUTY", "Ford Super Duty Cab Chassis", "ford-super-duty-cab-chassis.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® XL", "super-duty-f-250-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® XL", "super-duty-f-250-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® XL", "super-duty-f-450-xl.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® XLT", "super-duty-f-250-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® XLT", "super-duty-f-250-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® XLT", "super-duty-f-450-xlt.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® LARIAT", "super-duty-f-250-lariat.jpg"),
            ("FORD SUPER DUTY", "2025 Super Duty® F-350® LARIAT", "super-duty-f-250-lariat.jpg"),
            ("FORD SUPER DUTY", "2025 Super Duty® F-450® LARIAT", "super-duty-f-450-lariat.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® King Ranch", "super-duty-f-250-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® King Ranch", "super-duty-f-250-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® King Ranch", "super-duty-f-450-king-ranch.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-250® Platinum", "super-duty-f-250-platinum.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-350® Platinum", "super-duty-f-250-platinum.jpg"),
            ("FORD SUPER DUTY", "Super Duty® F-450® Platinum", "super-duty-f-450-king-ranch.jpg"),

            ("FORD SUPER DUTY", "Chassis Cab F-350® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-350® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-350® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-450® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® XLT", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-550® LARIAT®", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-600® XL", "chassis-cab-f-350-xl.jpg"),
            ("FORD SUPER DUTY", "Chassis Cab F-600® XLT", "chassis-cab-f-350-xl.jpg"),

            // RAM 1500
            ("RAM 1500", "RAM 1500 RHO", "ram-1500-rho.jpg"),
            ("RAM 1500", "RAM 1500 Tungsten", "ram-1500-tungsten.jpg"),

            // RAM HEAVY DUTY
            ("RAM HEAVY DUTY", "RAM Heavy Duty Pickup", "ram-heavy-duty-pickup.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 REBEL", "ram-heavy-duty-2500-rebel.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 LIMITED LONGHORN", "ram-heavy-duty-2500-limited-longhorn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 2500 LIMITED", "ram-heavy-duty-2500-limited.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LIMITED", "ram-heavy-duty-3500-limited.png"),
            ("RAM HEAVY DUTY", "RAM heavy Duty 3500 LARAMIE", "ram-heavy-duty-3500-laramie.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 TRADESMAN", "ram-heavy-duty-35000tradesman.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LARAMIE", "ram-heavy-duty-3500-laramie.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 LIMITED LONGHORN", "ram-heavy-duty-3500-limited-longhorn.png"),

            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 Cab Chassis TRADESMAN", "ram-heavy-duty-3500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 3500 Cab Chassis BIG HORN", "ram-heavy-duty-3500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 4500 Cab Chassis TRADESMAN", "ram-heavy-duty-4500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 4500 Cab Chassis BIG HORN", "ram-heavy-duty-4500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 5500 Cab Chassis TRADESMAN", "ram-heavy-duty-5500-cab-chassis-big-horn.png"),
            ("RAM HEAVY DUTY", "RAM Heavy Duty 5500 Cab Chassis BIG HORN", "ram-heavy-duty-5500-cab-chassis-tradesman.png"),
        };



    private static IReadOnlyList<ListItemSeed> GetStation0_ListItems()
           => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> GetStation1A_ListItems()
    => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> GetStation1B_ListItems()
   => new List<ListItemSeed>
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
    private static IReadOnlyList<ListItemSeed> Station3A_ListItems()
   => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> Invoice_ListItems()
     => new List<ListItemSeed>
     {
            new(1,  "Dealer Invoice Number","Dealer Invoice Number",CommentType.String, true, false),
            new(2,  "Dealer Invoice Paid","Dealer Invoice Paid",CommentType.Date, true, false),
            new(3,  "Customer Invoice Number","Customer Invoice Number",CommentType.String, true, false),
            new(4,  "Customer Invoice Paid","Customer Invoice Paid",CommentType.Date, true, false),
     };
    private static IReadOnlyList<ListItemSeed> GetPreDeliveryInspection_ListItems()
        => new List<ListItemSeed>
        {
        new (1,  "PRE-DELIVERY INSPECTION", "PRE-DELIVERY INSPECTION", null, false, true),
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

    private static IReadOnlyList<ListItemSeed> Procurement_ListItems()
      => new List<ListItemSeed>
      {
            new(1,  "Remanufacture invoice issued to correct supplier ENTER INVOICE NUMBER IN COMMENTS -->","Enter invoice number",CommentType.String, true, false),
            new(2,  "Invoice emailed","Invoice emailed",CommentType.String, true, false),
      };

    private static IReadOnlyList<ListItemSeed> AVVPackage_ListItems()
   => new List<ListItemSeed>
   {
        new(1,  "AVV Package Review","AVV Package Review",CommentType.String, true, false),
   };

    private static IReadOnlyList<ListItemSeed> Quality_ListItems()
     => new List<ListItemSeed>
     {
        new(1,  "LIN Module Rework","LIN Module Rework",CommentType.String, true, false),
     };

    private static IReadOnlyList<ListItemSeed> SubAssemblyElectrical_ListItems()
       => new List<ListItemSeed>
       {
            new(1,  "Dash Loom (IF GB install GB wiring, splice the wiring into the instrument cluster CAN signal as per drawing)","Dash Loom (IF GB install GB wiring, splice the wiring into the instrument cluster CAN signal as per drawing)",CommentType.String, true, false),
       };
    private static IReadOnlyList<ListItemSeed> LeatherSeatKit_ListItems()
       => new List<ListItemSeed>
       {
            new(1,  "If XLT (else mark N/A), confirm leather seat kit has been installed","If XLT (else mark N/A), confirm leather seat kit has been installed",CommentType.String, true, false),
       };

    private static IReadOnlyList<ListItemSeed> SeatsConversion_ListItems()
      => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> CentreConsole_ListItems()
     => new List<ListItemSeed>
     {
        new(1,  "Disassemble centre console","Disassemble centre console",CommentType.String, true, false),
        new(2,  "Convert top housing and gear selector","Convert top housing and gear selector",CommentType.String, true, false),
        new(3,  "Assemble centre console","Assemble centre console",CommentType.String, true, false),
        new(4,  "(IF NOT UN) plug AC110V socket and tape it on the loom behind the centre console","(IF NOT UN) plug AC110V socket and tape it on the loom behind the centre console",CommentType.String, true, false),
        new(5,  "(IF NOT UN) install AC110V blank (IF UN) leave AC110V socket on centre console","(IF NOT UN) install AC110V blank (IF UN) leave AC110V socket on centre console",CommentType.String, true, false),
     };

    private static IReadOnlyList<ListItemSeed> HVAC_ListItems()
    => new List<ListItemSeed>
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
    private static IReadOnlyList<ListItemSeed> DashRemanufacture_ListItems()
    => new List<ListItemSeed>
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
    private static IReadOnlyList<ListItemSeed> QualityRelease_ListItems()
    => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> WheelAlignment_ListItems()
     => new List<ListItemSeed>
     {
        new(1,  "Mark N/A until alignment is completed. ENTER DATE IN COMMENTS -->","Enter date",CommentType.Date, true, false),
     };
    private static IReadOnlyList<ListItemSeed> QualityControl_ListItems()
        => new List<ListItemSeed>
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
    private static IReadOnlyList<ListItemSeed> Station5QC_ListItems()
    => new List<ListItemSeed>
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
    private static IReadOnlyList<ListItemSeed> Station4_ListItems()
    => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> Station3B_ListItems()
     => new List<ListItemSeed>
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

    private static IReadOnlyList<ListItemSeed> Station2_ListItems()
    => new List<ListItemSeed>
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

    private static IReadOnlyList<RadioSeed> GetStation0_RadioSeeds()
    => new List<RadioSeed>
    {
            new(1, "Yes", "No", "N/A"),
            new(3, "Completed", "Deferred"),
            new(4, "Yes", "No", "N/A"),
            new(6, "Yes", "No", "N/A"),
            new(7, "Yes", "No", "N/A"),
            new(9, "2", "1", "0"),
            new(10, "No Lock Nuts", "Missing Key", "Lock Nuts with Key"),
            new(11, "Present", "Missing"),
            new(12, "Present", "Missing"),
            new(13, "Present", "Missing"),
            new(14, "Present", "Missing"),
            new(16, "Yes", "No", "N/A"),
            new(17, "Yes", "No", "N/A"),
            new(18, "Yes", "No", "N/A"),
            new(20, "Yes", "No", "N/A"),
            new(21, "Yes", "No", "N/A"),
            new(22, "Yes", "No", "N/A"),
            new(23, "Yes", "No", "N/A"),
            new(24, "Yes", "No", "N/A"),
            new(25, "Yes", "No", "N/A"),
            new(26, "Yes", "No", "N/A"),
            new(27, "Yes", "No", "N/A"),
            new(29, "Yes", "No", "N/A"),
            new(30, "Yes", "No", "N/A"),
            new(32, "SAE HL A I5 P P2 22TK", "Other (Write In Comments)"),
            new(33, "SAE HL A I5 P P2 22TK", "Other (Write In Comments)"),
            new(34, "SAE AIP2RST 22TK", "Other (Write In Comments)"),
            new(35, "SAE AIP2RST 22TK", "Other (Write In Comments)"),
            new(36, "SAE U3 (2)G 17TK","SAE U3 (2)G 15TK", "Other (Write In Comments)"),
            new(37, "SAE E220","N/A", "Other (Write In Comments)"),
            new(38, "SAE E220","N/A", "Other (Write In Comments)"),
            new(40, "DOT-22 M50L1 AS1", "Other (Write In Comments)"),
            new(41, "E11 43R-000257", "Other (Write In Comments)"),
            new(42, "E11 43R-000257", "Other (Write In Comments)"),
            new(43, "DOT-467 M40T3 AS3", "Other (Write In Comments)"),
            new(44, "DOT-467 M40T3 AS3", "Other (Write In Comments)"),
            new(45, "E11 43R-000147", "Other (Write In Comments)"),
            new(46, "N/A", "E2 43R 0115131", "Other (Write In Comments)"),
            new(47, "N/A", "E2 43R 0115131", "Other (Write In Comments)"),
            new(48, "E11 026533", "Other (Write In Comments)"),
            new(50, "Michelin", "General", "Hankook", "Goodyear", "Other (Write In Comments)"),
            new(52, "No", "Yes"),
            new(53, "Pass", "Fail", "N/A"),
            new(54, "Pass", "Fail", "N/A"),
            new(55, "Complete"),
            new(56, "No", "Yes"),
            new(57, "Yes", "No", "N/A")
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
       

        // PASS / FAIL
        new(37, "PASS", "FAIL"),
        new(38, "PASS", "FAIL"),
        new(39, "PASS", "FAIL"),
        new(40, "PASS", "FAIL"),
        new(41, "PASS", "FAIL"),
        new(42, "PASS", "FAIL"),
        new(43, "Yes", "No", "N/A"),
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
                new(1, "AUSEV", "AUSMV", "Performance", "SCD-RV/External($0 invoice)"),
                new(2, "Yes", "No", "N/A"),
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
}

