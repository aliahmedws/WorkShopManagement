using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.LogisticsDetails
{
    public class UpdateLogisticsDetailDto
    {
        [Required]
        public Guid CarId { get; set; }

        [StringLength(LogisticsDetailConsts.MaxBookingNumberLength)]
        public string? BookingNumber { get; set; }

        [StringLength(LogisticsDetailConsts.MaxClearingAgentLength)]
        public string? ClearingAgent { get; set; }

        [StringLength(LogisticsDetailConsts.MaxClearanceRemarksLength)]
        public string? ClearanceRemarks { get; set; }

        public DateTime? ClearanceDate { get; set; }
        public DateTime? CreSubmissionDate { get; set; }

        [StringLength(LogisticsDetailConsts.MaxRsvaNumberLength)]
        public string? RsvaNumber { get; set; }

        public Port Port { get; set; } = Port.Bne;

        public DateTime? ActualPortArrivalDate { get; set; }
        public DateTime? ActualScdArrivalDate { get; set; }

        // Dispatching Info
        [StringLength(LogisticsDetailConsts.MaxDeliverToLength)]
        public string? DeliverTo { get; set; }

        public DateTime? ConfirmedDeliverDate { get; set; }

        [StringLength(LogisticsDetailConsts.MaxConfirmedDeliverDateNotesLength)]
        public string? DeliverNotes { get; set; }

        [StringLength(LogisticsDetailConsts.MaxTransportDestinationLength)]
        public string? TransportDestination { get; set; }

        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        public List<FileAttachmentDto> TempFiles { get; set; } = [];

    }
}
