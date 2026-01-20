using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.Storage;

public interface ISignedUrlAppService : IApplicationService
{
    Task<string> GetSignedReadUrlAsync(string gcsUriOrObjectName);
}
