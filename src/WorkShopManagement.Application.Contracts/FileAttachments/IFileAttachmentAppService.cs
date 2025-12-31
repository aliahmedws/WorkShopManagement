using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WorkShopManagement.FileAttachments;

public interface IFileAttachmentAppService : IApplicationService
{
    Task SetAttachmentAsync(Guid entityId, IFormFile file);
    Task RemoveAttachmentAsync(Guid entityId);
}
