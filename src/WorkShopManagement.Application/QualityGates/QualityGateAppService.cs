using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.QualityGates;

[RemoteService(IsEnabled = false)]
[Authorize(WorkShopManagementPermissions.QualityGates.Default)]
public class QualityGateAppService : ApplicationService, IQualityGateAppService
{
    private readonly IRepository<QualityGate, Guid> _repository;

    public QualityGateAppService(IRepository<QualityGate, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize(WorkShopManagementPermissions.QualityGates.Create)]
    public async Task<QualityGateDto> CreateAsync(CreateQualityGateDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNull(input.GateName, nameof(input.GateName));
        Check.NotNull(input.Status, nameof(input.Status));
        Check.NotNull(input.CarBayId, nameof(input.CarBayId));

        var entity = new QualityGate(
            GuidGenerator.Create(),
            input.CarBayId,
            input.GateName,
            input.Status
        );

        entity = await _repository.InsertAsync(entity);
        return ObjectMapper.Map<QualityGate, QualityGateDto>(entity);
    }

    [Authorize(WorkShopManagementPermissions.QualityGates.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<QualityGateDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<QualityGate, QualityGateDto>(entity);
    }

    public async Task<List<QualityGateDto>> GetListAsync()
    {
        var entities = await _repository.GetListAsync();

        var ordered = entities
            .OrderBy(x => x.CreationTime)
            .ToList();

        return ObjectMapper.Map<List<QualityGate>, List<QualityGateDto>>(ordered);
    }

    [Authorize(WorkShopManagementPermissions.QualityGates.Delete)]
    public async Task<QualityGateDto> UpdateAsync(UpdateQualityGateDto input, Guid id)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNull(id, nameof(id));
        Check.NotNull(input.GateName, nameof(input.GateName));
        Check.NotNull(input.Status, nameof(input.Status));
        Check.NotNull(input.CarBayId, nameof(input.CarBayId));

        var entity = await _repository.GetAsync(id);

        entity.CarBayId = input.CarBayId;
        entity.GateName = input.GateName;
        entity.Status = input.Status;

        if (!input.ConcurrencyStamp.IsNullOrWhiteSpace())
        {
            entity.ConcurrencyStamp = input.ConcurrencyStamp;
        }

        entity = await _repository.UpdateAsync(entity);

        return ObjectMapper.Map<QualityGate, QualityGateDto>(entity);
    }

    public async Task<List<QualityGateDto>> GetListByCarBayIdAsync(Guid carBayId)
    {
        var entities = await _repository.GetListAsync(x => x.CarBayId == carBayId);
        return ObjectMapper.Map<List<QualityGate>, List<QualityGateDto>>(
            entities.OrderBy(x => x.CreationTime).ToList()
        );
    }
}
