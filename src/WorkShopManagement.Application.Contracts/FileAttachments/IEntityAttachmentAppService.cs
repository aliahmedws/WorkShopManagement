using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.FileAttachments;

public interface IEntityAttachmentAppService : IApplicationService
{
    Task<PagedResultDto<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input);

    Task<EntityAttachmentDto> UploadForCheckListAsync(Guid checkListId, IFormFile file);
    Task<EntityAttachmentDto> UploadForListItemAsync(Guid listItemId, IFormFile file);

    Task DeleteAsync(Guid id);
}
