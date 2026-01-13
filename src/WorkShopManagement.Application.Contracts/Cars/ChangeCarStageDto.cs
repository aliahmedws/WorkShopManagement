using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;

namespace WorkShopManagement.Cars;

public class ChangeCarStageDto
{
    public Stage TargetStage { get; set; }
    public StorageLocation StorageLocation { get; set; }
}

