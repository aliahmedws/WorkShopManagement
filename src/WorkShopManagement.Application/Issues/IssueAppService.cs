using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
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
    private readonly IRepository<IdentityUser, Guid> _userRepository;

    public IssueAppService(
        IRepository<Issue, Guid> issueRepository,
        IEntityAttachmentService attachmentService,
        ICarRepository carRepository,
        IRepository<IdentityUser, Guid> userRepository
        )
    {
        _issueRepository = issueRepository;
        _attachmentService = attachmentService;
        _carRepository = carRepository;
        _userRepository = userRepository;
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

        var userIds = issues
            .Select(i => new[] { i.CreatorId, i.LastModifierId })
            .SelectMany(userId => userId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        var mapped = ObjectMapper.Map<List<Issue>, List<IssueDto>>(issues);

        var fileAttachments = (await _attachmentService.GetListAsync(EntityType.Issue, issueIds))
                            .GroupBy(x => x.EntityId)
                            .ToDictionary(x => x.Key);

        var users = (await _userRepository.GetListAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id);

        foreach (var issue in mapped)
        {
            fileAttachments.TryGetValue(issue.Id, out var attachments);
            issue.EntityAttachments = attachments?.ToList() ?? [];

            if (issue.CreatorId != null)
            {
                users.TryGetValue(issue.CreatorId.Value, out var createdBy);
                issue.CreatorEmail = createdBy?.Email;
            }

            if (issue.LastModifierId != null)
            {
                users.TryGetValue(issue.LastModifierId.Value, out var lastModifiedBy);
                issue.LastModifierEmail = lastModifiedBy?.Email;
            }
        }

        return new ListResultDto<IssueDto>(mapped);
    }

    public async Task<PagedResultDto<IssueListDto>> GetListAsync(GetIssueListInput input)
    {
        var cars = await _carRepository.GetQueryableAsync();
        var issues = await _issueRepository.GetQueryableAsync();

        var projected = issues
            .Join(cars,
            issue => issue.CarId,
            car => car.Id,
            (issue, car) => new { Issue = issue, Car = car })
            .Select(x => new IssueListDto
            {
                Id = x.Issue.Id,
                CarId = x.Car.Id,
                Vin = x.Car.Vin,
                SrNo = x.Issue.SrNo,
                Description = x.Issue.Description,
                Type = x.Issue.Type,
                Status = x.Issue.Status,
                Stage = x.Car.Stage,
                CreatorId = x.Issue.CreatorId,
                CreationTime = x.Issue.CreationTime
            });

        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            input.Sorting = nameof(IssueListDto.CreationTime) + " DESC";
        }

        if (!string.IsNullOrWhiteSpace(input.Filter = input.Filter?.Trim()))
        {
            projected = projected.Where(x => x.Vin.Contains(input.Filter));
        }

        projected = projected
            .WhereIf(input.Type != null, x => x.Type == input.Type)
            .WhereIf(input.Status != null, x => x.Status == input.Status)
            .WhereIf(input.Stage != null, x => x.Stage == input.Stage);

        var totalCount = await AsyncExecuter.LongCountAsync(projected);
        var items = await AsyncExecuter.ToListAsync(projected.OrderBy(input.Sorting).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<IssueListDto>(totalCount, items);
    }
}