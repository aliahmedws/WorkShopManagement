using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.External.CarsXE
{
    public interface IVinInfoService:IApplicationService
    {
        Task<VinResponseDto> GetVinAsync(
        string vinNo,
        CancellationToken ct = default);

        Task<RecallsResponseDto> GetRecallAsync(
            string vinNo,
            CancellationToken ct = default);
    }
}
