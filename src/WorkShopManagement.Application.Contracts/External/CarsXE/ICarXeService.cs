using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using WorkShopManagement.External.CarsXE;

namespace WorkShopManagement.External.CarsXe
{
    public interface ICarXeService : ITransientDependency
    {
        Task<VinResponseDto> GetVinAsync(
        string vinNo,
        CancellationToken ct = default);

        Task<RecallsResponseDto> GetRecallAsync(
            string vinNo,
            CancellationToken ct = default);

        Task<SpecsResponseDto> GetSpecsAsync(
            string vinNo,
            CancellationToken ct = default);

        Task<ImagesResponseDto> GetImagesAsync(
            ImagesSearchRequestDto requestDto,
            CancellationToken ct = default);

    }
}
