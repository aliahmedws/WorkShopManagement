using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.LogisticsDetails.ArrivalEstimates;

namespace WorkShopManagement.LogisticsDetails
{
    public class LogisticsDetailDto : FullAuditedEntityDto<Guid>
    {
        public Guid CarId { get; set; }

        public string? BookingNumber { get; set; }
        public string? ClearingAgent { get; set; }
        public string? ClearanceRemarks { get; set; }
        public DateTime? ClearanceDate { get; set; }

        public CreStatus CreStatus { get; set; }
        public DateTime? CreSubmissionDate { get; set; }

        public string? RsvaNumber { get; set; }
        public Port Port { get; set; }

        public DateTime? ActualPortArrivalDate { get; set; }
        public DateTime? ActualScdArrivalDate { get; set; }

        // Dispatching Info
        public string? DeliverTo { get; set; }
        public DateTime? ConfirmedDeliverDate { get; set; }
        public string? DeliverNotes { get; set; }
        public string? TransportDestination { get; set; }
        public List<ArrivalEstimateDto> ArrivalEstimates { get; set; } = [];
        public List<EntityAttachmentDto> EntityAttachments { get; set; } = [];
    }
}
