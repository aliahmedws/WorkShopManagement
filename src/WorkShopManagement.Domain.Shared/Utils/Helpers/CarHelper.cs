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
    }
}
