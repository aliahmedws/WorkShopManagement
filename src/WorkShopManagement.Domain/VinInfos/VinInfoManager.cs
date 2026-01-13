using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.VinInfos
{
    public class VinInfoManager : DomainService
    {
        private readonly IRepository<VinInfo, Guid> _vinInfoRepository;
        private readonly IGuidGenerator _guidGenerator;

        public VinInfoManager(
            IRepository<VinInfo, Guid> vinInfoRepository,
            IGuidGenerator guidGenerator)
        {
            _vinInfoRepository = vinInfoRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task<VinInfo?> FindVinAsync(string? vin, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return null;
            }

            return await _vinInfoRepository.FirstOrDefaultAsync(x => x.VinNo == vin, cancellationToken: ct);
        }

        /// <summary>
        /// Upserts VIN response: if VinNo exists, updates only VinResponse + VinLastUpdated.
        /// Otherwise inserts a new row with VIN fields.
        /// </summary>
        public async Task<VinInfo> UpsertVinAsync(
            string vinNo,
            string vinResponse,
            DateTime? updatedAt = null,
            CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await FindVinAsync(vin, ct);

            var ts = updatedAt ?? Clock.Now;

            if (existing is null)
            {
                var entity = new VinInfo(
                    id: GuidGenerator.Create(),
                    vinNo: vin,
                    vinResponse: vinResponse,
                    vinLastUpdated: ts
                );

                await _vinInfoRepository.InsertAsync(entity, autoSave: true, cancellationToken: ct);
                return entity;
            }

            existing.SetVin(vinResponse, ts);

            await _vinInfoRepository.UpdateAsync(existing, autoSave: true, cancellationToken: ct);
            return existing;
        }

        /// <summary>
        /// Upserts Recall response: if VinNo exists, updates only RecallResponse + RecallLastUpdated.
        /// Otherwise inserts a new row with Recall fields.
        /// </summary>
        public async Task<VinInfo> UpsertRecallAsync(
            string vinNo,
            string recallResponse,
            DateTime? updatedAt = null,
            CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await FindVinAsync(vin, ct);

            var ts = updatedAt ?? Clock.Now;

            if (existing is null)
            {
                var entity = new VinInfo(
                    id: GuidGenerator.Create(),
                    vinNo: vin,
                    recallResponse: recallResponse,
                    recallLastUpdated: ts
                );

                await _vinInfoRepository.InsertAsync(entity, autoSave: true, cancellationToken: ct);
                return entity;
            }

            existing.SetRecall(recallResponse, ts);

            await _vinInfoRepository.UpdateAsync(existing, autoSave: true, cancellationToken: ct);
            return existing;
        }

        public async Task<VinInfo> UpsertSpecsAsync(
            string vinNo,
            string specsResponse,
            DateTime? updatedAt = null,
            CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);

            var existing = await FindVinAsync(vin, ct);

            var ts = updatedAt ?? Clock.Now;

            if (existing is null)
            {
                var entity = new VinInfo(
                    id: GuidGenerator.Create(),
                    vinNo: vin,
                    specsResponse: specsResponse,
                    specsLastUpdated: ts
                );

                await _vinInfoRepository.InsertAsync(entity, autoSave: true, cancellationToken: ct);
                return entity;
            }

            existing.SetSpecs(specsResponse, ts);

            await _vinInfoRepository.UpdateAsync(existing, autoSave: true, cancellationToken: ct);
            return existing;
        }

        public async Task<VinInfo> UpsertImagesAsync(
            string vinNo,
            string imagesResponse,
            DateTime? updatedAt = null,
            CancellationToken ct = default)
        {
            var vin = CarHelper.NormalizeAndValidateVin(vinNo);
            var existing = await FindVinAsync(vin, ct);
            var ts = updatedAt ?? Clock.Now;
            if (existing is null)
            {
                var entity = new VinInfo(
                    id: GuidGenerator.Create(),
                    vinNo: vin,
                    imagesResponse: imagesResponse,
                    imagesLastUpdated: ts
                );
                await _vinInfoRepository.InsertAsync(entity, autoSave: true, cancellationToken: ct);
                return entity;
            }
            existing.SetImages(imagesResponse, ts);
            await _vinInfoRepository.UpdateAsync(existing, autoSave: true, cancellationToken: ct);
            return existing;
        }
    }
}
