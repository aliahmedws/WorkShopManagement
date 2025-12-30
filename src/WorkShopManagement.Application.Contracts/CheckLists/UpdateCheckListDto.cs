using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.CheckLists;

public class UpdateCheckListDto
{
    [Required]
    [StringLength(CheckListConsts.NameMaxLength)]
    public string Name { get; set; } = default!;

    [Range(0, int.MaxValue)]
    public int Position { get; set; }

    [Required]
    public Guid CarModelId { get; set; }

    [Required]
    public CheckListType CheckListType { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
