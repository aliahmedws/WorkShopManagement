using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace WorkShopManagement.Account;

public class SendPhoneNumberConfirmationTokenDto
{
    [Required]
    public Guid UserId { get; set; }

    [CanBeNull]
    public string PhoneNumber { get; set; } = default!;
}
