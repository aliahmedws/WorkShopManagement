using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.RadioOptions;

public class GetRadioOptionListDto
{
    [Required]
    public Guid ListItemId { get; set; }
}
