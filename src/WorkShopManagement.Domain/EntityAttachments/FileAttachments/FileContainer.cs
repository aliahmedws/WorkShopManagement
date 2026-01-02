using Volo.Abp.BlobStoring;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

[BlobContainerName(FileContainerName)]
public class FileContainer
{
    public const string FileContainerName = "files";
}
