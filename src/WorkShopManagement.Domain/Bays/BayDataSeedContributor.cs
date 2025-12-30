using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace WorkShopManagement.Bays;

public class BayDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Bay, Guid> _bayRepository;

    public BayDataSeedContributor(IRepository<Bay, Guid> bayRepository)
    {
        _bayRepository = bayRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _bayRepository.AnyAsync())
        {
            return;
        }

        var bays = new List<Bay>
        {
            new(Guid.NewGuid(), "Bay 1", false),
            new(Guid.NewGuid(), "Bay 2", false),
            new(Guid.NewGuid(), "Bay 3", false),
            new(Guid.NewGuid(), "Bay 4", false),
            new(Guid.NewGuid(), "Bay 5", false),
            new(Guid.NewGuid(), "Bay 6", false),
            new(Guid.NewGuid(), "Bay 7", false),
            new(Guid.NewGuid(), "Bay 8", false),
        };

        foreach (var bay in bays)
        {
            await _bayRepository.InsertAsync(bay, autoSave: true);
        }
    }
}
