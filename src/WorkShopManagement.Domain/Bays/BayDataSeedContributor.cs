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

namespace WorkShopManagement.Bays;

public class BayDataSeedContributor : ITransientDependency
{
    private readonly IRepository<Bay, Guid> _bayRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<BayDataSeedContributor> _logger;

    public BayDataSeedContributor(
        ILogger<BayDataSeedContributor> logger,
        IRepository<Bay, Guid> bayRepository,
        IGuidGenerator guidGenerator)
    {
        _bayRepository = bayRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        _logger.LogInformation("Started.");

        var desiredBays = new List<Bay>
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

        var existing = await _bayRepository.GetListAsync();
        var existingNames = new HashSet<string>(
            existing.Select(x => x.Name.Trim()),
            StringComparer.OrdinalIgnoreCase
        );

        var inserted = 0;

        foreach (var bay in desiredBays)
        {
            if (existingNames.Contains(bay.Name.Trim()))
            {
                continue;
            }

            await _bayRepository.InsertAsync(bay, autoSave: false);
            existingNames.Add(bay.Name.Trim());
            inserted++;
        }

        _logger.LogInformation("Added {Count} new bays records", inserted);
    }
}
