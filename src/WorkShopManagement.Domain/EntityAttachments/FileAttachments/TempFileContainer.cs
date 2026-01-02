using Volo.Abp.BlobStoring;

namespace WorkShopManagement.EntityAttachments.FileAttachments
{
    [BlobContainerName(FileContainerName)]
    public class TempFileContainer
    {
        public const string FileContainerName = "temp-files";
    }
}
