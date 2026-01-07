using System.Collections.Generic;
using System.Linq;
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

        public MissingConfigurationsException(Dictionary<string, string[]> settings)
            : base(WorkShopManagementDomainErrorCodes.MissingConfigurationsWithPropertyNames)
        {
            var safeSettings = settings ?? new Dictionary<string, string[]>();

            // Format each entry as "Section: Field1, Field2"
            var formattedEntries = safeSettings.Select(kvp =>
                $"{kvp.Key}: {string.Join(", ", kvp.Value ?? [])}"
            );

            var joinedFields = string.Join(" | ", formattedEntries);
            WithData("fields", joinedFields);
        }
    }
}
