using System;

namespace WorkShopManagement.CarModels;

public class CarModelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
