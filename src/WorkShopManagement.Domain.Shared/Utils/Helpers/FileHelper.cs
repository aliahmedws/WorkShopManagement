using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Volo.Abp;

namespace WorkShopManagement.EntityAttachments.FileAttachments
{
    public static class FileHelper
    {
        public static string ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.NullField)
                    .WithData("field", nameof(fileName));
            }

            return fileName;
        }

        public static string ValidateFileNameWithExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.NullField)
                    .WithData("field", nameof(fileName));
            }

            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.InvalidFileFormat);
            }

            return fileName;
        }

        public static string ValidateFileExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.InvalidFileFormat);
            }

            return ext;
        }

        public static string NormalizePath(string? path)
        {
            return path?.Replace("\\", "/").Trim('/') ?? "";
        }
    }
}
