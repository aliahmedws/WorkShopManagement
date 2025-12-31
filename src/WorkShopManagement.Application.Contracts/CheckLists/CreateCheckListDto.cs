using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.CheckLists;

public class CreateCheckListDto
{
    [Required]
    [StringLength(CheckListConsts.NameMaxLength)]
    public string Name { get; set; } = default!;

    [Range(0, int.MaxValue)]
    public int Position { get; set; }
    [Required]
    public Guid CarModelId { get; set; }
    public bool? EnableIssueItems { get; set; }
    public bool? EnableTags { get; set; }
    public bool? EnableCheckInReport { get; set; }
}
