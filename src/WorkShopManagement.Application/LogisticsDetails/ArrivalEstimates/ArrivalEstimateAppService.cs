using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    [RemoteService(false)]
    [Authorize(WorkShopManagementPermissions.ArrivalEstimates.Default)]
    public class ArrivalEstimateAppService : WorkShopManagementAppService, IArrivalEstimateAppService
    {
        private readonly IArrivalEstimateRepository _estimateRepository;
        private readonly ILogisticsDetailRepository _logisticsRepository;
        private readonly IEntityAttachmentService _entityAttachmentService;

        public ArrivalEstimateAppService(
            IArrivalEstimateRepository estimateRepository,
            ILogisticsDetailRepository logisticsRepository,
            IEntityAttachmentService entityAttachmentService)
        {
            _estimateRepository = estimateRepository;
            _logisticsRepository = logisticsRepository;
            _entityAttachmentService = entityAttachmentService;
        }

        public async Task<ArrivalEstimateDto> GetAsync(Guid id)
        {
            var entity = await _estimateRepository.GetAsync(id);

            var dto = ObjectMapper.Map<ArrivalEstimate, ArrivalEstimateDto>(entity);

            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = id,
                EntityType = EntityType.ArrivalEstimate
            });

            dto.EntityAttachments = attachments ?? [];

            return dto;
        }

        public async Task<PagedResultDto<ArrivalEstimateDto>> GetListAsync(Guid logisticsDetailId, PagedAndSortedResultRequestDto input)
        {
            // ensure logistics exists
            var logisticsExists = await _logisticsRepository.AnyAsync(x => x.Id == logisticsDetailId);
            if (!logisticsExists)
            {
                throw new UserFriendlyException("Logistics detail not found.");
            }

            var totalCount = await _estimateRepository.GetLongCountAsync(logisticsDetailId);

            var items = await _estimateRepository.GetListAsync(
                logisticsDetailId: logisticsDetailId,
                skipCount: input.SkipCount,
                maxResultCount: input.MaxResultCount,
                sorting: input.Sorting,
                asNoTracking: true
            );

            var dtos = ObjectMapper.Map<List<ArrivalEstimate>, List<ArrivalEstimateDto>>(items);

            foreach (var dto in dtos)
            {
                var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
                {
                    EntityId = dto.Id,
                    EntityType = EntityType.ArrivalEstimate
                });

                dto.EntityAttachments = attachments ?? [];
            }

            return new PagedResultDto<ArrivalEstimateDto>(totalCount, dtos);
        }

        [Authorize(WorkShopManagementPermissions.ArrivalEstimates.Create)]
        public async Task<ArrivalEstimateDto> CreateAsync(CreateArrivalEstimateDto input)
        {
            // Ensure logistics exists
            var logistics = await _logisticsRepository.GetAsync(input.LogisticsDetailId, includeDetails: false);

            var entity = new ArrivalEstimate(
                id: GuidGenerator.Create(),
                logisticsDetailId: logistics.Id,
                etaPort: input.EtaPort,
                etaScd: input.EtaScd,
                notes: input.Notes
            );

            entity = await _estimateRepository.InsertAsync(entity, autoSave: true);

            // --- CREATE EntityAttachment
            await _entityAttachmentService.CreateAsync(new CreateAttachmentDto
            {
                EntityType = EntityType.ArrivalEstimate,
                EntityId = entity.Id,
                TempFiles = input.TempFiles
            });
            // --- create end

            var dto = ObjectMapper.Map<ArrivalEstimate, ArrivalEstimateDto>(entity);

            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.ArrivalEstimate
            });

            dto.EntityAttachments = attachments ?? [];

            return dto;
        }

        [Authorize(WorkShopManagementPermissions.ArrivalEstimates.Edit)]
        public async Task<ArrivalEstimateDto> UpdateAsync(Guid id, UpdateArrivalEstimateDto input)
        {
            var entity = await _estimateRepository.GetAsync(id);

            entity.SetNotes(input.Notes);

            entity = await _estimateRepository.UpdateAsync(entity, autoSave: true);

            // --- UPDATE EntityAttachment
            await _entityAttachmentService.UpdateAsync(new UpdateEntityAttachmentDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.ArrivalEstimate,
                TempFiles = input.TempFiles,
                EntityAttachments = input.EntityAttachments
            });
            // --- update end

            var dto = ObjectMapper.Map<ArrivalEstimate, ArrivalEstimateDto>(entity);

            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.ArrivalEstimate
            });

            dto.EntityAttachments = attachments ?? [];

            return dto;
        }

        [Authorize(WorkShopManagementPermissions.ArrivalEstimates.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _entityAttachmentService.DeleteAsync(id, EntityType.ArrivalEstimate);
            await _estimateRepository.DeleteAsync(id);
        }

        public async Task<ArrivalEstimateDto?> GetLatestAsync(Guid logisticsDetailId)
        {
            var latest = await _estimateRepository.GetLatestAsync(logisticsDetailId, asNoTracking: true);
            if (latest is null)
            {
                return null;
            }

            var dto = ObjectMapper.Map<ArrivalEstimate, ArrivalEstimateDto>(latest);

            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = latest.Id,
                EntityType = EntityType.ArrivalEstimate
            });

            dto.EntityAttachments = attachments ?? [];

            return dto;
        }
    }
}