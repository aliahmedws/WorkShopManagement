using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace WorkShopManagement.VinInfos
{
    public class VinInfoManager : DomainService
    {
        private readonly IRepository<VinInfo, Guid> _vinInfoRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IClock _clock;

        public VinInfoManager(
            IRepository<VinInfo, Guid> vinInfoRepository,
            IGuidGenerator guidGenerator,
            IClock clock)
        {
            _vinInfoRepository = vinInfoRepository;
            _guidGenerator = guidGenerator;
            _clock = clock;
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
            var vin = NormalizeVin(vinNo);

            var existing = await _vinInfoRepository.FirstOrDefaultAsync(
                x => x.VinNo == vin,
                cancellationToken: ct
            );

            var ts = updatedAt ?? _clock.Now;

            if (existing is null)
            {
                var entity = new VinInfo(
                    id: _guidGenerator.Create(),
                    vinNo: vin,
                    vinResponse: vinResponse,
                    recallResponse: null,
                    vinLastUpdated: ts,
                    recallLastUpdated: default
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
            var vin = NormalizeVin(vinNo);

            var existing = await _vinInfoRepository.FirstOrDefaultAsync(
                x => x.VinNo == vin,
                cancellationToken: ct
            );

            var ts = updatedAt ?? _clock.Now;

            if (existing is null)
            {
                var entity = new VinInfo(
                    id: _guidGenerator.Create(),
                    vinNo: vin,
                    vinResponse: null,
                    recallResponse: recallResponse,
                    vinLastUpdated: default,
                    recallLastUpdated: ts
                );

                await _vinInfoRepository.InsertAsync(entity, autoSave: true, cancellationToken: ct);
                return entity;
            }

            existing.SetRecall(recallResponse, ts);

            await _vinInfoRepository.UpdateAsync(existing, autoSave: true, cancellationToken: ct);
            return existing;
        }

        private static string NormalizeVin(string vinNo)
        {
            if (vinNo.IsNullOrWhiteSpace())
                throw new BusinessException("VinInfo:VinNoRequired");

            var normalized = vinNo.Trim().ToUpperInvariant();

            // Optional: VINs are 17 chars typically; keep if you want strict validation
            // if (normalized.Length != 17) throw new BusinessException("VinInfo:VinNoInvalidLength").WithData("Length", normalized.Length);

            return normalized;
        }

    }
}
