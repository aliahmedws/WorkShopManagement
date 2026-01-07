using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.Account;

public class ConfirmPhoneNumberInput
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } = default!;
}
