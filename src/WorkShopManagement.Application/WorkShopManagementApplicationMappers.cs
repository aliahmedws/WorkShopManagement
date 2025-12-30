using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using WorkShopManagement.CarModels;
using WorkShopManagement.CheckLists;
using WorkShopManagement.FileAttachments;

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
    public  override partial void Map(CheckList source, CheckListDto destination);
}
