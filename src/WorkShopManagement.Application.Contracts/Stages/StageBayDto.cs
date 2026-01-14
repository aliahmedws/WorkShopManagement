using System;
using WorkShopManagement.CarBays;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public class StageBayDto
{
    public Guid BayId { get; set; }
    public string BayName { get; set; } = default!;
    public Guid? CarBayId { get; set; }
    public Priority? Priority { get; set; }
    public Guid? CarId { get; set; }
    public string? Vin { get; set; }
    public DateTime? ManufactureStartDate { get; set; }
    public string? OwnerName { get; set; }
    public string? ModelName { get; set; }
    public string? ImageUrl { get; set; }
    public RecallStatus? RecallStatus { get; set; }
    public IssueStatus? IssueStatus { get; set; }
}
