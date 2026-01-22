using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.External.CarsXe;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.Recalls
{
    [RemoteService(false)]
    public class RecallAppService : WorkShopManagementAppService, IRecallAppService
    {
        private readonly IRepository<Recall, Guid> _recallRepository;
        private readonly IEntityAttachmentService _entityAttachmentService;
        private readonly ICarRepository _carRepository;
        private readonly ICarXeService _carXeService;
        public RecallAppService(
            IRepository<Recall, Guid> recallRepository,
            IEntityAttachmentService entityAttachmentAppService,
            ICarRepository carRepo,
            ICarXeService carXeService
            )
        {
            _recallRepository = recallRepository;
            _carRepository = carRepo;
            _carXeService = carXeService;
            _entityAttachmentService = entityAttachmentAppService;
        }



        public async Task<List<RecallDto>> GetListByCarAsync(Guid carId)
        {
            var queryable = await _recallRepository.GetQueryableAsync();
            var recalls = await AsyncExecuter.ToListAsync(queryable.Where(x => x.CarId == carId));

            if(recalls.Count <= 0)
            {
                return await GetExternalRecallsAsync(carId);  
            }

            var dtos = ObjectMapper.Map<List<Recall>, List<RecallDto>>(recalls);

            // Entity Attachments
            foreach (var dto in dtos)
            {
                var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
                {
                    EntityId = dto.Id,
                    EntityType = EntityType.Recall
                });
                dto.EntityAttachments = attachments!;
            }
            return dtos;

        }

        public async Task<List<RecallDto>> AddOrUpdateRecallsAsync(Guid carId, List<RecallDto> inputs)
        {
            var car = await _carRepository.GetAsync(carId);
            List<Recall> updatedRecalls = [];

            foreach (var input in inputs)
            {
                if (input.IsExternal || input.Id.Equals(Guid.Empty))
                {
                    // Create new recall
                    var newRecall = new Recall(
                        GuidGenerator.Create(),
                        carId,
                        input.Title,
                        input.Type,
                        input.Status,
                        input.RiskDescription,
                        input.Make,
                        input.ManufactureId,
                        input.Notes
                    );
                    newRecall = await _recallRepository.InsertAsync(newRecall);
                    updatedRecalls.Add(newRecall);

                    // --- Add EntityAttachment 
                    await _entityAttachmentService.CreateAsync(car.Vin, new CreateAttachmentDto
                    {
                        EntityType = EntityType.Recall,
                        EntityId = newRecall.Id,
                        TempFiles = input.TempFiles
                    });

                }
                else
                {
                    // Update existing recall
                    var existingRecall = await _recallRepository.GetAsync(input.Id);
                    existingRecall.SetTitle(input.Title);
                    existingRecall.SetType(input.Type);
                    existingRecall.SetStatus(input.Status);
                    existingRecall.SetNotes(input.Notes);
                    existingRecall.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
                    existingRecall = await _recallRepository.UpdateAsync(existingRecall);
                    updatedRecalls.Add(existingRecall);

                    // --- UPDATE EntityAttachment 
                    await _entityAttachmentService.UpdateAsync(car.Vin, new UpdateEntityAttachmentDto
                    {
                        EntityId = input.Id,
                        EntityType = EntityType.Recall,
                        TempFiles = input.TempFiles,
                        EntityAttachments = input.EntityAttachments
                    });
                }
            }

            return ObjectMapper.Map<List<Recall>, List<RecallDto>>(updatedRecalls);
        }

        public async Task<List<RecallDto>> GetExternalRecallsAsync(Guid carId)
        {
            var car = await _carRepository.GetAsync(carId);
            var vinNo = CarHelper.NormalizeAndValidateVin(car.Vin);

            var res = await _carXeService.GetRecallAsync(vinNo);

            var data = res?.Data;
            var recalls = data?.Recalls;

            // Error or no recalls Message? 

            if (res == null || data == null || recalls == null || recalls.Count == 0)
            {
                return [];
            }

            if (!res.Success || !data.HasRecalls || data.Recalls?.Count <= 0)
            {
                return [];
            }

            return recalls
                .Select(x => new RecallDto
                {
                    Id = Guid.NewGuid(),
                    CarId = carId,
                    Title =  (x.RecallName ?? "N/A").Trim(),
                    Make = data.Make?.Trim(),
                    ManufactureId = x.ManufacturerId?.Trim(),
                    RiskDescription = x.RiskDescription?.Trim(),
                    IsExternal = true
                }).ToList() ?? [];
        }




        public async Task<RecallDto> CreateAsync(CreateRecallDto input)
        {
            var car = await _carRepository.GetAsync(input.CarId);

            var recall = new Recall(
                GuidGenerator.Create(),
                input.CarId,
                input.Title,
                input.Type,
                input.Status,
                input.RiskDescription,
                input.Make,
                input.ManufactureId,
                input.Notes
                );

            recall = await _recallRepository.InsertAsync(recall);
            // --- CREATE EntityAttachment 
            await _entityAttachmentService.CreateAsync(car.Vin, new CreateAttachmentDto
            {
                EntityType = EntityType.Recall,
                EntityId = recall.Id,
                TempFiles = input.TempFiles
            });
            // --- create end

            return ObjectMapper.Map<Recall, RecallDto>(recall);
        }

        public async Task<RecallDto> UpdateAsync(Guid id, UpdateRecallDto input)
        {
            var recall = await _recallRepository.GetAsync(id);

            var car = await _carRepository.GetAsync(recall.CarId);

            if (recall == null)
            {
                throw new UserFriendlyException("Recall not found.");
            }

            recall.SetTitle(input.Title);
            recall.SetType(input.Type);
            recall.SetStatus(input.Status);
            recall.SetNotes(input.Notes);
            recall.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            recall = await _recallRepository.UpdateAsync(recall);

            // --- UPDATE EntityAttachment 
            await _entityAttachmentService.UpdateAsync(car.Vin, new UpdateEntityAttachmentDto
            {
                EntityId = recall.Id,
                EntityType = EntityType.Recall,
                TempFiles = input.TempFiles,
                EntityAttachments = input.EntityAttachments
            });
            // --- update end

            return ObjectMapper.Map<Recall, RecallDto>(recall);


        }

        public async Task DeleteAsync(Guid id)
        {
            var recall = await _recallRepository.GetAsync(id);
            if (recall == null)
            {
                throw new UserFriendlyException("Recall not found.");
            }
            await _recallRepository.DeleteAsync(id);
            return;
        }

        public async Task<RecallDto> GetAsync(Guid id)
        {
            var recall = await _recallRepository.GetAsync(id);
            return ObjectMapper.Map<Recall, RecallDto>(recall);
        }

    
    }
}
