using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace WorkShopManagement.Bays;

public class BayDataSeedContributor : ITransientDependency
{
    private readonly IRepository<Bay, Guid> _bayRepository;
    private readonly IGuidGenerator _guidGenerator;

    public BayDataSeedContributor(IRepository<Bay, Guid> bayRepository, IGuidGenerator guidGenerator)
    {
        _bayRepository = bayRepository;
        _guidGenerator = guidGenerator;
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
            new(_guidGenerator.Create(), "Bay 1", false),
            new(_guidGenerator.Create(), "Bay 2", false),
            new(_guidGenerator.Create(), "Bay 3", false),
            new(_guidGenerator.Create(), "Bay 4", false),
            new(_guidGenerator.Create(), "Bay 5", false),
            new(_guidGenerator.Create(), "Bay 6", false),
            new(_guidGenerator.Create(), "Bay 7", false),
            new(_guidGenerator.Create(), "Bay 8", false),
            new(_guidGenerator.Create(), "Bay 9", false),
            new(_guidGenerator.Create(), "Bay 10", false),
            new(_guidGenerator.Create(), "Bay 11", false),
            new(_guidGenerator.Create(), "Bay 12", false),
            new(_guidGenerator.Create(), "Bay 13", false),
            new(_guidGenerator.Create(), "Bay 14", false),
            new(_guidGenerator.Create(), "Bay 15", false),
            new(_guidGenerator.Create(), "Bay 16", false),
            new(_guidGenerator.Create(), "Bay 17", false),
            new(_guidGenerator.Create(), "Bay 18", false),
            new(_guidGenerator.Create(), "Bay 19", false),
            new(_guidGenerator.Create(), "Bay 20", false),
        };

        foreach (var bay in bays)
        {
            await _bayRepository.InsertAsync(bay, autoSave: true);
        }
    }
}
