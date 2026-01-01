using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkShopManagement.Priorities;

public class Priority : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [Range(0, 50000)]
    public int Number { get; set; }
    public string? Description { get; set; }

    private Priority()
    {
    }

    internal Priority(
        Guid id,
        int number,
        string? description = null
    ) : base(id)
    {
        SetNumber(number);
        SetDescription(description);
    }

    internal Priority ChangeNumber(int number)
    {
        SetNumber(number);
        return this;
    }

    public Priority ChangeDescription(string? description)
    {
        SetDescription(description);
        return this;
    }

    private void SetNumber(int number)
    {
        Check.Range(
           number,
           nameof(number),
           PriorityConsts.MinNumber,
           PriorityConsts.MaxNumberLength
       );

        Number = number;
    }

    private void SetDescription(string? description)
    {
        if (!description.IsNullOrWhiteSpace())
        {
            Description = Check.NotNullOrWhiteSpace(
                description,
                nameof(description),
                maxLength: PriorityConsts.MaxDescriptionLength
            );
        }
        else
        {
            Description = null;
        }
    }
}
