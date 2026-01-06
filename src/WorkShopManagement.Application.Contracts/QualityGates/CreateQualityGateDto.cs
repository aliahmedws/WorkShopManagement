using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.QualityGates;

public class CreateQualityGateDto
{
    [Required]
    public GateName GateName { get; set; }

    [Required]
    public QualityGateStatus Status { get; set; }
}
