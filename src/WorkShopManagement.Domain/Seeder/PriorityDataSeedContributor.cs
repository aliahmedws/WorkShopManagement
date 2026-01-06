//using System;
//using System.Threading.Tasks;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Domain.Repositories;
//using Volo.Abp.Uow;
//using WorkShopManagement.Priorities;

//namespace WorkShopManagement.Seeder;

//public class PriorityDataSeedContributor : ITransientDependency
//{
//    private readonly IPriorityRepository _priorityRepository;
//    public PriorityDataSeedContributor(IPriorityRepository priorityRepository)
//    {
//        _priorityRepository = priorityRepository;
//    }

//    [UnitOfWork]
//    public async Task SeedAsync(DataSeedContext context)
//    {
//        for (int i = 0; i <= 4; i++)
//        {
//            if (!await _priorityRepository.AnyAsync(p => p.Number == i))
//            {
//                await _priorityRepository.InsertAsync(new Priority(Guid.NewGuid(), i, $"Priority {i}"));
//            }
//        }
//    }

//}
 