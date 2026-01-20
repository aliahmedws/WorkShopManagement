using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.CarBays;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.External.CarsXe;
using WorkShopManagement.External.CarsXE;
using WorkShopManagement.External.Vpic;
using WorkShopManagement.Permissions;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.Cars;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.Cars.Default)]
[Authorize(WorkShopManagementPermissions.Cars.Default)]
public class CarAppService : WorkShopManagementAppService, ICarAppService
{
    private readonly ICarRepository _carRepository;
    private readonly ICarBayRepository _carBayRepository;
    private readonly IRepository<CarOwner, Guid> _carOwnerRepository;
    private readonly IVpicService _vpicService;
    private readonly ICarXeService _carXeService;
    private readonly IEntityAttachmentService _entityAttachmentService;
    private readonly CarManager _carManager;


    public CarAppService(
        ICarRepository carRepository,
        ICarBayRepository carBayRepository,
        IRepository<CarOwner, Guid> carOwnerRepository,
        IVpicService vpicService,
        ICarXeService carXeService,
        IEntityAttachmentService entityAttachmentService,
        CarManager carManager
        )
    {
        _carRepository = carRepository;
        _carBayRepository = carBayRepository;
        _carOwnerRepository = carOwnerRepository;
        _vpicService = vpicService;
        _carXeService = carXeService;
        _entityAttachmentService = entityAttachmentService;
        _carManager = carManager;
    }

    public async Task<CarDto> GetAsync(Guid id)
    {
        var car = await _carRepository.GetAsync(id);

        var dto = ObjectMapper.Map<Car, CarDto>(car);

        var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
        {
            EntityId = id,
            EntityType = EntityType.Car
        });

