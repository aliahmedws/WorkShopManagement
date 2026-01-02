using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.EntityAttachments;

namespace WorkShopManagement.CheckLists;

public class GetCheckListListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Name { get; set; }
    public int? Position { get; set; }
    public Guid? CarModelId { get; set; }
    public List<EntityAttachmentDto?> Attachments { get; set; } = [];
}
