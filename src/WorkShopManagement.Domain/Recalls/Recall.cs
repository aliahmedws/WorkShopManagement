using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using WorkShopManagement.Cars;

namespace WorkShopManagement.Recalls
{
    [Audited]
    public class Recall : FullAuditedAggregateRoot<Guid>
    {
        // ToDo: Add IsAttachment bool to know if attachments are saved against this recall, Optional Rename ManufactureId
        public string Title { get; private set; } = default!;
        public string? Make { get; private set; }
        public string? RiskDescription { get; private set; }
        public string? ManufactureId { get; private set; }
        public RecallType Type { get; private set; }
        public RecallStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public Guid CarId { get; private set; } = default!;
        public virtual Car? Car { get; set; } = default!;

        private Recall() { }

        public Recall(
            Guid id,
            Guid carId,
            string title,
            RecallType type = RecallType.Recalls,
            RecallStatus status = RecallStatus.Pending,
            string? riskDescription = null,
            string? make = null,
            string? manufactureId = null,
            string? notes = null
        ) : base(id)
        {
            CarId = carId;
            SetTitle(title);
            SetType(type);
            SetStatus(status);
            SetRiskDescription(riskDescription);
            SetMakeManufacture(make, manufactureId);
            SetNotes(notes);
        }

        public void SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), maxLength: RecallConsts.MaxTitleLength).Trim();
        }

        public void SetRiskDescription(string? riskDescription)
        {
            RiskDescription = DomainCheck.TrimOptional(riskDescription, nameof(riskDescription), RecallConsts.MaxRiskDescriptionLength);
        }

        public void SetMakeManufacture(string? make, string? manufactureId)
        {
            Make =  DomainCheck.TrimOptional(make, nameof(make), RecallConsts.MaxMakeLength);
            ManufactureId = DomainCheck.TrimOptional(manufactureId, nameof(manufactureId), RecallConsts.MaxManufactureIdLength);
        }

        public void SetType(RecallType type)
        {
            Type = Check.NotNull(type, nameof(type));
        }

        public void SetStatus(RecallStatus status)
        {
            Status = Check.NotNull(status, nameof(status));
        }

        public void SetNotes(string? notes)
        {
            Notes = DomainCheck.TrimOptional(notes, nameof(notes), RecallConsts.MaxNotesLength);
        }

    }
}
