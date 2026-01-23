using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;
using static WorkShopManagement.Permissions.WorkShopManagementPermissions;

namespace WorkShopManagement.LogisticsDetails
{
    [RemoteService(false)]
    [Authorize(WorkShopManagementPermissions.LogisticsDetails.Default)]
    public class LogisticsDetailAppService : WorkShopManagementAppService, ILogisticsDetailAppService
    {
        private readonly ILogisticsDetailRepository _logisticsRepository;
        private readonly IRepository<Car, Guid> _carRepository;
        private readonly IEntityAttachmentService _entityAttachmentService;
        private readonly LogisticsDetailManager _logisticsDetailManager;

        public LogisticsDetailAppService(
            ILogisticsDetailRepository logisticsRepository,
            IRepository<Car, Guid> carRepository,
            IEntityAttachmentService entityAttachmentService,
            LogisticsDetailManager logisticsDetailManager)
        {
            _logisticsRepository = logisticsRepository;
            _carRepository = carRepository;
            _entityAttachmentService=entityAttachmentService;
            _logisticsDetailManager = logisticsDetailManager;
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
            
            var logisticDetail = await _logisticsDetailManager.CreateAsync(
                carId: input.CarId,
                port: input.Port,
                bookingNumber: input.BookingNumber
            );
             
            logisticDetail = await _logisticsRepository.InsertAsync(logisticDetail, autoSave: true);

            var car = await _carRepository.GetAsync(logisticDetail.CarId);

            // --- CREATE EntityAttachment 
            await _entityAttachmentService.CreateAsync(car.Vin, new CreateAttachmentDto
            {
                EntityType = EntityType.LogisticsDetail,
                EntityId = logisticDetail.Id,
                TempFiles = input.TempFiles
            });
            // --- create end

            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(logisticDetail);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Edit)] 
        public async Task<LogisticsDetailDto> UpdateAsync(Guid id, UpdateLogisticsDetailDto input)
        {
            var entity = await _logisticsDetailManager.UpdateAsync(
                id: id,
                port: input.Port,
                bookingNumber: input.BookingNumber,
                creStatus: input.CreStatus,
                creSubmissionDate: input.CreSubmissionDate,
                rvsaNumber: input.RvsaNumber,
                clearingAgent: input.ClearingAgent,
                clearanceRemarks: input.ClearanceRemarks,
                clearanceDate: input.ClearanceDate,
                actualPortArrivalDate:  input.ActualPortArrivalDate,
                actualScdArrivalDate: input.ActualScdArrivalDate
            );

            var car = await _carRepository.GetAsync(entity.CarId);

            await _logisticsRepository.UpdateAsync(entity, autoSave: true);
            await _entityAttachmentService.UpdateAsync(car.Vin, new UpdateEntityAttachmentDto
            {
                EntityId = entity.Id,
                EntityType = EntityType.LogisticsDetail,
                TempFiles = input.TempFiles,
                EntityAttachments = input.EntityAttachments
            });
            // --- update end

            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Edit)]
        public async Task<LogisticsDetailDto> SubmitCreStatusAsync(Guid id)
        {
            var entity = await _logisticsDetailManager.SubmitCreStatus(id);
            await _logisticsRepository.UpdateAsync(entity, autoSave: true);
            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Edit)]
        public async Task<LogisticsDetailDto> AddOrUpdateDeliverDetailsAsync(Guid id, AddOrUpdateDeliverDetailDto input)
        {
            var entity = await _logisticsDetailManager.AddOrUpdateDeliverDetailsAsync(
                id: id,
                confirmedDeliverDate: input.ConfirmedDeliverDate,
                deliverNotes: input.DeliverNotes,
                deliverTo: input.DeliverTo,
                transportDestination: input.TransportDestination
                );

            await _logisticsRepository.UpdateAsync(entity, autoSave: true);
            return ObjectMapper.Map<LogisticsDetail, LogisticsDetailDto>(entity);
        }

        [Authorize(WorkShopManagementPermissions.LogisticsDetails.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _logisticsRepository.DeleteAsync(id);
        }

        public async Task<CreDetailDto> GetCreDetailByCarIdAsync(Guid carId)
        {
            var logis = await _logisticsRepository.FindByCarIdAsync(carId);
            if (logis == null)
            {
                throw new UserFriendlyException("Logistic Details for the car does not exist.");
            }
            return ObjectMapper.Map<LogisticsDetail, CreDetailDto>(logis);
        }

        public async Task<CreDetailDto> AddOrUpdateCreDetailAsync(
            Guid carId,
            AddOrUpdateCreDetailDto input)
        {
            var logis = await _logisticsRepository.FindByCarIdAsync(carId);
            if (logis == null)
            {
                throw new UserFriendlyException("Logistic Details for the car does not exist.");
            }

            logis = await _logisticsDetailManager.AddOrUpdateCreDetailsAsync(id: logis.Id, input.CreStatus, input.CreSubmissionDate, input.RvsaNumber);
            return ObjectMapper.Map<LogisticsDetail, CreDetailDto>(logis);
        }
    }
}
