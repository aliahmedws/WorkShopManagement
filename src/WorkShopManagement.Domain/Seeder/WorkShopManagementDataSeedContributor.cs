using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;

namespace WorkShopManagement.Seeder;

public class WorkShopManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly CarModelDataSeedContributor _carModelSeeder;
    private readonly CheckListDataSeedContributor _checkListSeeder;

    public WorkShopManagementDataSeedContributor(
        CarModelDataSeedContributor carModelSeeder,
        CheckListDataSeedContributor checkListSeeder)
    {
        _carModelSeeder = carModelSeeder;
        _checkListSeeder = checkListSeeder;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await _carModelSeeder.SeedAsync(context);

        await _checkListSeeder.SeedAsync(context);
    }
}
