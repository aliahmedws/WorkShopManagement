using Volo.Abp;

namespace WorkShopManagement.Priorities;

public class PriorityAlreadyExistsException : BusinessException
{
    public PriorityAlreadyExistsException(int number) : base(WorkShopManagementDomainErrorCodes.PriorityAlreadyExists)
    {
        WithData("number", number);
    }
}
