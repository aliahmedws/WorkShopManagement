using System;
using WorkShopManagement.Cars.Stages;

namespace WorkShopManagement.Issues;

public class IssueListDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string Vin { get; set; } = default!;
    public int SrNo { get; set; }
    public string Description { get; set; } = default!;
    public IssueType Type { get; set; }
    public IssueStatus Status { get; set; }
    public Stage Stage { get; set; }
    public Guid? CreatorId { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreationTime { get; set; }
    public bool HasBay { get; set; }
}