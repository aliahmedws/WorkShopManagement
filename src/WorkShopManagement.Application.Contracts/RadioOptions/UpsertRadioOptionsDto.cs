using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.RadioOptions;

public class UpsertRadioOptionsDto
{
    [Required]
    public Guid ListItemId { get; set; }

    [Required]
    public List<UpsertRadioOptionItemDto> Items { get; set; } = [];

    public string? ConcurrencyStamp { get; set; }
}

public class UpsertRadioOptionItemDto
{
    [Required]
    [MaxLength(RadioOptionConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    public bool IsAcceptable { get; set; }
}
