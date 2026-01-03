using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.TempFiles;

namespace WorkShopManagement.ListItems;

public class UpdateListItemDto
{
    [Required]
    public Guid CheckListId { get; set; }

    public int Position { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = default!;

    [StringLength(256)]
    public string? CommentPlaceholder { get; set; }

    public CommentType? CommentType { get; set; }

    public bool? IsAttachmentRequired { get; set; }

    public bool? IsSeparator { get; set; }

    [Required]
    public string ConcurrencyStamp { get; set; } = default!;
    public List<FileAttachmentDto> TempFiles { get; set; } = [];
    public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
}
