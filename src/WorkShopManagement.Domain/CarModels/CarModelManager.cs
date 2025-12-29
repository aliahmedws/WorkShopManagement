using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using WorkShopManagement.FileAttachments;

namespace WorkShopManagement.CarModels;

public class CarModelManager : DomainService
{
    private readonly IRepository<CarModel, Guid> _repository;
    private readonly FileManager _fileManager;

    public CarModelManager(
        IRepository<CarModel, Guid> repository,
        FileManager fileManager)
    {
        _repository = repository;
        _fileManager = fileManager;
    }

    public async Task<CarModel> CreateAsync(
        string name,
        string? description,
        IFormFile file)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNull(file, nameof(file));

        var tenantId = CurrentTenant.Id;
        var existing = await _repository.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == name);
        if (existing != null)
        {
            throw new CarModelAlreadyExistsException(name);
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var attachment = await _fileManager.SaveAsync(stream, file.FileName, "car-models");

        var carModel = new CarModel(
            GuidGenerator.Create(),
            name,
            description,
            attachment
        );

        await _repository.InsertAsync(carModel, autoSave: true);
        return carModel;
    }

    public async Task DeleteAsync(Guid id)
    {
        Check.NotNull(id, nameof(id));

        var carModel = await _repository.FirstOrDefaultAsync(x => x.Id == id);
        if (carModel == null)
        {
            throw new CarModelNotFoundException();
        }

        if (carModel.FileAttachments != null && !string.IsNullOrWhiteSpace(carModel.FileAttachments.Path))
        {
            await _fileManager.DeleteFileAsync(carModel.FileAttachments);
        }

        await _repository.DeleteAsync(carModel, autoSave: true);
    }

    public async Task<CarModel> UpdateAsync(
        Guid id,
        string name,
        string? description,
        string? concurrencyStamp = null)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNullOrWhiteSpace(name, nameof(name));

        var carModel = await _repository.FirstOrDefaultAsync(x => x.Id == id);
        if (carModel == null)
        {
            throw new CarModelNotFoundException();
        }

        carModel.SetConcurrencyStampIfNotNull(concurrencyStamp);

        var duplicate = await _repository.FirstOrDefaultAsync(x => x.Id != id && x.Name == name);
        if (duplicate != null)
        {
            throw new CarModelAlreadyExistsException(name);
        }

        carModel.ChangeName(name);
        carModel.ChangeDescription(description);


        await _repository.UpdateAsync(carModel, autoSave: true);
        return carModel;
    }
}