        dto.EntityAttachments = attachments!;
        return dto;

    }

    public async Task<PagedResultDto<CarDto>> GetListAsync(GetCarListInput input)
    {
        var totalCount = await _carRepository.GetLongCountAsync(input.Filter, input.Stage);

        var items = await _carRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.Stage);

        var dtos = ObjectMapper.Map<List<Car>, List<CarDto>>(items);

        var carIds = dtos.Select(x => x.Id).ToList();

        var carBays = await _carBayRepository.GetListAsync(
           carId: null,
           bayId: null,
           isActive: false,
           maxResultCount: int.MaxValue
        );

        var bayByCarId = carBays
           .Where(cb => carIds.Contains(cb.CarId))
           .GroupBy(cb => cb.CarId)
           .Select(g => g.OrderByDescending(x => x.CreationTime).First())
           .ToDictionary(x => x.CarId);

        foreach (var dto in dtos)
        {
            if (bayByCarId.TryGetValue(dto.Id, out var cb))
            {
                dto.BayId = cb.BayId;
                dto.BayName = cb.Bay?.Name;
                dto.CarBayId = cb.Id;
            }
        }


        foreach (var item in dtos)
        {
            var attachments = await _entityAttachmentService.GetListAsync(new GetEntityAttachmentListDto
            {
                EntityId = item.Id,
                EntityType = EntityType.Car
            });
            item.EntityAttachments = attachments!;
        }

        return new PagedResultDto<CarDto>(totalCount, dtos);

    }

    [Authorize(WorkShopManagementPermissions.Cars.Create)]
    public async Task<CarDto> CreateAsync(CreateCarDto input)
    {

        var ownerId = await ResolveOrCreateOwnerAsync(input.OwnerId, input.Owner);
        var car = await _carManager.CreateAsync(
            GuidGenerator.Create(),
            ownerId: ownerId,
            vin: input.Vin,
            color: input.Color,
            modelId: input.ModelId,
            modelYear: input.ModelYear,
            cnc: input.Cnc,
            cncFirewall: input.CncFirewall,
            cncColumn: input.CncColumn,
            dueDate: input.DueDate,
            deliverDate: input.DeliverDate,
            startDate: input.StartDate,
            notes: input.Notes,
            missingParts: input.MissingParts,
            //storageLocation: input.StorageLocation,
            buildMaterialNumber: input.BuildMaterialNumber,
            angleBailment: input.AngleBailment,
            avvStatus: input.AvvStatus,
            pdiStatus: input.PdiStatus,

            imageLink: input.ImageLink

        );

        var entity = await _carRepository.InsertAsync(car, autoSave: true);


        // --- CREATE EntityAttachment 
        await _entityAttachmentService.CreateAsync(car.Vin, new CreateAttachmentDto
        {
            EntityType = EntityType.Car,
            EntityId = entity.Id,
            TempFiles = input.TempFiles
        });
        // --- create end


        //Create Logistics Default ?? 

        return ObjectMapper.Map<Car, CarDto>(car);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Edit)]
    public async Task<CarDto> UpdateAsync(Guid id, UpdateCarDto input)
    {
        var ownerId = await ResolveOrCreateOwnerAsync(input.OwnerId, input.Owner);
        var car = await _carManager.UpdateAsync(
            id,
            ownerId,
            input.Vin,
            input.Color,
            input.ModelId,
            input.ModelYear,
            input.Cnc,
            input.CncFirewall,
            input.CncColumn,
            input.DueDate,
            input.DeliverDate,
            input.StartDate,
            input.Notes,
            input.MissingParts,
            //input.StorageLocation,
            input.BuildMaterialNumber,
            input.AngleBailment,
            input.AvvStatus,
            input.PdiStatus,

            input.ImageLink
        );

        var entity = await _carRepository.UpdateAsync(car, autoSave: true);
        // --- UPDATE EntityAttachment 
        await _entityAttachmentService.UpdateAsync(car.Vin, new UpdateEntityAttachmentDto
        {
            EntityId = entity.Id,
            EntityType = EntityType.Car,
            TempFiles = input.TempFiles,
            EntityAttachments = input.EntityAttachments
        });
        // --- update end

        return ObjectMapper.Map<Car, CarDto>(car);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var car = await _carRepository.GetAsync(id);
        await _carRepository.DeleteAsync(id);
        await _entityAttachmentService.DeleteAsync(id, EntityType.Car, car.Vin);
    }

    private async Task<Guid> ResolveOrCreateOwnerAsync(Guid? carOwnerId, CreateCarOwnerDto? ownerDto)
    {
        // Case A: Front-end provides CarOwnerId
        if (carOwnerId.HasValue && carOwnerId.Value != Guid.Empty)
        {
            var exists = await _carOwnerRepository.AnyAsync(x => x.Id == carOwnerId.Value);
            if (!exists)
            {
                // Owner id provided, but not found; require caller to send owner details.
                throw new UserFriendlyException("Car owner not found. Provide valid CarOwnerId or provide CarOwner details to create a new owner.");
            }

            return carOwnerId.Value;
        }

        // Case B: No id provided => must provide owner details
        if (ownerDto is null)
        {
            throw new UserFriendlyException("CarOwnerId or CarOwner details are required.");
        }

        var newOwner = new CarOwner(GuidGenerator.Create(), ownerDto.Name, ownerDto.Email, ownerDto.ContactId);

        await _carOwnerRepository.InsertAsync(newOwner, autoSave: true);

        return newOwner.Id;
    }

    public async Task<ExternalCarDetailsDto> GetExternalCarDetailsAsync(
        [Length(CarConsts.VinLength, CarConsts.VinLength)]
        string vin,
        string? modelYear = null
        )
    {
        // CarXe Api
        var res = await _carXeService.GetSpecsAsync(vin); 
        if (res != null && res.Success)                    
        {
            var dto = new ExternalCarDetailsDto
            {
                Error = res.Message,
                Success = res.Success
            };

            if (res.Attributes != null)
            {

                dto.Model = res.Attributes.Model;
                dto.ModelYear = res.Attributes.Year;
                dto.Colors = res.Attributes.ExteriorColor ?? []; 
               
            }
            return dto;

        }

        // govt api
        var externalCarDetails = await _vpicService.DecodeVinExtendedAsync(vin, modelYear);
        return ObjectMapper.Map<VpicVariableResultDto, ExternalCarDetailsDto>(externalCarDetails);
    }

    public async Task<CarDto> SaveCarImageAsync(
        [Required]
        Guid carId,
        [Required]
        [MaxLength(CarConsts.ImageLinkLength)]
        string link)
    {
        var car = await _carRepository.GetAsync(carId);                     
        //var car = await _carRepository.GetWithDetailsAsync(carId);        // Get with detail to save url of model in car link

        CarHelper.TryGetValidHttpUrl(link, out var url);
        car.SetImageLink(url);

        car = await _carRepository.UpdateAsync(car, autoSave: true);
        return ObjectMapper.Map<Car, CarDto>(car);
    }



    public async Task<List<string>> GetExternalCarImagesAsync(Guid carId)
    {
        var car = await _carRepository.GetAsync(carId);

        // Calling get Specs to get (Trim, Make)
        var specs = await _carXeService.GetSpecsAsync(car.Vin);

        var make = specs?.Attributes?.Make;
        var trim = specs?.Attributes?.Trim;
        var model = specs?.Attributes?.Model;         // from 

        if (string.IsNullOrEmpty(make) &&  string.IsNullOrEmpty(model))
        {
            return [];              // cannot get image without make
        }

        var input = new ImagesSearchRequestDto
        {
            Vin = car.Vin,
            Make = make!,
            Model = model!,
            Trim = trim,
            Year = car.ModelYear.ToString(),
            Color = car.Color,
        };

        var res = await _carXeService.GetImagesAsync(input);
        List<string> imgs = [];
        if (res != null && res.Success)
        {
            if(res.Images != null && res.Images.Count!=0)
            {
                foreach (var i in res.Images)
                {
                    if (!CarHelper.TryGetValidHttpUrl(i.Link, out var url))
                        continue;

                    imgs.Add(url!);
                }
            }
        }

        return imgs.Distinct(StringComparer.OrdinalIgnoreCase).ToList();


    }
    public async Task<CarDto> ChangeStageAsync(Guid id, ChangeCarStageDto input)
    {
        //var car = await _carRepository.GetAsync(id); // Get is working in manager
        var car = await _carManager.ChangeStageAsync(id, input.TargetStage);

        car = await _carRepository.UpdateAsync(car);

        return ObjectMapper.Map<Car, CarDto>(car);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Edit)]
    public async Task<CarDto> UpdateAvvStatusAsync(Guid id, UpdateCarAvvStatusDto input)
    {
        var car = await _carRepository.GetAsync(id);
        car.SetAvvStatus(input.AvvStatus);

        var entity = await _carRepository.UpdateAsync(car, autoSave: true);

        return ObjectMapper.Map<Car, CarDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.Cars.Edit)]
    public async Task<CarDto> UpdateEstimatedReleaseAsync(Guid id, DateTime estimatedReleaseDate)
    {
        var car = await _carRepository.GetAsync(id);


        car.SetSchedule(
            dueDate: car.DueDate,
            deliverDate: estimatedReleaseDate,
            startDate: car.StartDate
        );

        var entity = await _carRepository.UpdateAsync(car, autoSave: true);
        return ObjectMapper.Map<Car, CarDto>(entity);
    }

    public async Task<CarDto> UpdateNotesAsync(Guid id, string? notes)
    {
        var car = await _carRepository.GetAsync(id);

        car.SetNotes(notes, car.MissingParts);

        var entity = await _carRepository.UpdateAsync(car, autoSave: true);
        return ObjectMapper.Map<Car, CarDto>(entity);
    }
}
