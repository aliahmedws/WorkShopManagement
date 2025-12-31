using System;
using Volo.Abp.Application.Dtos;

namespace WorkShopManagement.FileAttachments;

public class GetEntityAttachmentListDto : PagedAndSortedResultRequestDto
{
    public Guid? CheckListId { get; set; }
    public Guid? ListItemId { get; set; }
}
