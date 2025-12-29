using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using WorkShopManagement.External.Nhtsa;

namespace WorkShopManagement;

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