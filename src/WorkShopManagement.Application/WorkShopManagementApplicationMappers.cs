using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using WorkShopManagement.CarModels;
using WorkShopManagement.Cars;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.External.Nhtsa;
using WorkShopManagement.External.Vpic;
using WorkShopManagement.FileAttachments;
using WorkShopManagement.ListItems;
using WorkShopManagement.Lookups;
using WorkShopManagement.RadioOptions;

namespace WorkShopManagement;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CarModelMapper : MapperBase<CarModel, CarModelDto>
{
    public override partial CarModelDto Map(CarModel source);
    public override partial void Map(CarModel source, CarModelDto destination);

    public partial FileAttachmentDto Map(FileAttachment source);
    public partial void Map(FileAttachment source, FileAttachmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CheckListMapper : MapperBase<CheckList, CheckListDto>
{
    public override partial CheckListDto Map(CheckList source);
    public override partial void Map(CheckList source, CheckListDto destination);
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
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
    public override partial ExternalCarDetailsDto Map(VpicVariableResultDto source);
    public override partial void Map(VpicVariableResultDto source, ExternalCarDetailsDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GuidLookupToGuidLookupDto : MapperBase<GuidLookup, GuidLookupDto>
{
    public override partial GuidLookupDto Map(GuidLookup source);
    public override partial void Map(GuidLookup source, GuidLookupDto destination);
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