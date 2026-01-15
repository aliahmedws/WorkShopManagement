using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkShopManagement.Cars;
using WorkShopManagement.EntityAttachments.FileAttachments;

namespace WorkShopManagement.LogisticsDetails;

public class CreateLogisticsDetailDto
{
    [Required]
    public Guid CarId { get; set; }

    [Required]
    public Port Port { get; set; }

    [StringLength(LogisticsDetailConsts.MaxBookingNumberLength)]
    public string? BookingNumber { get; set; }
    public List<FileAttachmentDto> TempFiles { get; set; } = [];

}
