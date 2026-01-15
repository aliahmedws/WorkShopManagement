using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using WorkShopManagement.Bays;
using WorkShopManagement.CarBayItems;
using WorkShopManagement.CarBays;
using WorkShopManagement.CarModels;
using WorkShopManagement.Cars;
using WorkShopManagement.CheckInReports;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Vpic;
using WorkShopManagement.Issues;
using WorkShopManagement.ListItems;
using WorkShopManagement.LogisticsDetails;
using WorkShopManagement.LogisticsDetails.ArrivalEstimates;
using WorkShopManagement.Lookups;
using WorkShopManagement.ModelCategories;
using WorkShopManagement.QualityGates;
using WorkShopManagement.RadioOptions;
using WorkShopManagement.Recalls;
using WorkShopManagement.Stages;

namespace WorkShopManagement;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ModelCategoryMapper : MapperBase<ModelCategory, ModelCategoryDto>
{
    public override partial ModelCategoryDto Map(ModelCategory source);
    public override partial void Map(ModelCategory source, ModelCategoryDto destination);

    public partial FileAttachmentDto Map(FileAttachment source);
    public partial void Map(FileAttachment source, FileAttachmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CarModelMapper : MapperBase<CarModel, CarModelDto>
{
    public override partial CarModelDto Map(CarModel source);
    public override partial void Map(CarModel source, CarModelDto destination);

    public partial FileAttachmentDto Map(FileAttachment source);
    public partial void Map(FileAttachment source, FileAttachmentDto destination);
}



[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CheckListMapper : MapperBase<CheckList, CheckListDto>
{
    public override partial CheckListDto Map(CheckList source);
    public override partial void Map(CheckList source, CheckListDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CarBayMapper : MapperBase<CarBay, CarBayDto>
{
    [MapProperty("Bay.Name", nameof(CarBayDto.BayName))]
    [MapProperty("Car.Vin", nameof(CarBayDto.CarVin))]
    [MapProperty("Car.Owner.Name", nameof(CarBayDto.OwnerName))]
    [MapProperty("Car.Model.Name", nameof(CarBayDto.ModelName))]
    [MapProperty("Car.Model.ModelCategory.Name", nameof(CarBayDto.ModelCategoryName))]
    [MapProperty("Car.Model.FileAttachments.Path", nameof(CarBayDto.ModelImagePath))]
    [MapProperty("Car.Model.CheckLists", nameof(CarBayDto.CheckLists))]
    public override partial CarBayDto Map(CarBay source);

    [MapProperty("Bay.Name", nameof(CarBayDto.BayName))]
    [MapProperty("Car.Vin", nameof(CarBayDto.CarVin))]
    [MapProperty("Car.Owner.Name", nameof(CarBayDto.OwnerName))]
    [MapProperty("Car.Model.Name", nameof(CarBayDto.ModelName))]
    [MapProperty("Car.Model.ModelCategory.Name", nameof(CarBayDto.ModelCategoryName))]
    [MapProperty("Car.Model.FileAttachments.Path", nameof(CarBayDto.ModelImagePath))]
    [MapProperty("Car.Model.CheckLists", nameof(CarBayDto.CheckLists))]
    public override partial void Map(CarBay source, CarBayDto destination);

    // nested mapping for lists
    public partial CheckListDto Map(CheckList source);
    public partial ListItemDto Map(ListItem source);
    public partial RadioOptionDto Map(RadioOption source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CarBayItemMapper : MapperBase<CarBayItem, CarBayItemDto>
{
    public override partial CarBayItemDto Map(CarBayItem source);
    public override partial void Map(CarBayItem source, CarBayItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class QualityGateMapper : MapperBase<QualityGate, QualityGateDto>
{
    public override partial QualityGateDto Map(QualityGate source);
    public override partial void Map(QualityGate source, QualityGateDto destination);
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class ListItemMapper : MapperBase<ListItem, ListItemDto>
{
    public override partial ListItemDto Map(ListItem source);
    public override partial void Map(ListItem source, ListItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class NhtsaRecallByVehicleResponseEtoToNhtsaRecallByVehicleResponseDtoMapper : MapperBase<NhtsaRecallByVehicleResponseEto, NhtsaRecallByVehicleResponseDto>
{
    public override partial NhtsaRecallByVehicleResponseDto Map(NhtsaRecallByVehicleResponseEto source);
    public override partial void Map(NhtsaRecallByVehicleResponseEto source, NhtsaRecallByVehicleResponseDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class NhtsaRecallByVehicleResultEtoToNhtsaRecallByVehicleResultDtoMapper : MapperBase<NhtsaRecallByVehicleResultEto, NhtsaRecallByVehicleResultDto>
{
    public override partial NhtsaRecallByVehicleResultDto Map(NhtsaRecallByVehicleResultEto source);
    public override partial void Map(NhtsaRecallByVehicleResultEto source, NhtsaRecallByVehicleResultDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CarToCarDtoMapper : MapperBase<Car, CarDto>
{
    public override partial CarDto Map(Car source);
    public override partial void Map(Car source, CarDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class VpicVariableResultDtoToExternalCarDetailsDtoMapper : MapperBase<VpicVariableResultDto, ExternalCarDetailsDto>
{
    [MapperIgnoreTarget(nameof(ExternalCarDetailsDto.Colors))]
    public override partial ExternalCarDetailsDto Map(VpicVariableResultDto source);

    [MapperIgnoreTarget(nameof(ExternalCarDetailsDto.Colors))]
    public override partial void Map(VpicVariableResultDto source, ExternalCarDetailsDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GuidLookupToGuidLookupDto : MapperBase<GuidLookup, GuidLookupDto>
{
    public override partial GuidLookupDto Map(GuidLookup source);
    public override partial void Map(GuidLookup source, GuidLookupDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GuidLookupToIntLookupDto : MapperBase<IntLookup, IntLookupDto>
{
    public override partial IntLookupDto Map(IntLookup source);
    public override partial void Map(IntLookup source, IntLookupDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EntityAttachmentMapper : MapperBase<EntityAttachment, EntityAttachmentDto>
{
    public override partial EntityAttachmentDto Map(EntityAttachment source);
    public override partial void Map(EntityAttachment source, EntityAttachmentDto destination);

    public partial FileAttachmentDto Map(FileAttachment source);
    public partial void Map(FileAttachment source, FileAttachmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RadioOptionMapper : MapperBase<RadioOption, RadioOptionDto>
{
    public override partial RadioOptionDto Map(RadioOption source);
    public override partial void Map(RadioOption source, RadioOptionDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FileAttachmentMapper : MapperBase<FileAttachment, FileAttachmentDto>
{
    public override partial FileAttachmentDto Map(FileAttachment source);
    public override partial void Map(FileAttachment source, FileAttachmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecallMapper : MapperBase<Recall, RecallDto>
{
    [MapperIgnoreTarget(nameof(RecallDto.EntityAttachments))]
    [MapProperty("Car.Vin", nameof(RecallDto.Vin))]
    public override partial RecallDto Map(Recall source);
    [MapperIgnoreTarget(nameof(RecallDto.EntityAttachments))]
    [MapProperty("Car.Vin", nameof(RecallDto.Vin))]
    public override partial void Map(Recall source, RecallDto destination);
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CheckInReportToCheckInReportDtoMapper : MapperBase<CheckInReport, CheckInReportDto>
{
    [MapProperty("Car.StorageLocation", nameof(CheckInReportDto.StorageLocation))]
    public override partial CheckInReportDto Map(CheckInReport source);
    [MapProperty("Car.StorageLocation", nameof(CheckInReportDto.StorageLocation))]
    public override partial void Map(CheckInReport source, CheckInReportDto destination);

    // Add this so Mapperly can generate mapping for the nested property
    public partial CarDto Map(Car source);
    public partial void Map(Car source, CarDto destination);

}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CheckInReportFiltersDtoToInputMapper : MapperBase<CheckInReportFiltersDto, CheckInReportFiltersInput>
{
    public override partial CheckInReportFiltersInput Map(CheckInReportFiltersDto source);
    public override partial void Map(CheckInReportFiltersDto source, CheckInReportFiltersInput destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class IssueToIssueDtoMapper : MapperBase<Issue, IssueDto>
{
    public override partial IssueDto Map(Issue source);
    public override partial void Map(Issue source, IssueDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LogisticsDetailToLogisticsDetailDtoMapper : MapperBase<LogisticsDetail, LogisticsDetailDto>
{
    public override partial LogisticsDetailDto Map(LogisticsDetail source);
    public override partial void Map(LogisticsDetail source, LogisticsDetailDto destination);

    // Nested list mapping
    public partial ArrivalEstimateDto Map(ArrivalEstimate source);
    public partial void Map(ArrivalEstimate source, ArrivalEstimateDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ArrivalEstimateToArrivalEstimateDtoMapper : MapperBase<ArrivalEstimate, ArrivalEstimateDto>
{
    public override partial ArrivalEstimateDto Map(ArrivalEstimate source);
    public override partial void Map(ArrivalEstimate source, ArrivalEstimateDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StageModelToStageDtoMapper : MapperBase<StageModel, StageDto>
{
    public override partial StageDto Map(StageModel source);
    public override partial void Map(StageModel source, StageDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StageBayModelToStageBayDtoMapper : MapperBase<StageBayModel, StageBayDto>
{
    public override partial StageBayDto Map(StageBayModel source);
    public override partial void Map(StageBayModel source, StageBayDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BayToBayDtoMapper : MapperBase<Bay, BayDto>
{
    public override partial BayDto Map(Bay source);
    public override partial void Map(Bay source, BayDto destination);
}