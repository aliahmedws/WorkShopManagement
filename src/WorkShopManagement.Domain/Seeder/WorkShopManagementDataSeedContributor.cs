using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;
using WorkShopManagement.Priorities;

namespace WorkShopManagement.Seeder;

public class WorkShopManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly CarModelDataSeedContributor _carModelSeeder;
    private readonly CheckListDataSeedContributor _checkListSeeder;
    private readonly PriorityDataSeedContributor _priorityDataSeedContributor;

    public WorkShopManagementDataSeedContributor(
        CarModelDataSeedContributor carModelSeeder,
        CheckListDataSeedContributor checkListSeeder,
        PriorityDataSeedContributor priorityDataSeedContributor)
    {
        _carModelSeeder = carModelSeeder;
        _checkListSeeder = checkListSeeder;
        _priorityDataSeedContributor = priorityDataSeedContributor;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await _carModelSeeder.SeedAsync(context);

        await _checkListSeeder.SeedAsync(context);
        await _priorityDataSeedContributor.SeedAsync(context);
    }
}
