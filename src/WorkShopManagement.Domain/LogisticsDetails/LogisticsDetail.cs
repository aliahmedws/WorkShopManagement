using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;
using WorkShopManagement.Extensions;
using WorkShopManagement.LogisticsDetails.ArrivalEstimates;

namespace WorkShopManagement.LogisticsDetails
{

    [Audited]
    public class LogisticsDetail : FullAuditedAggregateRoot<Guid>
    {
        public string? BookingNumber { get; private set; }
        public string? ClearingAgent { get; private set; }
        public string? ClearanceRemarks { get; private set; }
        public DateTime? ClearanceDate { get; private set; }
        public CreStatus CreStatus { get; private set; } = CreStatus.Pending;      
        public DateTime? CreSubmissionDate { get; private set; }
        public string? RvsaNumber { get; private set; } = default!;
        public Port Port { get; private set; }                      // Destination Port (BNE Port)
        public DateTime? ActualPortArrivalDate { get; private set; }
        public DateTime? ActualScdArrivalDate { get; private set; }

        // Dispatching Info 
        public string? DeliverTo { get; private set; }
        public DateTime? ConfirmedDeliverDate { get; private set; }
        public string? DeliverNotes { get; private set; }
        public string? TransportDestination { get; private set; }
        public virtual ICollection<ArrivalEstimate> ArrivalEstimates { get; private set; } = [];

        public Guid CarId { get; private set; }

        private LogisticsDetail() { }

        public LogisticsDetail(
            Guid id,
            Guid carId,
            Port port,
            string? bookingNumber = null
        ) : base(id)
        {
            CarId = Check.NotDefaultOrNull<Guid>(carId, nameof(carId));
            SetPort(port);
            SetBookingNumber(bookingNumber);
        }


        public void SetPort(Port port)
        {
            Port = port.EnsureDefined(nameof(Port));
        }

        public void SetBookingNumber(string? bookingNumber)
        {
            BookingNumber = DomainCheck.TrimOptional(
                bookingNumber,
                nameof(bookingNumber),
                maxLength: LogisticsDetailConsts.MaxBookingNumberLength
            );

            
        }

        // Todo: Add methods in manager.
        public void SetClearanceDetails(string? clearingAgent, string? clearanceRemarks, DateTime? clearanceDate)
        {

            ClearingAgent = DomainCheck.TrimOptional(
                clearingAgent,
                nameof(clearingAgent),
                maxLength: LogisticsDetailConsts.MaxClearingAgentLength
            );

            ClearanceRemarks = DomainCheck.TrimOptional(
                clearanceRemarks,
                nameof(clearanceRemarks),
                maxLength: LogisticsDetailConsts.MaxClearanceRemarksLength
            );

            ClearanceDate = clearanceDate;
        }

        // Todo: Add methods in manager.
        internal void SetCreDetails(string? rvsaNumber, DateTime? creSubmissionDate)
        {
            SetRvsaNumber(rvsaNumber);
            SetCreSubmissionDate(creSubmissionDate);
        }
        internal void SetCreStatus(CreStatus creStatus)
        {
            creStatus.EnsureDefined(nameof(CreStatus));
            CreStatus = creStatus;
        }

        internal void SetActualArrivals(DateTime? actualPortArrivalDate, DateTime? actualScdArrivalDate)
        {
            if (actualPortArrivalDate.HasValue && actualScdArrivalDate.HasValue &&
                actualScdArrivalDate.Value.Date < actualPortArrivalDate.Value.Date)
            {
                var portDate = actualScdArrivalDate.Value.Date.ToString("dd-MMM-yyyy");
                var scdDate = actualScdArrivalDate.Value.Date.ToString("dd-MMM-yyyy");
                throw new UserFriendlyException($"Actual SCD arrival date cannot be earlier than actual port arrival date.</br></br> Port Arrival Date: <strong>{portDate}</strong></br> SCD Arrival Date: <strong>{scdDate}</strong>");
            }

            ActualPortArrivalDate = actualPortArrivalDate;
            ActualScdArrivalDate = actualScdArrivalDate;
        }

        // Todo: Add methods in manager.
        internal void SetDeliverDetails(DateTime? confirmedDeliverDate, string? deliverNotes, string? deliverTo, string? transportDestination  )
        {
            SetConfirmedDeliverDate(confirmedDeliverDate);
            SetDeliverNotes(deliverNotes);
            SetDeliverTo(deliverTo);
            SetTransportDestination(transportDestination);
        }


        internal void SetConfirmedDeliverDate(DateTime? confirmedDeliverDate)
        {
            ConfirmedDeliverDate = confirmedDeliverDate;
        }

        internal void SetDeliverNotes(string? deliverNotes)
        {
            DeliverNotes = DomainCheck.TrimOptional(
                deliverNotes,
                nameof(deliverNotes),
                maxLength: LogisticsDetailConsts.MaxDeliverNotesLength
            );
        }

        internal void SetDeliverTo(string? deliverTo)
        {
            DeliverTo = DomainCheck.TrimOptional(
                deliverTo,
                nameof(deliverTo),
                maxLength: LogisticsDetailConsts.MaxDeliverToLength
            );
        }

        internal void SetTransportDestination(string? transportDestination)
        {
            TransportDestination = DomainCheck.TrimOptional(
                transportDestination,
                nameof(transportDestination),
                maxLength: LogisticsDetailConsts.MaxTransportDestinationLength
            );
        }

        internal void SetRvsaNumber(string? rsvsNumber)
        {
            RvsaNumber = DomainCheck.TrimOptional(
                rsvsNumber,
                nameof(rsvsNumber),
                maxLength: LogisticsDetailConsts.MaxRvsaNumberLength
            );
        }

        internal void SetCreSubmissionDate(DateTime? creSubmissionDate)
        {
            //CreStatus = Check.NotNull(creStatus, nameof(creStatus));
            CreSubmissionDate = creSubmissionDate;
        }

    }

}
