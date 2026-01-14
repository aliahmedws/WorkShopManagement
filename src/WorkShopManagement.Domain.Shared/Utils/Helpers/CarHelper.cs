using System;
using System.Text.RegularExpressions;
using Volo.Abp;

namespace WorkShopManagement.Utils.Helpers
{
    public static class CarHelper
    {
        private static readonly Regex VinRegex = new(@"^[A-Z0-9]{17}$", RegexOptions.Compiled);
        public static string NormalizeAndValidateVin(string vin)
        {

            if (vin.IsNullOrWhiteSpace())
                throw new UserFriendlyException("VIN is required.");

            var normalized = vin.Trim().ToUpperInvariant();

            if (!VinRegex.IsMatch(normalized))
                throw new UserFriendlyException("VIN is Invalid. Vin must be 17 chars combination of alphabets and numbers.");

            return normalized;
        }


        // REVIEW THIS: Is this needed?
        public static bool TryGetValidHttpUrl(string? url, out string normalized)
        {
            normalized = string.Empty;

            if (string.IsNullOrWhiteSpace(url))
                return false;

            url = url.Trim();

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            // Only allow http/https
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return false;

            // Must have a host
            if (string.IsNullOrWhiteSpace(uri.Host))
                return false;

            normalized = uri.AbsoluteUri;
            return true;
        }

        // REVIEW THIS: Is this needed?
        // Optional: if you want to ensure it's an image-like URL (basic heuristic)
        public static bool HasCommonImageExtension(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            var path = uri.AbsolutePath;

            return path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase);
        }
    }
}
