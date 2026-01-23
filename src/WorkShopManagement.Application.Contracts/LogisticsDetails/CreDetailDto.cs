using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using WorkShopManagement.Cars;

namespace WorkShopManagement.LogisticsDetails
{
    public class CreDetailDto : EntityDto<Guid>
    {
        public Guid CarId { get; set; }
        public CreStatus CreStatus { get; set; }
        public DateTime? CreSubmissionDate { get; set; }
        [StringLength(LogisticsDetailConsts.MaxRvsaNumberLength)]
        public string? RvsaNumber { get; set; }
    }
}
