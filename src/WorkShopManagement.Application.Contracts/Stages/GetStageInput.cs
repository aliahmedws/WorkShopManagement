using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars.Stages;

namespace WorkShopManagement.Stages;

public class GetStageInput : PagedAndSortedResultRequestDto
{
    [Required]
    public Stage? Stage { get; set; }
    public string? Filter { get; set; }
}