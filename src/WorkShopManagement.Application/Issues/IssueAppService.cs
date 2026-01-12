using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
    public async Task<IssueDto> UpsertAsync(Guid carId, UpsertIssueDto input)
    {
        Check.NotDefaultOrNull<Guid>(carId, nameof(carId));
        Check.NotNull(input, nameof(input));

        // Ensure car exists (fail fast, clearer error)
        await _carRepository.GetAsync(carId);

        var existingIssue = await _issueRepository.FirstOrDefaultAsync(i => i.CarId == carId && i.SrNo == input.SrNo);
        if (existingIssue != null && (input.Id == null || input.Id != existingIssue.Id))
        {
            throw new BusinessException("WorkShopManagement:DuplicateIssueSrNo")
            .WithData("CarId", carId)
            .WithData("SrNo", input.SrNo);
        }

        Issue issue = null!;

        if (input.Id.HasValue)
        {
            issue = await _issueRepository.GetAsync(input.Id.Value);
            if (issue.CarId != carId)
            {
                throw new BusinessException("WorkShopManagement:IssueCarMismatch")
                    .WithData("IssueId", issue.Id)
                    .WithData("CarId", carId);
            }

            IssueBuilder.Update(issue, input);
            issue = await _issueRepository.UpdateAsync(issue);
        }
        else
        {
            issue = IssueBuilder.Create(carId, GuidGenerator.Create(), input);
            await _issueRepository.InsertAsync(issue);
        }

        await _attachmentService.UpdateAsync(new UpdateEntityAttachmentDto
        {
            EntityId = issue.Id,
            EntityType = EntityType.Issue,
            TempFiles = input.TempFiles,
            EntityAttachments = input.EntityAttachments
        });

        return ObjectMapper.Map<Issue, IssueDto>(issue);
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