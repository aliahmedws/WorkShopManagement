using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.Priorities;

public class UpdatePriorityDto
{
    [Required]
    public int Number { get; set; }
    public string? Description { get; set; }
}
