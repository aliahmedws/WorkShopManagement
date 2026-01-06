using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.QualityGates;

public class UpdateQualityGateDto
{
    [Required]
    public GateName GateName { get; set; }

    [Required]
    public QualityGateStatus Status { get; set; }

    [Required]
    public string ConcurrencyStamp { get; set; } = default!;
}
