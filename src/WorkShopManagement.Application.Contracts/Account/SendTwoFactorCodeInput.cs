using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManagement.Account;

public class SendTwoFactorCodeInput
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Provider { get; set; } = default!;

    [Required]
    public string Token { get; set; } = default!;
}
