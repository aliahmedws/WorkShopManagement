using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.Cars;

public class CreateCarOwnerDto
{
    [Required]
    [StringLength(CarOwnerConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(CarOwnerConsts.MaxEmailLength)]
    public string? Email { get; set; }

    [StringLength(CarOwnerConsts.MaxContactIdLength)]
    public string? ContactId { get; set; }
}
