using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.Priorities;
public class CreatePriorityDto
{
    [Required]
    [Range(PriorityConsts.MinNumber, PriorityConsts.MaxNumber)]
    public int Number { get; set; }

    [StringLength(PriorityConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
