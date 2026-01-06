using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace WorkShopManagement.Priorities;

public class PriorityDataSeedContributor :  ITransientDependency
{
    private readonly IRepository<Priority, Guid> _priorityRepository;
    private readonly IGuidGenerator _guidGenerator;
    public PriorityDataSeedContributor(
        IRepository<Priority, Guid> priorityRepository,
        IGuidGenerator guidGenerator)
    {
        _priorityRepository = priorityRepository;
        _guidGenerator = guidGenerator;
    }
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var priorities = new List<Priority>
        {
            new Priority(_guidGenerator.Create(), 1, "Priority 1"),
            new Priority(_guidGenerator.Create(), 2, "Priority 2"),
            new Priority(_guidGenerator.Create(), 3, "Priority 3"),
            new Priority(_guidGenerator.Create(), 4, "Priority 4"),
            new Priority(_guidGenerator.Create(), 5, "Priority 5"),
            new Priority(_guidGenerator.Create(), 6, "Priority 6")
        };
        foreach (var priority in priorities)
        {
            await _priorityRepository.InsertAsync(priority, autoSave: true);
        }
    }
}
