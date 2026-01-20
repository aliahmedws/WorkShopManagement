using System;
using WorkShopManagement.CarBays;
using WorkShopManagement.Cars;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public class StageDto
{
    public Guid CarId { get; set; }
    public Stage Stage { get; set; } = default!;        // New Added to get stage again in car bay . prod-detail modal
    public string Vin { get; set; } = default!;
    public string Color { get; set; } = default!;
    public StorageLocation? StorageLocation { get; set; }
    public Guid ModelId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public AvvStatus? AvvStatus { get; set; }
    public DateTime? EstimatedRelease { get; set; }
    public string? Notes { get; set; }
    public string? OwnerName { get; set; }
    public string? ModelName { get; set; }
    public Port? Port { get; set; }
    public string? BookingNumber { get; set; }
    public string? ClearingAgent { get; set; }
    public CreStatus? CreStatus { get; set; }
    public DateTime? EtaScd { get; set; }
    public RecallStatus? RecallStatus { get; set; }
    public IssueStatus? IssueStatus { get; set; }
    public Priority? Priority { get; set; }
    public Guid? CarBayId { get; set; }
    public Guid? BayId { get; set; }
}