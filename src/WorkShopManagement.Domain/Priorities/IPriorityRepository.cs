using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.Priorities;

public interface IPriorityRepository : IRepository<Priority, Guid>
{
    Task<Priority?> FindByNumberAsync(int number);
    Task<List<Priority>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        string? filter = null
    );
    Task<long> GetCountAsync(
        string? filter = null
    );
}
