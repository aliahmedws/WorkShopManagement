using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using WorkShopManagement.Bays;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;

namespace WorkShopManagement.Seeder;

public class WorkShopManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly CarModelDataSeedContributor _carModelSeeder;
    private readonly CheckListDataSeedContributor _checkListSeeder;
    private readonly BayDataSeedContributor _baySeeder;
    private readonly ILogger<WorkShopManagementDataSeedContributor> _logger;

    public WorkShopManagementDataSeedContributor(
        CarModelDataSeedContributor carModelSeeder,
        CheckListDataSeedContributor checkListSeeder,
        BayDataSeedContributor baySeeder,
        ILogger<WorkShopManagementDataSeedContributor> logger)
    {
        _carModelSeeder = carModelSeeder;
        _checkListSeeder = checkListSeeder;
        _baySeeder = baySeeder;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await _baySeeder.SeedAsync(context);

        await _carModelSeeder.SeedAsync(context);

        await _checkListSeeder.SeedAsync(context);
    }
}
