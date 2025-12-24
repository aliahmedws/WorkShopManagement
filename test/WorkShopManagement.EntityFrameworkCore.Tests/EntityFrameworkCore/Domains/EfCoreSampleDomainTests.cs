using WorkShopManagement.Samples;
using Xunit;

namespace WorkShopManagement.EntityFrameworkCore.Domains;

[Collection(WorkShopManagementTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<WorkShopManagementEntityFrameworkCoreTestModule>
{

}
