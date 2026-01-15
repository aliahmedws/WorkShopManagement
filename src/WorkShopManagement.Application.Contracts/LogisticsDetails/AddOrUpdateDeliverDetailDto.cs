using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.LogisticsDetails
{
    public class AddOrUpdateDeliverDetailDto
    {
        public DateTime? ConfirmedDeliverDate { get; set; }
        [StringLength(LogisticsDetailConsts.MaxDeliverNotesLength)]
        public string? DeliverNotes { get; set; }

        [StringLength(LogisticsDetailConsts.MaxDeliverToLength)]
        public string? DeliverTo { get; set; }

        [StringLength(LogisticsDetailConsts.MaxTransportDestinationLength)]
        public string? TransportDestination { get; set; }

    }
}
