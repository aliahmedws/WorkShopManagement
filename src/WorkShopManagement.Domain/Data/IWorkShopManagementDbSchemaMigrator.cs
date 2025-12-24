using System.Threading.Tasks;

namespace WorkShopManagement.Data;

public interface IWorkShopManagementDbSchemaMigrator
{
    Task MigrateAsync();
}
