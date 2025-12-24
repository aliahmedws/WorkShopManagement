using Xunit;

namespace WorkShopManagement.EntityFrameworkCore;

[CollectionDefinition(WorkShopManagementTestConsts.CollectionDefinitionName)]
public class WorkShopManagementEntityFrameworkCoreCollection : ICollectionFixture<WorkShopManagementEntityFrameworkCoreFixture>
{

}
