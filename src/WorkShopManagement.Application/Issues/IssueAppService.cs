using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.Permissions;

namespace WorkShopManagement.Issues;

[RemoteService(false)]
[Authorize(WorkShopManagementPermissions.Issues.Default)]
public class IssueAppService : WorkShopManagementAppService, IIssueAppService
{
    private readonly IRepository<Issue, Guid> _issueRepository;
    private readonly IEntityAttachmentService _attachmentService;
    private readonly ICarRepository _carRepository;

    public IssueAppService(
        IRepository<Issue, Guid> issueRepository, 
        IEntityAttachmentService attachmentService,
        ICarRepository carRepository
        )
    {
        _issueRepository = issueRepository;
        _attachmentService = attachmentService;
        _carRepository = carRepository;
    }

    [Authorize(WorkShopManagementPermissions.Issues.Upsert)]
    public async Task UpsertAsync(Guid carId, UpsertIssuesRequestDto input)
    {
        Check.NotDefaultOrNull<Guid>(carId, nameof(carId));
        Check.NotNull(input, nameof(input));
        Check.NotNull(input.Items, nameof(input.Items));
        
        // Ensure car exists (fail fast, clearer error)
        await _carRepository.GetAsync(carId);

        var duplicateIds = input.Items
            .Where(i => i.Id.HasValue)
            .GroupBy(i => i.Id!.Value)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateIds.Count > 0)
        {
            throw new BusinessException("WorkShopManagement:DuplicateIssueIdsInRequest")
                .WithData("IssueIds", string.Join(",", duplicateIds));
        }

        var existingIssues = await _issueRepository.GetListAsync(i => i.CarId == carId);

        var requestIds = input.Items
            .Where(i => i.Id.HasValue)
            .Select(i => i.Id!.Value)
            .ToHashSet();

        var issuesToDelete = existingIssues
            .Where(i => !requestIds.Contains(i.Id))
            .ToList();

        if (issuesToDelete.Count > 0)
        {
            await _issueRepository.DeleteManyAsync(issuesToDelete);
            await _attachmentService.DeleteManyAsync(EntityType.Issue, [.. issuesToDelete.Select(i => i.Id)]);
        }

        var existingDict = existingIssues.Except(issuesToDelete).ToDictionary(i => i.Id);

        foreach (var item in input.Items)
        {
            var entityId = Guid.Empty;

            if (item.Id.HasValue)
            {
                if (!existingDict.TryGetValue(item.Id.Value, out var issue))
                {
                    throw new EntityNotFoundException(typeof(Issue), item.Id.Value);
                }

                entityId = IssueBuilder.Update(issue, item).Id;
            }
            else
            {
                entityId = GuidGenerator.Create();
                var newIssue = IssueBuilder.Create(carId, entityId, item);
                await _issueRepository.InsertAsync(newIssue);
            }

            await _attachmentService.UpdateAsync(new UpdateEntityAttachmentDto
            {
                EntityId = entityId,
                EntityType = EntityType.Issue,
                TempFiles = item.TempFiles,
                EntityAttachments = item.EntityAttachments
            });
        }
    }

    public async Task<ListResultDto<IssueDto>> GetListByCarAsync(Guid carId)
    {
        var issues = await _issueRepository.GetListAsync(i => i.CarId == carId);
        var issueIds = issues.Select(i => i.Id).ToList();

        var mapped = ObjectMapper.Map<List<Issue>, List<IssueDto>>(issues);

        var fileAttachments = await _attachmentService.GetListAsync(EntityType.Issue, issueIds);
        var attachmentDict = fileAttachments.GroupBy(x => x.EntityId).ToDictionary(x => x.Key);

        foreach (var issue in mapped)
        {
            attachmentDict.TryGetValue(issue.Id, out var attachments);
            issue.EntityAttachments = attachments?.ToList() ?? [];
        }

        return new ListResultDto<IssueDto>(mapped);
    }
}