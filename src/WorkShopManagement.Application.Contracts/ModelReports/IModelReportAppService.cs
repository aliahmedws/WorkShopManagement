using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace WorkShopManagement.ModelReports;

public interface IModelReportAppService : IApplicationService
{
    Task<IRemoteStreamContent> DownloadAsync(Guid carId);
}
