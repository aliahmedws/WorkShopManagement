using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.Priorities;

public class PriorityDto : EntityDto<Guid>
{
    public int Number { get; set; }
    public string? Description { get; set; }
}
