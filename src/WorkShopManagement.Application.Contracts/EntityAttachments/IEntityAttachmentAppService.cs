using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.EntityAttachments;

public interface IEntityAttachmentAppService : IApplicationService
{
    Task<List<EntityAttachmentDto>> GetListAsync(GetEntityAttachmentListDto input);

    Task<List<EntityAttachmentDto>> UploadAttachmentsAsync(UploadAttachmentDto input, List<IFormFile> file);

    Task DeleteAsync(Guid id);
}
