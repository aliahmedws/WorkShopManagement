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
        public Port Port { get; set; }

        [StringLength(LogisticsDetailConsts.MaxBookingNumberLength)]
        public string? BookingNumber { get; set; }

        // Cre Info
        [Required]
        public CreStatus CreStatus { get; set; }
        public DateTime? CreSubmissionDate { get; set; }

        [StringLength(LogisticsDetailConsts.MaxRsvaNumberLength)]
        public string? RsvaNumber { get; set; }

        // Clearance info
        [StringLength(LogisticsDetailConsts.MaxClearingAgentLength)]
        public string? ClearingAgent { get; set; }
        [StringLength(LogisticsDetailConsts.MaxClearanceRemarksLength)]
        public string? ClearanceRemarks { get; set; }
        public DateTime? ClearanceDate { get; set; }

        // Actual Arrival Info
        public DateTime? ActualPortArrivalDate { get; set; }
        public DateTime? ActualScdArrivalDate { get; set; }


        // File Attachments

        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
        public List<FileAttachmentDto> TempFiles { get; set; } = [];

    }
}
