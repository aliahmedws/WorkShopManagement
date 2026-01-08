using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace WorkShopManagement.EntityAttachments.FileAttachments;

public class FileAttachment : ValueObject
{
    public string Name { get; } = default!;
    public string BlobName { get; } = default!;
    public string Path { get; } = default!;

    [NotMapped]
    public string Extension => System.IO.Path.GetExtension(Name);

    private FileAttachment() { }

    public FileAttachment(
        string name,
        string blobName,
        string path
        )
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: FileAttachmentConsts.MaxNameLength);
        BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(blobName), maxLength: FileAttachmentConsts.MaxPathLength);
        Path = Check.NotNullOrWhiteSpace(path, nameof(path), maxLength: FileAttachmentConsts.MaxPathLength);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return BlobName;
        yield return Path;
    }
}
