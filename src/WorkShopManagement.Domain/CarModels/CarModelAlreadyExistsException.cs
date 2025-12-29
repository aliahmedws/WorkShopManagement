using Volo.Abp;

namespace WorkShopManagement.CarModels;

public class CarModelAlreadyExistsException : BusinessException
{
    public CarModelAlreadyExistsException(string name) : base(WorkShopManagementDomainErrorCodes.CarModelAlreadyExists)
    {
        WithData("name", name);
    }
}
