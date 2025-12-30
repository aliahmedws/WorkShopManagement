using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.CheckLists;

public class CheckListDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public Guid CarModelId { get; set; }
    public CheckListType CheckListType { get; set; }
}
