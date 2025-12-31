using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class FileAttachment : ValueObject
{
    public string Name { get; } = default!;
    public string Path { get; } = default!;
    public string FileExtension { get; } = default!;
    public string BlobName { get; } = default!;

    [NotMapped]
    public string Extension => System.IO.Path.GetExtension(BlobName);

    private FileAttachment() { }

    public FileAttachment(
        string name,
        string path,
        string blobName)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: FileAttachmentConsts.MaxNameLength);
        Path = Check.NotNullOrWhiteSpace(path, nameof(path), maxLength: FileAttachmentConsts.MaxPathLength);
        BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(blobName));
        FileExtension = System.IO.Path.GetExtension(BlobName);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Path;
        yield return BlobName;
        yield return FileExtension;
    }
}
