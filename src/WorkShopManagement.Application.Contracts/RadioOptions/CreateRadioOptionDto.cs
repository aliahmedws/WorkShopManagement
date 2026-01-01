using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.RadioOptions;

public class CreateRadioOptionDto
{
    [Required]
    public Guid ListItemId { get; set; }

    [Required]
    [MaxLength(RadioOptionConsts.MaxNameLength)]
    public string Name { get; set; } = default!;
}
