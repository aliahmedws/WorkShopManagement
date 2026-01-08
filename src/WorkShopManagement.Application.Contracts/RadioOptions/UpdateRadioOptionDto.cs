using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.RadioOptions;

public class UpdateRadioOptionDto
{
    [Required]
    public Guid ListItemId { get; set; }

    [Required]
    [MaxLength(RadioOptionConsts.MaxNameLength)]
    public string Name { get; set; } = default!;
    public string? ConcurrencyStamp { get; set; }
}
