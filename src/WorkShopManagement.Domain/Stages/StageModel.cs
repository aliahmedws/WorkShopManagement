using System;
using System.Collections.Generic;
using WorkShopManagement.CarBays;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public class StageModel
{
    //--Cars
    public Guid CarId { get; set; }
    public string Vin { get; set; } = default!;
    public string Color { get; set; } = default!;
    public StorageLocation? StorageLocation { get; set; }
    public Guid ModelId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public AvvStatus? AvvStatus { get; set; }
    public DateTime? EstimatedRelease { get; set; }
    public string? Notes { get; set; }

    //--CarOwners
    public string? OwnerName { get; set; }

    //--CarModels
    public string? ModelName { get; set; }

    //--LogisticsDetail
    public Port? Port { get; set; }
    public string? BookingNumber { get; set; }
    public string? ClearingAgent { get; set; }
    public CreStatus? CreStatus { get; set; }
    public DateTime? EtaScd { get; set; }

    //--Recalls
    public IEnumerable<RecallStatus> RecallStatuses { get; set; } = [];
    public RecallStatus? RecallStatus => StageColorHelper.MapRecallStatus(RecallStatuses);

    //--Issues
    public IEnumerable<IssueStatus> IssueStatuses { get; set; } = [];
    public IssueStatus? IssueStatus => StageColorHelper.MapIssueStatus(IssueStatuses);

    //--CarBay
    public CarBay? CarBay { get; set; }
    public Priority? Priority => CarBay?.Priority;
    public Guid? CarBayId => CarBay?.Id;
    public Guid? BayId => CarBay?.BayId;
}