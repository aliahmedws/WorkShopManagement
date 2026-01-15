namespace WorkShopManagement.LogisticsDetails
{
    public static class LogisticsDetailConsts
    {
        // Strings
        public const int MaxBookingNumberLength = 64;
        public const int MaxClearingAgentLength = 128;
        public const int MaxClearanceRemarksLength = 1024;
        public const int MaxRsvaNumberLength = 64;
        public const int MaxDeliverNotesLength = 1024;
        public const int MaxTransportDestinationLength = 128;
        public const int MaxDeliverToLength = 128;

        // Sorting
        public const string DefaultSorting = "CreationTime DESC";

        public static string GetNormalizedSorting(string? sorting)
        {
            sorting = sorting?.Trim();

            if (string.IsNullOrWhiteSpace(sorting))
            {
                return DefaultSorting;
            }

            // Map "friendly" properties to real ones if needed in future.
            // For now, pass through.
            return sorting;
        }
    }
}
