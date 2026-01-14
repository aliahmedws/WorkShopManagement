using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;
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
        public CreStatus CreStatus { get; private set; } = CreStatus.Pending;       // TODO: Will be needed only if enforce attachment for cre as well. 
        public DateTime? CreSubmissionDate { get; private set; }
        public string? RsvaNumber { get; private set; } = default!;
        public Port Port { get; private set; } = Port.Bne;                          // Destination Port (BNE Port)
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
            Port port = Port.Bne,
            string? bookingNumber = null
        ) : base(id)
        {
            SetCarId(carId);
            SetPort(port);
            SetBookingNumber(bookingNumber);
        }

        public void SetCarId(Guid carId)
        {
            if (carId == Guid.Empty)
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.LogisticsEmptyCarId)
                    .WithData(nameof(carId), carId);
            }

            CarId = Check.NotNull(carId, nameof(carId));
        }

        public void SetPort(Port port)
        {
            Port = Check.NotNull(port, nameof(port));
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
        public void SetCreDetails(string? rsvaNumber, DateTime? creSubmissionDate)
        {
            SetRsvaNumber(rsvaNumber);
            SetCreSubmissionDate(creSubmissionDate);
        }
        public void SetCreStatus(CreStatus creStatus)
        {
            CreStatus = creStatus;
        }

        public void SetActualArrivals(DateTime? actualPortArrivalDate, DateTime? actualScdArrivalDate)
        {
            if (actualPortArrivalDate.HasValue && actualScdArrivalDate.HasValue &&
                actualScdArrivalDate.Value < actualPortArrivalDate.Value)
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.LogisticsInvalidActualArrivalRange)
                    .WithData(nameof(actualPortArrivalDate), actualPortArrivalDate.Value)
                    .WithData(nameof(actualScdArrivalDate), actualScdArrivalDate.Value);
            }

            ActualPortArrivalDate = actualPortArrivalDate;
            ActualScdArrivalDate = actualScdArrivalDate;
        }

        // Todo: Add methods in manager.
        public void SetDeliverDetails(DateTime? confirmedDeliverDate, string? deliverNotes, string? deliverTo, string? transportDestination  )
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
                maxLength: LogisticsDetailConsts.MaxConfirmedDeliverDateNotesLength
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

        internal void SetRsvaNumber(string? rsvsNumber)
        {
            RsvaNumber = DomainCheck.TrimOptional(
                rsvsNumber,
                nameof(rsvsNumber),
                maxLength: LogisticsDetailConsts.MaxRsvaNumberLength
            );
        }

        internal void SetCreSubmissionDate(DateTime? creSubmissionDate)
        {
            //CreStatus = Check.NotNull(creStatus, nameof(creStatus));
            CreSubmissionDate = creSubmissionDate;
        }

    }

}
