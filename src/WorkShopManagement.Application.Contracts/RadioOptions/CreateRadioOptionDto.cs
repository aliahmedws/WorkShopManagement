using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.RadioOptions;

public class CreateRadioOptionDto
{
    [Required]
    public Guid ListItemId { get; set; }

    [Required]
    [MaxLength(RadioOptionConsts.MaxNameLength)]
    public List<string> Names { get; set; } = default!;
}
