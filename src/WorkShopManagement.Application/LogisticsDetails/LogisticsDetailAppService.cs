using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.LogisticsDetails
{
    [RemoteService(false)]
    [Authorize(WorkShopManagementPermissions.LogisticsDetails.Default)]
    public class LogisticsDetailAppService : WorkShopManagementAppService, ILogisticsDetailAppService
    {
        private readonly ILogisticsDetailRepository _logisticsRepository;
        private readonly IRepository<Car, Guid> _carRepository;
        private readonly IEntityAttachmentService _entityAttachmentService;

        public LogisticsDetailAppService(
            ILogisticsDetailRepository logisticsRepository,
            IRepository<Car, Guid> carRepository,
            IEntityAttachmentService entityAttachmentService)
        {
            _logisticsRepository = logisticsRepository;
            _carRepository = carRepository;
            _entityAttachmentService=entityAttachmentService;
        }

        public async Task<LogisticsDetailDto> GetAsync(Guid id)
        {
            var entity = await _logisticsRepository.GetAsync(id, includeDetails: true);
            var dto = ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = id,
                EntityType = EntityType.LogisticsDetail,
            });

            dto.EntityAttachments = attachments!;
            return dto;
        }

        public async Task<LogisticsDetailDto?> GetByCarIdAsync(Guid carId)
        {
            var entity = await _logisticsRepository.FindByCarIdAsync(carId, includeDetails: true, asNoTracking: true);

            if(entity != null)
            {
                var dto = ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
                var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
                {
                    EntityId = dto.Id,
                    EntityType = EntityType.LogisticsDetail,
                });

                dto.EntityAttachments = attachments!;
                return dto;
            }

            return null;
        }

        public async Task<PagedResultDto<LogisticsDetailDto>> GetListAsync(
            PagedAndSortedResultRequestDto input,
            string? filter = null,
            Guid? carId = null)
        {
            var totalCount = await _logisticsRepository.GetLongCountAsync(filter, carId);
            var items = await _logisticsRepository.GetListAsync(
                skipCount: input.SkipCount,
                maxResultCount: input.MaxResultCount,
                sorting: input.Sorting,
                filter: filter,
                carId: carId
            );

            var dtos = ObjectMapper.Map<List<LogisticsDetail>, List<LogisticsDetailDto>>(items);

            foreach (var item in dtos)
            {
                var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
                {
                    EntityId = item.Id,
                    EntityType = EntityType.LogisticsDetail
                });
                item.EntityAttachments = attachments!;
            }

            return new PagedResultDto<LogisticsDetailDto>(totalCount, dtos);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Create)]     
        public async Task<LogisticsDetailDto> CreateAsync(CreateLogisticsDetailDto input)
        {
            // Ensure car exists
            var carExists = await _carRepository.AnyAsync(x => x.Id == input.CarId);
            if (!carExists)
            {
                throw new UserFriendlyException("Car not found.");
            }

            var entity = new LogisticsDetail(
                id: GuidGenerator.Create(),
                carId: input.CarId,
                port: input.Port,
                bookingNumber: input.BookingNumber
            );

            entity = await _logisticsRepository.InsertAsync(entity, autoSave: true);

            // --- CREATE EntityAttachment 
            await _entityAttachmentService.CreateAsync(new CreateAttachmentDto
            {
                EntityType = EntityType.LogisticsDetail,
                EntityId = entity.Id,
                TempFiles = input.TempFiles
            });
            // --- create end

            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Edit)] 
        public async Task<LogisticsDetailDto> UpdateAsync(Guid id, UpdateLogisticsDetailDto input)
        {
            var entity = await _logisticsRepository.GetAsync(id, includeDetails: true);     // TODO: use FirstOrDefaultAsync and check for null to throw not found exception

            var carExists = await _carRepository.AnyAsync(x => x.Id == input.CarId);
            if (!carExists)
            {
                throw new UserFriendlyException("Car not found.");
            }

            var other = await _logisticsRepository.FindByCarIdAsync(input.CarId, includeDetails: false, asNoTracking: true);
            if (other is not null && other.Id != entity.Id)
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.DuplicateRecordWithPropertyName)
                    .WithData("PropertyName", nameof(LogisticsDetail.CarId))
                    .WithData("Value", input.CarId);
            }
            
            entity.SetPort(input.Port);
            entity.SetBookingNumber(input.BookingNumber);
            entity.SetCreDetails(input.RsvaNumber, input.CreSubmissionDate);
            entity.SetClearanceDetails(input.ClearingAgent, input.ClearanceRemarks, input.ClearanceDate);
            entity.SetActualArrivals(input.ActualPortArrivalDate, input.ActualScdArrivalDate);
            entity.SetDeliverDetails(confirmedDeliverDate: input.ConfirmedDeliverDate,
                deliverNotes: input.DeliverNotes,
                deliverTo: input.DeliverTo,
                transportDestination: input.TransportDestination);

            await _logisticsRepository.UpdateAsync(entity, autoSave: true);

            await _entityAttachmentService.UpdateAsync(new UpdateEntityAttachmentDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.LogisticsDetail,
                TempFiles = input.TempFiles,
                EntityAttachments = input.EntityAttachments
            });
            // --- update end

            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Delete)]      
        public async Task DeleteAsync(Guid id)
        {
            await _logisticsRepository.DeleteAsync(id);
        }

    }
}
