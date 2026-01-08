using Volo.Abp;

namespace WorkShopManagement.Utils.Exceptions
{
    public class DuplicateRecordException : BusinessException
    {
        public DuplicateRecordException() : base(WorkShopManagementDomainErrorCodes.DuplicateRecord) { }
        public DuplicateRecordException(string propertyName, string propertyValue)
            : base(WorkShopManagementDomainErrorCodes.DuplicateRecordWithValue)
        {
            WithData("propertyName", propertyName);
            WithData("propertyValue", propertyValue);
        }
        public DuplicateRecordException(string propertyName)
            : base(WorkShopManagementDomainErrorCodes.DuplicateRecordWithPropertyName)
        {
            WithData("propertyName", propertyName);
        }
    }
}
