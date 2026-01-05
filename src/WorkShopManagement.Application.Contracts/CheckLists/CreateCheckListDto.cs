using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.TempFiles;

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
    public List<FileAttachmentDto> TempFiles { get; set; } = [];
}
