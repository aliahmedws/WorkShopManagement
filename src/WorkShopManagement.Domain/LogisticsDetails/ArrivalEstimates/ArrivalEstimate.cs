using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;

namespace WorkShopManagement.LogisticsDetails.ArrivalEstimates
{
    public class ArrivalEstimate : FullAuditedEntity<Guid>
    {
        public Guid LogisticsDetailId { get; private set; }

        public DateTime EtaPort { get; private set; }
        public DateTime EtaScd { get; private set; }

        public string? Notes { get; private set; }

        private ArrivalEstimate() { }

        public ArrivalEstimate(
            Guid id,
            Guid logisticsDetailId,
            DateTime etaPort,
            DateTime etaScd,
            string? notes = null
        ) : base(id)
        {
            LogisticsDetailId = logisticsDetailId;
            SetEta(etaPort, etaScd);
            SetNotes(notes);
        }

        public void SetEta(DateTime etaPort, DateTime etaScd)
        {
            if (etaPort < etaScd)
            {
                throw new BusinessException(WorkShopManagementDomainErrorCodes.LogisticsInvalidEtaRange)
                    .WithData(nameof(etaPort), etaPort)
                    .WithData(nameof(etaScd), etaScd);
            }

            EtaPort = etaPort;
            EtaScd = etaScd;
        }

        public void SetNotes(string? notes)
        {
            Notes = DomainCheck.TrimOptional(notes, nameof(notes), maxLength: ArrivalEstimateConsts.MaxNotesLength);
        }
    }
}
