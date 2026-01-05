using Volo.Abp;

namespace WorkShopManagement.Utils.Exceptions
{
    public class MissingConfigurationsException : BusinessException
    {
        public MissingConfigurationsException() : base(WorkShopManagementDomainErrorCodes.MissingConfigurations) { }
        public MissingConfigurationsException(string[] fields)
            : base(WorkShopManagementDomainErrorCodes.MissingConfigurationsWithPropertyNames)
        {
            var joinedFields = string.Join(", ", fields ?? []);
            WithData("fields", joinedFields);
        }
    }
}
