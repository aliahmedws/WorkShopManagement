using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars.Stages;

namespace WorkShopManagement.Issues;

public class GetIssueListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public IssueType? Type { get; set; }
    public IssueStatus? Status { get; set; }
    public Stage? Stage { get; set; }
}
