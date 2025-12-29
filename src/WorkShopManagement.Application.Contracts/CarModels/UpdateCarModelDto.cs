using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.CarModels;

public class UpdateCarModelDto
{
    [Required]
    [StringLength(CarModelConsts.NameMaxLength)]
    public string Name { get; set; } = default!;

    [StringLength(CarModelConsts.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public string ConcurrencyStamp { get; set; } = default!;
}
