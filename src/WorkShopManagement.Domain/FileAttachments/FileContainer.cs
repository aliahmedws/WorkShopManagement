using Volo.Abp.BlobStoring;

namespace WorkShopManagement.FileAttachments;

[BlobContainerName(FileContainerName)]
public class FileContainer
{
    public const string FileContainerName = "file";
}
