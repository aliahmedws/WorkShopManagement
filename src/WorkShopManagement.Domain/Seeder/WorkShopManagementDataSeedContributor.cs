using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using WorkShopManagement.Bays;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;
using WorkShopManagement.ListItems;
using WorkShopManagement.ModelCategories;
using WorkShopManagement.RadioOptions;

namespace WorkShopManagement.Seeder;

public class WorkShopManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ModelCategoryDataSeedContributor _modelCategorySeeder;
    private readonly CarModelDataSeedContributor _carModelSeeder;
    private readonly CheckListDataSeedContributor _checkListSeeder;
    private readonly BayDataSeedContributor _baySeeder;
    private readonly ListItemDataSeedContributor _listItemSeeder;
    private readonly RadioOptionDataSeedContributor _radioOptionSeeder;
    private readonly ILogger<WorkShopManagementDataSeedContributor> _logger;

    public WorkShopManagementDataSeedContributor(
        ModelCategoryDataSeedContributor modelCategorySeeder,
        CarModelDataSeedContributor carModelSeeder,
        CheckListDataSeedContributor checkListSeeder,
        BayDataSeedContributor baySeeder,
        ListItemDataSeedContributor listItemSeeder,
        RadioOptionDataSeedContributor radioOptionSeeder,
        ILogger<WorkShopManagementDataSeedContributor> logger)
    {
        _modelCategorySeeder = modelCategorySeeder;
        _carModelSeeder = carModelSeeder;
        _checkListSeeder = checkListSeeder;
        _listItemSeeder = listItemSeeder;
        _radioOptionSeeder = radioOptionSeeder;
        _baySeeder = baySeeder;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "WorkShop seed started. PropertiesCount={PropertiesCount}",
            context?.Properties?.Count ?? 0);

        try
        {
            await RunStepAsync("Bays", () => _baySeeder.SeedAsync(context!));
            await RunStepAsync("ModelCategories", () => _modelCategorySeeder.SeedAsync(context!));
            await RunStepAsync("CarModels", () => _carModelSeeder.SeedAsync(context!));
            await RunStepAsync("CheckLists", () => _checkListSeeder.SeedAsync(context!));
            await RunStepAsync("ListItems", () => _listItemSeeder.SeedAsync(context!));
            await RunStepAsync("RadioOptions", () => _radioOptionSeeder.SeedAsync(context!));

            sw.Stop();
            _logger.LogInformation("Workshop seed finished successfully. ElapsedMs={ElapsedMs}", sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Workshop seed failed. ElapsedMs={ElapsedMs}", sw.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task RunStepAsync(string stepName, Func<Task> action)
    {
        _logger.LogInformation("Seeding {Step} started.", stepName);

        var sw = Stopwatch.StartNew();
        await action();
        sw.Stop();

        _logger.LogInformation("Seeding {Step} finished. ElapsedMs={ElapsedMs}", stepName, sw.ElapsedMilliseconds);
    }
}
