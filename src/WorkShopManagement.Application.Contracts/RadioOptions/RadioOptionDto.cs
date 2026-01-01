using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.RadioOptions;

public class RadioOptionDto : EntityDto<Guid>
{
    public Guid ListItemId { get; set; }
    public string Name { get; set; } = default!;
}
