namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class FileAttachmentDto
{
    public string Name { get; set; } = default!;
    public string FileExtension { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string BlobName { get; set; } = default!;
}
