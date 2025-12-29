using Volo.Abp;

namespace WorkShopManagement.CarModels;

public class CarModelNotFoundException : BusinessException
{
    public CarModelNotFoundException() : base(WorkShopManagementDomainErrorCodes.CarModelNotFound) { }
}
