using System;
using System.Collections.Generic;
using WorkShopManagement.CarBays;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public class StageBayModel
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

    //--Recalls
    public IEnumerable<RecallStatus> RecallStatuses { get; set; } = [];
    public RecallStatus? RecallStatus => StageColorHelper.MapRecallStatus(RecallStatuses);

    //--Issues
    public IEnumerable<IssueStatus> IssueStatuses { get; set; } = [];
    public IssueStatus? IssueStatus => StageColorHelper.MapIssueStatus(IssueStatuses);
}
