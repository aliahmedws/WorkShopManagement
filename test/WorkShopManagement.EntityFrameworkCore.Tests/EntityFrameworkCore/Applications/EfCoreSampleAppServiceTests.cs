using WorkShopManagement.Samples;
using Xunit;

namespace WorkShopManagement.EntityFrameworkCore.Applications;

[Collection(WorkShopManagementTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<WorkShopManagementEntityFrameworkCoreTestModule>
{

}
