using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;
using WorkShopManagement.FileAttachments;
using WorkShopManagement.ListItems;

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
/*
 * You can add your own mappings here.
 * [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
 * public partial class WorkShopManagementApplicationMappers : MapperBase<BookDto, CreateUpdateBookDto>
 * {
 *    public override partial CreateUpdateBookDto Map(BookDto source);
 * 
 *    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
 * }
 */

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