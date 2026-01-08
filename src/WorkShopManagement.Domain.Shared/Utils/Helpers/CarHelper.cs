using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp;
using WorkShopManagement.Cars;

namespace WorkShopManagement.Utils.Helpers
{
    public static class CarHelper
    {
        private static readonly Regex VinRegex =
        new(@"^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.Compiled);
        public static string NormalizeAndValidateVin(string vin)
        {

            if (vin.IsNullOrWhiteSpace())
                throw new UserFriendlyException("VIN is required.");

            var normalized = vin.Trim().ToUpperInvariant();

            if (!VinRegex.IsMatch(normalized))
                throw new UserFriendlyException("VIN must be exactly 17 characters.");

            return normalized;
        }
    }
}
