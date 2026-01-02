using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace WorkShopManagement.Cars.Exceptions;

public class InvalidVinLengthException : BusinessException
{
    public InvalidVinLengthException(string vin) : base(WorkShopManagementDomainErrorCodes.InvalidVinLength)
    {
        WithData("length", vin?.Length ?? 0);
    }
}
