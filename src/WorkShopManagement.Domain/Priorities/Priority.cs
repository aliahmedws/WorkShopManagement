using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace WorkShopManagement.Priorities;

public class Priority : FullAuditedEntity<Guid>
{
    [Required]
    [Range(0, 5000)]
    public int Number { get; set; }
    public string? Description { get; set; }
    private Priority() { }
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
           PriorityConsts.MaxNumber
       );
        Number = number;
    }
    private void SetDescription(string? description)
    {
        //if (description.IsNullOrWhiteSpace())
        //{
        //    Description = null;
        //    return;
        //}
        //if (description!.Length > PriorityConsts.MaxDescriptionLength)
        //{
        //    throw new UserFriendlyException(
        //        $"Description length cannot be greater than {PriorityConsts.MaxDescriptionLength}");
        //}

        //Description = description;
        Description = Check.Length(
            description,
            nameof(description),
            maxLength:PriorityConsts.MaxDescriptionLength);
    }
}
