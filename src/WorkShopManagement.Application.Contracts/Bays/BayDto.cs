using System;

namespace WorkShopManagement.Bays;

public class BayDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
}
