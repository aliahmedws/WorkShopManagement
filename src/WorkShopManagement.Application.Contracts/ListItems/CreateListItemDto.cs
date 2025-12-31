using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.ListItems;

public class CreateListItemDto
{
    [Required]
    public Guid CheckListId { get; set; }

    public int Position { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = default!;

    [StringLength(256)]
    public string? CommentPlaceholder { get; set; }

    public CommentType CommentType { get; set; } = CommentType.String;

    public bool IsAttachmentRequired { get; set; }

    public bool IsSeparator { get; set; }
}
