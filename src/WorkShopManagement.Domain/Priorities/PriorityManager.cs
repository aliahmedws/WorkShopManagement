using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace WorkShopManagement.Priorities;

public class PriorityManager : DomainService
{
    private readonly IPriorityRepository _priorityRepository;
    public PriorityManager(IPriorityRepository priorityRepository)
    {
        _priorityRepository = priorityRepository;
    }

    public async Task<Priority> CreateAsync(
       int number,
       string? description = null
       )
    {
        Check.NotNull(number, nameof(number));

        var existingPriority = await _priorityRepository.FindByNumberAsync(number);
        if (existingPriority != null)
        {
            throw new PriorityAlreadyExistsException(number);
        }

        return new Priority(
            GuidGenerator.Create(),
            number,
            description
        );
    }

    public async Task ChangeNumberAsync(
       Priority priority,
       int newNumber
       )
    {
        Check.NotNull(priority, nameof(priority));

        var existingPriority = await _priorityRepository.FindByNumberAsync(newNumber);
        if (existingPriority != null && existingPriority.Id != priority.Id)
        {
            throw new PriorityAlreadyExistsException(newNumber);
        }

        priority.ChangeNumber(newNumber);
    }

}
