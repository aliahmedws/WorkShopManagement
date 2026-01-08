using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkShopManagement.Account;

public class GetTwoFactorProvidersInput
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } = default!;
}
